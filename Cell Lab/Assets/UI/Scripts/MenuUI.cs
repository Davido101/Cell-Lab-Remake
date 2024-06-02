using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [Header("Properties")]
    public HorizontalList horizontalList;
    public TabList tabList;
    public List<string> tabs = new List<string>() { "Gene Bank", "Experiments", "Challenges", "Settings", "About" };

    // To Do:
    // - Add colors to title and description
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
        { "general", new Section("General") }
    };
    

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

    public struct Element
    {
        public Element(object element, VerticalList.ElementType elementType)
        {
            this.element = element;
            this.elementType = elementType;
        }

        object element;
        VerticalList.ElementType elementType;
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

    void Start()
    {
        horizontalList.AddOptions(tabs, 2);
        tabList.SetTab("Challenges");
        
        // Initialize Challenges tab
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
                    // TO DO: initalize every element in section
                }
            }
        }
    }

    void Update()
    {
        tabList.SetTab(horizontalList.selectedOption);
    }

    void ChallengeSelected(string id)
    {
        // Change to Challenge Scene or load in Game Scene
        // with the Challenge Save from the game's source code
        Debug.Log("Challenge Selected: " + id);
    }

    void SettingSelected(string id)
    {
        Debug.Log("Setting Selected: " + id);
    }
}
