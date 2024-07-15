using UnityEngine;

public class SubstrateElement : MonoBehaviour
{
    public int gridID;
    public Substrate substrate;

    public Vector2 position;

    public bool UpdateGrid()
    {
        int newGridID = ToGridID();
        if (newGridID != gridID)
        {
            substrate.RemoveFromGridCell(gridID, this);
            gridID = newGridID;
            substrate.AddToGridCell(gridID, this);
            return true;
        }
        return false;
    }

    public int ToGridID()
    {
        int col = Mathf.FloorToInt(position.x / substrate.interactionCellWidth);
        int row = Mathf.FloorToInt(position.y / substrate.interactionCellWidth);
        return col + row * substrate.interactionGridLength;
    }
}
