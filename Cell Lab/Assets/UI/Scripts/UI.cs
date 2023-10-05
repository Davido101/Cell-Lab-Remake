using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI : MonoBehaviour
{
    public Button cellSpawn;
    public Substrate substrate;
    public GameObject dropdownPrefab;
    public Dropdown dropdown;
    public GameObject uiCanvas;
    List<string> cellTypesStr = new List<string>();
    List<Sprite> cellTypesIcon = new List<Sprite>();
    EventSystem eventSystem;

    void Start()
    {
        eventSystem = this.GetComponent<EventSystem>();
        dropdown = Instantiate(dropdownPrefab, uiCanvas.transform).GetComponent<Dropdown>();
        dropdown.trigger = cellSpawn;
        dropdown.closeOnSelect = true;
        dropdown.useSvgs = true;
        dropdown.SetTitle("Select Cell Type");

        foreach (CellInfo cellInfo in substrate.cellTypes)
        {
            cellTypesStr.Add(cellInfo.cellType.ToString());
            cellTypesIcon.Add(cellInfo.cellSprite);
        }

        Debug.Log(cellTypesStr.Count);
        Debug.Log(cellTypesIcon.Count);

        dropdown.ClearOptions();
        dropdown.AddOptions(cellTypesStr, cellTypesIcon);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !eventSystem.IsPointerOverGameObject())
        {
            SetCellType();
            substrate.SpawnCellEvent();
        }
    }

    public void SetCellType()
    {
        foreach (CellInfo cellInfo in substrate.cellTypes)
        {
            if (cellInfo.cellType.ToString() == dropdown.selectedOption)
            {
                substrate.cellType = cellInfo.cellType;
            }
        }
    }
}
