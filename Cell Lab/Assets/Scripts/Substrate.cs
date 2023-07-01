using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class Substrate : MonoBehaviour
{
    const float maxScale = 1.25f;
    public float zoomSpeed = 3.5f;
    public float zoomSnap = 0.99f;
    public float moveSpeed = 3.5f;
    Vector3 originalPos;
    public List<Cell> cells = new List<Cell>();
    public List<Food> foods = new List<Food>();
    public GameObject defaultCell;
    public GameObject defaultFood;
    public float radius = 1;
    public float zoom = 1;
    public float temperature = 1;
    public Camera camera;
    public TMP_Text zoomUI;
    public RNG rng = new RNG();

    void Start()
    {
        this.transform.localScale = new Vector3(radius, radius, 1);
        camera = (Camera)GameObject.FindObjectOfType(typeof(Camera));
        zoom = maxScale * radius;
        camera.orthographicSize = zoom;
        AdjustSpeed();
        SpawnCell(typeof(Phagocyte), 0.5f, 0.5f, new Color(0.7019f, 1f, 0.2235f));
        SpawnCell(typeof(Phagocyte), -0.5f, -0.5f, new Color(0.7019f, 1f, 0.2235f));
        SpawnFoodLump(0, 0, 500, 0.05f);
    }

    public void update()
    {
        UpdateCamera();
        AdjustSpeed();
        List<Cell> deadCells = new List<Cell>();
        foreach (Cell cell in cells)
        {
            cell.update();
            if (cell.dead)
                deadCells.Add(cell);
        }
        foreach (Cell cell in deadCells)
        {
            cells.Remove(cell);
            cell.Destroy();
        }
    }

    public void fixedupdate()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            for (int j = 0; j < cells.Count; j++)
            {
                if (i != j && !cells[i].dead && !cells[j].dead) cells[i].React(cells[j]);
            }
        }

        float deltaT = temperature / Time.timeScale / 50;
        foreach (Cell cell in cells)
        {
            cell.fixedupdate(deltaT);
        }

        foreach (Food food in foods)
        {
            food.fixedupdate();
        }
    }

    void AdjustSpeed()
    {
        Time.timeScale = Mathf.Clamp(temperature, 1, 100);
    }

    Cell SpawnCell(Type cellType, float x, float y, Color color)
    {
        GameObject cellObject = Instantiate(defaultCell, new Vector3(x * radius, y * radius, 0), new Quaternion());
        Cell cell = cellObject.AddComponent(cellType) as Cell;
        cell.position = new Vector2(x * radius, y * radius);
        cell.velocity = new Vector2(0.07f, 0.07f);
        cell.color = color;
        cell.substrate = this;
        cells.Add(cell);
        return cell;
    }

    Food SpawnFood(float x, float y)
    {
        return SpawnFood(x, y, 1.2f, 0);
    }

    Food SpawnFood(float x, float y, float size, float coating)
    {
        GameObject foodObject = Instantiate(defaultFood, new Vector3(x * radius, y * radius, 0), new Quaternion());
        Food food = foodObject.AddComponent<Food>();
        food.position = new Vector2(x * radius, y * radius);
        food.size = size;
        food.coating = coating;
        food.substrate = this;
        food.fixedupdate();
        // I will later on implement something like in the original game's source code where you can pass down a deltaT of 0 to initialize the object
        // Here I just want the radius to set immediately. I could do it manually but I don't want to rewrite code where I don't have to
        foods.Add(food);
        return food;
    }

    void SpawnFoodLump(float x, float y, int foodCount, float lumpSize)
    {
        SpawnFoodLump(x, y, foodCount, lumpSize, 1.2f, 0);
    }

    void SpawnFoodLump(float x, float y, int foodCount, float lumpSize, float foodSize, float coating)
    {
        Debug.Log(foodCount);
        for (int i = 0; i < foodCount; i++)
        {
            Debug.Log(i);
            float foodX = x + rng.Gaussian() * lumpSize;
            float foodY = y + rng.Gaussian() * lumpSize;
            if (x * x + y * y < radius * radius)
                SpawnFood(foodX, foodY, foodSize, coating);
        }
    }

    void UpdateCamera()
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
        //zoomUI.text = $"x{Mathf.Floor((1.25f / zoom) * 50)}";

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
}
