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
    // Start is called before the first frame update
    void Start()
    {
        CreateCell(0, 0);
        //KillCell(cells[0]);
    }

    public void update()
    {
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
        foreach (Cell cell in cells)
        {
            cell.fixedupdate();
        }
    }

    void CreateCell(float x, float y)
    {
        GameObject cellObject = Instantiate(defaultCell, new Vector3(x, y, 0), new Quaternion());
        Cell cell = cellObject.AddComponent<Cell>();
        cell.radius /= radius;
        cell.substrate = this;
        cells.Add(cell);
    }

    void KillCell(Cell cell)
    {
        cell.Kill();
    }
}
