using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Jobs;
using Unity.Collections;

public class Substrate : MonoBehaviour
{
    public NativeList<JobHandle> threads;
    public int maxThreads = 1; // For now the game crashes if this is set any higher

    public float maxScale = 2.25f;
    public float zoom = 1;
    public float zoomSpeed = 3.5f;
    public float zoomSnap = 0.99f;
    public float moveSpeed = 3.5f;
    public Vector3 originalPos;

    public List<CellInfo> cellTypes;
    public Type cellType = typeof(Phagocyte);
    public List<Cell> cells = new List<Cell>();
    public List<Food> foods = new List<Food>();
    public GameObject defaultCell;
    public GameObject defaultFood;

    public Dictionary<int, GridCell> interactionGrid;
    public float interactionSquareWidth = 0.1f;
    public int interactionGridLength;
    
    public float radius = 1;
    public float temperature = 1;
    public float dynamicFriction = 5;
    public float lightAmount = 1;
    public float lightRange = 1;
    public float lightAngle = 0;

    public new Camera camera;
    public TMP_Text zoomUI;
    public RNG rng = new RNG();
    public bool updateCamera = true;

    public Material substrateShader;

    private void Awake()
    {
        cellTypes = new List<CellInfo>() {
             new CellInfo(typeof(Phagocyte), LoadIcon("Cells/phagocyte")),
             new CellInfo(typeof(Flagellocyte), LoadIcon("Cells/flagellocyte")),
             new CellInfo(typeof(Devorocyte), LoadIcon("Cells/devorocyte")),
             new CellInfo(typeof(Photocyte), LoadIcon("Cells/photocyte")),
        };
    }

    void Start()
    {
        transform.localScale = new Vector3(radius * 2.04f, radius * 2.04f, 1);
        camera = (Camera)FindObjectOfType(typeof(Camera));
        zoom = maxScale * radius;
        camera.orthographicSize = zoom;


        interactionGridLength = Mathf.CeilToInt(radius * 2 / interactionSquareWidth);

        substrateShader = GetComponent<SpriteRenderer>().material;
        lightAngle = Mathf.PI / 2;

        lightRange = 0.2f;
        AdjustSpeed();
        Cell flagellocyte = SpawnCell(typeof(Flagellocyte), -0.9f, 0, new Color(0.7019f, 1f, 0.2235f));
        flagellocyte.angle = Mathf.PI;
    }

    public Sprite LoadIcon(string resourcePath)
    {
        return Resources.Load<GameObject>(resourcePath).GetComponent<SpriteRenderer>().sprite;
    }

    public void update()
    {
        if (updateCamera)
            UpdateCamera();
        UpdateLight();
        AdjustSpeed();
        foreach (Cell cell in cells)
        {
            cell.update();
        }
    }

    public void fixedupdate()
    {
        List<Cell> deadCells = new List<Cell>();
        interactionGrid = new Dictionary<int, GridCell>();
        foreach (Cell cell in cells)
        {
            if (cell.dead)
            {
                deadCells.Add(cell);
                continue;
            }

            int gridID = ToGridID(cell);
            cell.gridID = gridID;
            if (interactionGrid.ContainsKey(gridID))
            {
                interactionGrid[gridID].cells.Add(cell);
            }
            else
            {
                interactionGrid[gridID] = new GridCell();
                interactionGrid[gridID].cells.Add(cell);
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

            int gridID = ToGridID(food);
            food.gridID = gridID;
            if (interactionGrid.ContainsKey(gridID))
            {
                interactionGrid[gridID].foods.Add(food);
            }
            else
            {
                interactionGrid[gridID] = new GridCell();
                interactionGrid[gridID].foods.Add(food);
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
        //threads = new NativeList<JobHandle>(Allocator.Temp);
        foreach (Cell cell in cells)
        {
            if (cell.optimizedInteractions) OptimizedInteractions(cell, dt);
            else Interactions(cell, dt);
        }
        //JobHandle.CompleteAll(threads);
        //threads.Clear();

        foreach (Cell cell in cells)
        {
            cell.fixedupdate(dt);
        }

        foreach (Food food in foods)
        {
            food.fixedupdate(dt);
        }
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
        cell.position = position;
        cell.lastPosition = position;
        cell.color = color;
        cell.substrate = this;
        renderer.sortingOrder = 1;
        cells.Add(cell);
        return cell;
    }

    public Food SpawnFood(float x, float y)
    {
        return SpawnFood(x, y, 1.2f, 0);
    }

    public Food SpawnFood(float x, float y, float size, float coating)
    {

        GameObject foodObject = Instantiate(defaultFood, new Vector3(x * radius, y * radius, 0), new Quaternion());
        Food food = foodObject.AddComponent<Food>();
        food.position = new Vector2(x * radius, y * radius);
        food.size = size;
        food.coating = coating;
        food.substrate = this;
        food.fixedupdate(0);
        foods.Add(food);
        return food;
    }

    public void SpawnFoodLump(float x, float y, int foodCount, float lumpSize, float foodSize = 1.2f, float coating = 0)
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
        zoomUI.text = $"x{Mathf.Floor((1.25f / zoom) * 50)}";

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

    public void UpdateLight()
    {
        substrateShader.SetFloat("amount", lightAmount * 2); // temporary fix to light bug (remove when fixed)
        substrateShader.SetFloat("lrange", lightRange);
        substrateShader.SetFloat("dirX", MathF.Cos(lightAngle));
        substrateShader.SetFloat("dirY", MathF.Sin(lightAngle));
    }

    public int ToGridID(Cell cell)
    {
        return ToGridID(cell.position.x, cell.position.y);
    }

    public int ToGridID(Food food)
    {
        return ToGridID(food.position.x, food.position.y);
    }

    public int ToGridID(float x, float y) {
        return ToGridID(Mathf.FloorToInt((x + radius) / interactionSquareWidth), Mathf.FloorToInt((y + radius) / interactionSquareWidth));
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
        if (maxThreads == 1) cell1.React(cell2, dt);
        else
        {
            while (threads.Length >= maxThreads)
            {
                for (int i = 0; i < threads.Length; i++)
                {
                    if (threads[i].IsCompleted)
                    {
                        threads.RemoveAt(i);
                        i--;
                    }
                }
            }

            //threads.Add(new Interaction() { cell1 = _cell1, cell2 = _cell2 }.Schedule());
        }
        return true;
    }

    public bool Interact(Cell cell, Food food, float dt)
    {
        if (!cell.reactsToFood) return false;
        if (maxThreads == 1) cell.React(food, dt);
        else
        {
            while (threads.Length >= maxThreads)
            {
                for (int i = 0; i < threads.Length; i++)
                {
                    if (threads[i].IsCompleted)
                    {
                        threads.RemoveAt(i);
                        i--;
                    }
                }
            }

            //threads.Add(new Interaction() { cell1 = _cell1, cell2 = _cell2 }.Schedule());
        }
        return true;
    }

    public float LightAmountAt(float x, float y)
    {
        float weightedPos = (Mathf.Cos(lightAngle) * x + Mathf.Sin(lightAngle) * y) / (radius * 2);
        float weightedDist = 1 + (1 / lightRange - 1) * (1 - Mathf.Sqrt(x * x + y * y) / (radius * 2));
        return Mathf.Max(0, ((1 - lightRange) * weightedPos + lightRange) / (weightedDist * weightedDist)) * lightAmount;
    }
}
