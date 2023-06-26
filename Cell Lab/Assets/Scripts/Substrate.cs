using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Substrate : MonoBehaviour
{
    const float maxScale = 1.25f;
    public float zoomSpeed = 3.5f;
    public float zoomSnap = 0.99f;
    public float moveSpeed = 3.5f;
    Vector3 originalPos;
    public List<Cell> cells = new List<Cell>();
    public GameObject defaultCell;
    public float radius = 1;
    public float zoom = 1;
    public float temperature = 1;
    public Camera camera;

    void Start()
    {
        this.transform.localScale = new Vector3(radius, radius, 1);
        camera = (Camera)GameObject.FindObjectOfType(typeof(Camera));
        zoom = maxScale * radius;
        camera.orthographicSize = zoom;
        AdjustSpeed();
        SpawnCell(0.5f, 0.5f, new Color(0.7019f, 1f, 0.2235f));
        SpawnCell(-0.5f, -0.5f, new Color(0.7019f, 1f, 0.2235f));
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
                if (i != j && !cells[i].dead && !cells[j].dead) cells[i].react(cells[j]);
            }
        }

        float deltaT = temperature / Time.timeScale / 50;
        foreach (Cell cell in cells)
        {
            cell.fixedupdate(deltaT);
        }
    }

    void AdjustSpeed()
    {
        Time.timeScale = Mathf.Clamp(temperature, 1, 100);
    }

    void SpawnCell(float x, float y, Color color)
    {
        GameObject cellObject = Instantiate(defaultCell, new Vector3(x, y, 0), new Quaternion());
        Cell cell = cellObject.AddComponent<Cell>();
        cell.position = new Vector2(x * radius, y * radius);
        cell.color = color;
        cell.substrate = this;
        cells.Add(cell);
    }

    void UpdateCamera()
    {
        float maxZoom = maxScale * radius;

        // Zoom
        float scrolling = Input.GetAxis("Mouse ScrollWheel");
        if (scrolling != 0)
        {
            float zooming = Mathf.Pow(zoomSpeed, -scrolling);
            zoom *= zooming;
            zoom = Mathf.Clamp(zoom, 0.0001f, maxZoom);
        }
        float zoomLerping = 1 - Mathf.Pow(1 - zoomSnap, Time.deltaTime);
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, zoom, zoomLerping);

        // Move
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
