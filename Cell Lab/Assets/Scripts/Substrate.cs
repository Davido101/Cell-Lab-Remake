using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.SceneManagement;

public class Substrate : MonoBehaviour
{
    public float maxScale;
    public float zoom;
    public float zoomSpeed;
    public float zoomSnap;
    public float moveSpeed;
    public Vector3 originalPos;

    public List<CellInfo> cellTypes;
    public Type cellType = typeof(Phagocyte);
    public List<Cell> cells = new List<Cell>();
    public List<Food> foods = new List<Food>();
    public GameObject defaultCell;
    public GameObject defaultFood;

    public Dictionary<int, GridCell> interactionGrid;
    public float interactionCellWidth;
    public int interactionGridLength;
    
    public float radius;
    public float temperature;
    public float dynamicFriction;
    public float lightAmount;
    public float lightRange;
    public float lightAngle;
    public float age = 0;

    public new Camera camera;
    public RNG rng = new RNG();
    public bool updateCamera = true;

    public Material substrateShader;
    public static string substrateFile = "";

    private void Awake()
    {
        cellTypes = new List<CellInfo>() {
             new CellInfo(typeof(Phagocyte), LoadShader("Cells/Materials/PhagocyteMaterial")),
             new CellInfo(typeof(Flagellocyte), LoadShader("Cells/Materials/FlagellocyteMaterial")),
             new CellInfo(typeof(Devorocyte), LoadShader("Cells/Materials/DevorocyteMaterial")),
             new CellInfo(typeof(Photocyte), LoadShader("Cells/Materials/PhotocyteMaterial")),
             new CellInfo(typeof(Keratinocyte), LoadShader("Cells/Materials/KeratinocyteMaterial")),
             new CellInfo(typeof(Lipocyte), LoadShader("Cells/Materials/LipocyteMaterial"))
        };
        interactionGrid = new Dictionary<int, GridCell>();
    }

    void Start()
    {
        if (substrateFile != "")
        {
            Substrate self = GetComponent<Substrate>();
            bool success = FileManager.LoadLegacySubstrate(substrateFile, ref self);
            substrateFile = "";
            if (!success)
            {
                Debug.Log("Error loading substrate.");
                SceneManager.LoadScene(0);
            }
        }

        transform.localScale = new Vector3(radius * 2.04f, radius * 2.04f, 1);
        camera = (Camera)FindObjectOfType(typeof(Camera));
        zoom = 1.2f * radius;
        camera.orthographicSize = zoom;


        interactionGridLength = Mathf.CeilToInt(radius * 2 / interactionCellWidth);

        substrateShader = GetComponent<SpriteRenderer>().material;
        lightAngle = Mathf.PI / 2;

        lightRange = 0.2f;
        AdjustSpeed();
        
        SpawnCell(typeof(Flagellocyte), -50, 0, new Color(0.7019f, 1f, 0.2235f));
        SpawnCell(typeof(Phagocyte), 50, 0, new Color(0.7019f, 1f, 0.2235f));
    }

    public Material LoadShader(string shaderPath)
    {
        return Resources.Load<Material>(shaderPath);
    }

    public void update()
    {
        if (updateCamera)
            UpdateCamera();
        UpdateShader();
        AdjustSpeed();
        foreach (Cell cell in cells)
        {
            cell.update();
        }

        foreach (Food food in foods)
        {
            food.update();
        }
    }

    public void fixedupdate()
    {
        List<Cell> deadCells = new List<Cell>();
        foreach (Cell cell in cells)
        {
            if (cell.dead)
            {
                deadCells.Add(cell);
                continue;
            }
        }

        List<Food> eatenFood = new List<Food>();
        foreach (Food food in foods)
        {
            if (food.eaten)
            {
                eatenFood.Add(food);
                continue;
            }
        }

        foreach (Cell cell in deadCells)
        {
            cells.Remove(cell);
            cell.Destroy();
        }

        foreach (Food food in eatenFood)
        {
            foods.Remove(food);
            food.Destroy();
        }

        float dt = temperature / Time.timeScale / 50;
        age += dt;
        foreach (Cell cell in cells)
        {
            if (cell.optimizedInteractions) OptimizedInteractions(cell, dt);
            else Interactions(cell, dt);
        }

        foreach (Cell cell in cells)
        {
            cell.fixedupdate(dt);
        }

        foreach (Food food in foods)
        {
            food.fixedupdate(dt);
        }
    }

