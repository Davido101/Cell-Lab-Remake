using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [Header("Properties")]
    public HorizontalList horizontalList;
    public TabList tabList;
    public List<string> tabs = new List<string>() { "Gene Bank", "Experiments", "Challenges", "Settings", "About" };
    public Audio buttonClick;
    public Audio buttonClick2;

    [Header("Experiments Tab")]
    public VerticalList experimentsVerticalList;

    [Header("Challenge Tab")]
    public VerticalList challengesVerticalList;
    public Dictionary<string, Challenge> challenges = new Dictionary<string, Challenge>()
    {
        { "tutorial1", new Challenge("Tutorial I", "Introduction to the Microscope") },
        { "tutorial2", new Challenge("Tutorial II", "Introduction to the Genome Editor") },
        { "algae", new Challenge("1: Algae", "Challenge not completed. Difficulty: Undergrad\nUnlocks gene Phagocyte and further challenges.") }
    };

    [Header("Settings")]
    public VerticalList settingsVerticalList;
    public Dictionary<string, Section> sections = new Dictionary<string, Section>()
    {
        { "general", new Section("General", new List<Element>
            {
                new Element(VerticalList.ElementType.DropdownProperty, "language", new DropdownProperty("Language", "The language of the UI", new List<string>(){
                    "English"
                })),
                new Element(VerticalList.ElementType.ToggleProperty, "ask_tab_leave", new ToggleProperty("Ask before leaving lab", "Show a prompt before leaving the lab.", true)),
                new Element(VerticalList.ElementType.ToggleProperty, "ask_reload", new ToggleProperty("Ask before sterilize and reload", "Show a prompt before sterilizing and reloading.", false)),
                new Element(VerticalList.ElementType.ToggleProperty, "show_cell_type", new ToggleProperty("Show cell type", "When enabled, this will indicate cell type abbreviations next to mode settings in the genome editor.", false))
            })
        }
    };

    public static Canvas ui;
    public static Canvas overlay;
    

    public struct Challenge
    {
        public Challenge(string title, string desc)
        {
            heading = title;
            description = desc;
        }

        public string heading;
        public string description;
    }

    public struct ToggleProperty
    {
        public ToggleProperty(string heading, string description)
        {
            this.heading = heading;
            this.description = description;
            this.defaultValue = false;
        }

        public ToggleProperty(string heading, string description, bool defaultValue)
        {
            this.heading = heading;
            this.description = description;
            this.defaultValue = defaultValue;
        }

        public string heading;
        public string description;
        public bool defaultValue;
    }

    public struct DropdownProperty
    {
        public DropdownProperty(string heading, string description, List<string> options)
        {
            this.heading = heading;
            this.description = description;
            this.options = options;
            this.defaultValue = this.options[0];
        }

        public DropdownProperty(string heading, string description, string defaultValue, List<string> options)
        {
            this.heading = heading;
            this.description = description;
            this.defaultValue = defaultValue;
            this.options = options;
        }

        public string heading;
        public string description;
        public string defaultValue;
        public List<string> options;
    }

    public struct Element
    {
        public Element(VerticalList.ElementType type, string id, object element)
        {
            this.element = element;
            this.id = id;
            this.type = type;
        }

        public object element;
        public string id;
        public VerticalList.ElementType type;
    }

    public struct Section
    {
        public Section(string heading, List<Element> elements = null)
        {
            this.heading = heading;
            this.elements = elements;
        }

        public string heading;
        public List<Element> elements;
    }

    private void Awake()
    {
        ui = GameObject.Find("UI").GetComponent<Canvas>();
        overlay = GameObject.Find("Overlay").GetComponent<Canvas>();
    }

    void Start()
    {
        horizontalList.clickAudio= buttonClick;
        horizontalList.AddOptions(tabs, 2);
        tabList.SetTab("Challenges");
        StartCoroutine(LateStart());

        // Initialize Experiments tab
        experimentsVerticalList.clickAudio = buttonClick2;
        experimentsVerticalList.clickedCallback = ExperimentSelected;
        experimentsVerticalList.AddElement(VerticalList.ElementType.Element, "new_plate", "<color=#FFA000>New Plate</color>", "Right-click for advanced settings");

        // Initialize Challenges tab
        challengesVerticalList.clickAudio = buttonClick2;
        challengesVerticalList.clickedCallback = ChallengeSelected;
        foreach (KeyValuePair<string, Challenge> challenge in challenges)
        {
            challengesVerticalList.AddElement(VerticalList.ElementType.Element, challenge.Key, challenge.Value.heading, challenge.Value.description);
        }

        // Initialize Settings tab
        settingsVerticalList.clickedCallback = SettingSelected;
        foreach (KeyValuePair<string, Section> section in sections)
        {
            settingsVerticalList.AddElement(VerticalList.ElementType.Heading, section.Key, section.Value.heading);
            if (section.Value.elements != null)
            {
                foreach (Element element in section.Value.elements)
                {
                    if (element.type == VerticalList.ElementType.ToggleProperty)
                    {
                        ToggleProperty toggleProperty = (ToggleProperty)element.element;
                        settingsVerticalList.AddElement(VerticalList.ElementType.ToggleProperty, element.id, toggleProperty.heading, toggleProperty.description, toggleProperty.defaultValue);
                    }
                    else if (element.type == VerticalList.ElementType.DropdownProperty)
                    {
                        DropdownProperty dropdownProperty = (DropdownProperty)element.element;
                        settingsVerticalList.AddElement(VerticalList.ElementType.DropdownProperty, element.id, dropdownProperty.heading, dropdownProperty.description, dropdownProperty.defaultValue, dropdownProperty.options);
                    }
                }
            }
        }
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.01f);
        tabList.ResetPosition();
        horizontalList.ResetPosition();
    }

    void Update()
    {
        tabList.SetTab(horizontalList.selectedOption);
    }

    void ExperimentSelected(string id, VerticalList.ClickedEventData eventData)
    {
        if (id == "new_plate")
        {
            if (buttonClick2.audioSource.isPlaying)
                buttonClick2.SceneUpdate(true);
            SceneManager.LoadScene(1);
        }
    }

    void ChallengeSelected(string id, VerticalList.ClickedEventData eventData)
    {
        // Change to Challenge Scene or load in Game Scene
        // with the Challenge Save from the game's source code
        Debug.Log("Challenge Selected: " + id);
    }

    void SettingSelected(string id, VerticalList.ClickedEventData eventData)
    {
        if (eventData.selectedOption)
        {
            Debug.Log("Dropdown Option Selected: " + eventData.value + ", of Dropdown: " + id);
        }
        else
        {
            Debug.Log("Setting Selected: " + id + ", of type: " + eventData.type);
        }
    }
}
