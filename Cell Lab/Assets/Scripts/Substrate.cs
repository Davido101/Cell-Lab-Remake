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
        Time.timeScale = temperature;
        SpawnCell(0.5f, 0.5f);
        SpawnCell(-0.5f, -0.5f);
    }

    public void update()
    {
        Time.timeScale = temperature;
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

        float deltaT = Time.fixedDeltaTime;
        foreach (Cell cell in cells)
        {
            cell.fixedupdate(deltaT);
        }
    }

    void SpawnCell(float x, float y)
    {
        GameObject cellObject = Instantiate(defaultCell, new Vector3(x, y, 0), new Quaternion());
        Cell cell = cellObject.AddComponent<Cell>();
        cell.position = new Vector2(x * radius, y * radius);
        //cell.radius /= radius;
        cell.substrate = this;
        cells.Add(cell);
    }

    void KillCell(Cell cell)
    {
        cell.Kill();
    }
}
