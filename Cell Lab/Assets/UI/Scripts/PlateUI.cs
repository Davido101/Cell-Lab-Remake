using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using SFB;

public class PlateUI : MonoBehaviour
{
    public ButtonAction cellSpawn;
    public ButtonAction plate;
    public Substrate substrate;
    public Dropdown cellDropdown;
    public Dropdown plateDropdown;
    public GameObject uiCanvas;
    public AudioClip buttonClick;
    public AudioClip buttonClick2;
    public AudioClip sterilize;
    List<string> cellTypeNames = new List<string>();
    List<Material> cellTypeIcons = new List<Material>();
    EventSystem eventSystem;

    void Start()
    {
        eventSystem = this.GetComponent<EventSystem>();

        // Load Audio
        buttonClick = Audio.LoadAudio("Audio/button");
        buttonClick2 = Audio.LoadAudio("Audio/button2");
        sterilize = Audio.LoadAudio("Audio/sterilize");

        // Create Select Cell Type ButtonAction
        cellSpawn = UI.CreateButtonAction(UI.LoadSprite("Sprites/buttonBackground45"), UI.LoadSprite("Sprites/cell"));

        // Create Dropdown
        cellDropdown = UI.CreateDropdown(UI.overlayCanvas.transform);
        cellSpawn.triggered += () => { cellDropdown.Toggle(); Audio.PlayAudio(buttonClick); };
        cellDropdown.closeOnSelect = true;
        cellDropdown.SetTitle("Select Cell Type");
        cellDropdown.clickAudio = buttonClick;

        foreach (CellInfo cellInfo in substrate.cellTypes)
        {
            cellTypeNames.Add(cellInfo.cellType.ToString());
            cellTypeIcons.Add(cellInfo.cellShader);
        }

        cellDropdown.ClearOptions();
        cellDropdown.AddOptions(cellTypeNames, cellTypeIcons);

        // Create Plate ButtonAction
        plate = UI.CreateButtonAction(UI.LoadSprite("Sprites/buttonBackground45"), UI.LoadSprite("Sprites/plate"));

        plateDropdown = UI.CreateDropdown(UI.overlayCanvas.transform);
        plate.triggered += () => { plateDropdown.Toggle(); Audio.PlayAudio(buttonClick); };
        plateDropdown.closeOnSelect = true;
        plateDropdown.SetTitle("Select action");
        plateDropdown.AddOption("Save plate...", UI.LoadSprite("Sprites/plate_load"));
        plateDropdown.AddOption("Sterilize plate", UI.LoadSprite("Sprites/plate_clear"));
        plateDropdown.callback = ActionClicked;
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

    public void ActionClicked(string action)
    {
        switch (action)
        {
            case "Save plate...":
                ExtensionFilter[] extensions = new[]
                {
                    new ExtensionFilter("substrate files", "substrate"),
                };
                string path = StandaloneFileBrowser.SaveFilePanel("Save substrate", "", "", extensions);
                substrate.Save(path);
                break;
            case "Sterilize plate":
                Audio.PlayAudio(sterilize);
                substrate.Clear();
                break;
        }
    }

    public void ExitPlate()
    {
        Audio.PlayAudio(buttonClick2, true);
        SceneManager.LoadScene(0);
    }
}
