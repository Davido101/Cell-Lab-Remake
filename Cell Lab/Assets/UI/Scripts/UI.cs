using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public Button cellSpawn;
    public Substrate substrate;
    public GameObject dropdownPrefab;
    public Dropdown dropdown;
    public GameObject uiCanvas;
    List<string> cellTypeNames = new List<string>();
    List<Sprite> cellTypeIcons = new List<Sprite>();
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
            cellTypeNames.Add(cellInfo.cellType.ToString());
            cellTypeIcons.Add(cellInfo.cellSprite);
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(cellTypeNames, cellTypeIcons);
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

    public void ExitPlate()
    {
        SceneManager.LoadScene(0);
    }
}
