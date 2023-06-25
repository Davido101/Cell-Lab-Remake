using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Substrate : MonoBehaviour
{
    public List<Cell> cells = new List<Cell>();
    public GameObject defaultCell;
    public float radius = 1;
    // Start is called before the first frame update
    void Start()
    {
        CreateCell(0, -2);
        //KillCell(cells[0]);
    }

    public void update()
    {
        foreach (Cell cell in cells)
        {
            cell.update();
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
        cellObject.transform.localScale = new Vector3(cellObject.transform.localScale.x / radius, cellObject.transform.localScale.y / radius, 0);
        Cell cell = cellObject.GetComponent<Cell>();
        cell.substrate = this;
        cell.position = new Vector2(x, y);
        cells.Add(cell);
    }

    void KillCell(Cell cell)
    {
        cell.Kill();
    }
}
