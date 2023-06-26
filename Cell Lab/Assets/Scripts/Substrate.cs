using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Substrate : MonoBehaviour
{
    public List<Cell> cells = new List<Cell>();
    public GameObject defaultCell;
    public float radius = 1;
    public float temperature = 1;
    public Camera camera;

    void Start()
    {
        this.transform.localScale = new Vector3(radius, radius, 1);
        camera = (Camera)GameObject.FindObjectOfType(typeof(Camera));
        camera.orthographicSize = 1.25f * radius;
        adjustSpeed();
        SpawnCell(0.5f, 0.5f, new Color(0.7019f, 1f, 0.2235f));
        SpawnCell(-0.5f, -0.5f, new Color(0.7019f, 1f, 0.2235f));
    }

    public void update() 
    { 
        adjustSpeed();
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

    void adjustSpeed()
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

    void KillCell(Cell cell)
    {
        cell.Kill();
    }
}