    public void Clear()
    {
        foreach (Cell cell in cells)
        {
            cell.Destroy();
        }

        foreach (Food food in foods)
        {
            food.Destroy();
        }

        cells.Clear();
        foods.Clear();
    }

    public void Save(string path)
    {
        Substrate self = GetComponent<Substrate>();
        FileManager.SaveLegacySubstrate(path, ref self);
    }

    public void AdjustSpeed()
    {
        Time.timeScale = Mathf.Clamp(temperature, 1, 100);
    }

    public Cell? SpawnCell(Type cellType, float x, float y, Color color)
    {
        Vector2 position = new Vector2(x, y);
        if (position.sqrMagnitude >= radius * radius) return null;
        GameObject cellObject = Instantiate(defaultCell, new Vector3(x, y, 0), new Quaternion());
        SpriteRenderer renderer = cellObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        Cell cell = cellObject.AddComponent(cellType) as Cell;
        renderer.material = cell.shader;
        cell.shader = renderer.material;
        float scaleConst = renderer.material.GetFloat("scaleConst") / 200;
        cellObject.transform.GetChild(0).localScale = new Vector3(scaleConst / 3.2f, scaleConst / 3.2f, scaleConst / 3.2f);
        cell.position = position;
        cell.lastPosition = position;
        cell.color = color;
        cell.substrate = this;
        cell.gridID = cell.ToGridID();
        AddToGridCell(cell.gridID, cell);
        renderer.sortingOrder = 2;
        cells.Add(cell);
        return cell;
    }

    public Food SpawnFood(float x, float y)
    {
        return SpawnFood(x, y, 0.1875f, 0);
    }

    public Food SpawnFood(float x, float y, float size, float coating)
    {

        GameObject foodObject = Instantiate(defaultFood, new Vector3(x * radius, y * radius, 0), new Quaternion());
        Renderer renderer = foodObject.GetComponent<Renderer>();
        Food food = foodObject.AddComponent<Food>();
        renderer.material = food.shader;
        food.shader = renderer.material;
        food.position = new Vector2(x * (radius / 500), y * (radius / 500));
        food.size = size;
        food.coating = coating;
        food.substrate = this;
        food.gridID = food.ToGridID();
        AddToGridCell(food.gridID, food);
        food.update();
        foods.Add(food);
        return food;
    }

    public void SpawnFoodLump(float x, float y, int foodCount, float lumpSize, float foodSize = 0.1875f, float coating = 0)
    {
        for (int i = 0; i < foodCount; i++)
        {
            float foodX = x + rng.Gaussian() * lumpSize;
            float foodY = y + rng.Gaussian() * lumpSize;
            if (foodX * foodX + foodY * foodY < radius * radius)
                SpawnFood(foodX, foodY, foodSize, coating);
        }
    }

    public void SpawnCellEvent()
    {
        Vector3 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        SpawnCell(cellType, mousePos.x, mousePos.y, new Color(0.7019f, 1f, 0.2235f));
    }

    public void UpdateCamera()
    {
        float maxZoom = maxScale * radius;

        float scrolling = Input.GetAxis("Mouse ScrollWheel");
        if (scrolling != 0)
        {
            float zooming = Mathf.Pow(zoomSpeed, -scrolling);
            zoom *= zooming;
            zoom = Mathf.Clamp(zoom, 0.0001f, maxZoom);
        }
        float zoomLerping = 1 - Mathf.Pow(1 - zoomSnap, Time.deltaTime);
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, zoom, zoomLerping);
        // zoomUI.text = $"x{Mathf.Floor((1.25f / zoom) * 50)}";

        if (Input.GetMouseButtonDown(1))
        {
            originalPos = camera.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(1))
        {
            Vector3 movement = originalPos - camera.ScreenToWorldPoint(Input.mousePosition);
            camera.transform.position += movement;
        }

        Vector3 bottomLeft = camera.ScreenToWorldPoint(Vector3.zero);
        Vector3 topRight = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        float maxZoomX = maxZoom * Screen.width / Screen.height;

