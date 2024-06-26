using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlateUI : MonoBehaviour
{
    public ButtonAction cellSpawn;
    public Substrate substrate;
    public Dropdown cellDropdown;
    public GameObject uiCanvas;
    public AudioClip buttonClick;
    public AudioClip buttonClick2;
    List<string> cellTypeNames = new List<string>();
    List<Material> cellTypeIcons = new List<Material>();
    EventSystem eventSystem;

    void Start()
    {
        eventSystem = this.GetComponent<EventSystem>();

        // Load Audio
        buttonClick = Audio.LoadAudio("Audio/button");
        buttonClick2 = Audio.LoadAudio("Audio/button2");

        // Create Select Cell Type ButtonAction
        cellSpawn = UI.CreateButtonAction(UI.LoadSprite("Sprites/buttonBackground45"), UI.LoadSprite("Sprites/cell"));

        // Create Dropdown
        cellDropdown = UI.CreateDropdown(UI.overlayCanvas.transform);
        cellSpawn.triggered += () => { cellDropdown.Toggle(); Audio.PlayAudio(buttonClick); };
        cellDropdown.closeOnSelect = true;
        cellDropdown.SetTitle("Select Cell Type");

        foreach (CellInfo cellInfo in substrate.cellTypes)
        {
            cellTypeNames.Add(cellInfo.cellType.ToString());
            cellTypeIcons.Add(cellInfo.cellShader);
        }

        cellDropdown.ClearOptions();
        cellDropdown.AddOptions(cellTypeNames, cellTypeIcons);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !eventSystem.IsPointerOverGameObject())
        {
            UpdateCellType();
            substrate.SpawnCellEvent();
        }
    }

    public void UpdateCellType()
    {
        foreach (CellInfo cellInfo in substrate.cellTypes)
        {
            if (cellInfo.cellType.ToString() == cellDropdown.selectedOption)
            {
                substrate.cellType = cellInfo.cellType;
            }
        }
    }

    public void ExitPlate()
    {
        Audio.PlayAudio(buttonClick2, true);
        SceneManager.LoadScene(0);
    }
}
