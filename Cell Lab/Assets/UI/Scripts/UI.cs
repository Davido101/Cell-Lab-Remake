using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI : MonoBehaviour
{
    public TMP_Dropdown cellSpawn;
    public Substrate substrate;
    List<string> cellTypesStr = new List<string>();
    EventSystem eventSystem;

    void Start()
    {
        eventSystem = this.GetComponent<EventSystem>();

        foreach (Type cellType in substrate.cellTypes)
        {
            cellTypesStr.Add(cellType.ToString());
        }

        cellSpawn.ClearOptions();
        cellSpawn.AddOptions(cellTypesStr);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !eventSystem.IsPointerOverGameObject())
        {
            substrate.SpawnCellEvent();
        }
    }

    public void OnValueChanged()
    {
        string value = cellTypesStr[cellSpawn.value];

        foreach (Type cellType in substrate.cellTypes)
        {
            if (cellType.ToString() == value)
            {
                substrate.cellType = cellType;
            }
        }
    }
}