        float leftDiff = Mathf.Max(-bottomLeft.x - maxZoomX, 0);
        float rightDiff = Mathf.Max(topRight.x - maxZoomX, 0);
        float bottomDiff = Mathf.Max(-bottomLeft.y - maxZoom, 0);
        float topDiff = Mathf.Max(topRight.y - maxZoom, 0);
        camera.transform.position += new Vector3(leftDiff - rightDiff, bottomDiff - topDiff, 0);
    }

    public void UpdateShader()
    {
        //substrateShader.SetFloat("radius", radius); // we actually shouldn't set this
        substrateShader.SetFloat("amount", lightAmount * 2); // temporary fix to light bug (remove when fixed)
        substrateShader.SetFloat("lrange", lightRange);
        substrateShader.SetFloat("dirX", MathF.Cos(lightAngle));
        substrateShader.SetFloat("dirY", MathF.Sin(lightAngle));
    }

    public int ToGridID(int x, int y) {
        return x + y * interactionGridLength;
    }

    public (int, int) ToGridXY(int gridID)
    {
        return (gridID % interactionGridLength, gridID / interactionGridLength);
    }

    public void OptimizedInteractions(Cell cell1, float dt) {
        (int gridX, int gridY) = ToGridXY(cell1.gridID);
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int gridID = ToGridID(gridX + x, gridY + y);
                if (interactionGrid.ContainsKey(gridID))
                {
                    foreach (Cell cell2 in interactionGrid[gridID].cells)
                    {
                        Interact(cell1, cell2, dt);
                    }

                    if (!cell1.reactsToFood) continue;
                    foreach (Food food in interactionGrid[gridID].foods)
                    {
                        Interact(cell1, food, dt);
                    }
                }
            }
        }
    }

    public void Interactions(Cell cell1, float dt)
    {
        foreach (Cell cell2 in cells)
        {
            Interact(cell1, cell2, dt);
        }

        if (!cell1.reactsToFood) return;
        foreach (Food food in foods)
        {
            Interact(cell1, food, dt);
        }
    }

    public bool Interact(Cell cell1, Cell cell2, float dt)
    {
        if (cell1.Equals(cell2)) return false;
        cell1.React(cell2, dt);
        return true;
    }

    public bool Interact(Cell cell, Food food, float dt)
    {
        if (!cell.reactsToFood) return false;
        cell.React(food, dt);
        return true;
    }

    public GridCell GridCellAt(int gridID)
    {
        if (interactionGrid.ContainsKey(gridID))
        {
            return interactionGrid[gridID];
        }
        else
        {
            return interactionGrid[gridID] = new GridCell(); ;
        }
    }

    public void AddToGridCell(int gridID, SubstrateElement element)
    {
        GridCell gridCell = GridCellAt(gridID);
        if (element is Cell)
        {
            gridCell.cells.Add(element as Cell);
        }
        else if (element is Food)
        {
            gridCell.foods.Add(element as Food);
        }
    }

    public void RemoveFromGridCell(int gridID, SubstrateElement element)
    {
        GridCell gridCell = GridCellAt(gridID);
        if (element is Cell)
        {
            gridCell.cells.Remove(element as Cell);
        }
        else if (element is Food)
        {
            gridCell.foods.Remove(element as Food);
        }
        CheckGridCellEmpty(gridID);
    }

    public bool CheckGridCellEmpty(int gridID)
    {

        GridCell gridCell = interactionGrid[gridID];
        if (gridCell.cells.Count == 0 && gridCell.foods.Count == 0)
        {
            interactionGrid.Remove(gridID);
            return true;
        }
        return false;
    }

    public float LightAmountAt(float x, float y)
    {
        float weightedPos = (Mathf.Cos(lightAngle) * x + Mathf.Sin(lightAngle) * y) / (radius * 2);
        float weightedDist = 1 + (1 / lightRange - 1) * (1 - Mathf.Sqrt(x * x + y * y) / (radius * 2));
        return Mathf.Max(0, ((1 - lightRange) * weightedPos + lightRange) / (weightedDist * weightedDist)) * lightAmount;
    }
}
