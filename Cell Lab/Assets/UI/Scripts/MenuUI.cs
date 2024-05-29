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

    void Start()
    {
        horizontalList.AddOptions(tabs, 2);
        tabList.SetTab("Challenges");
        challengesVerticalList.clickedCallback = ChallengeSelected;
        foreach (KeyValuePair<string, Challenge> challenge in challenges)
        {
            challengesVerticalList.AddElement(VerticalList.ElementType.Element, challenge.Key, challenge.Value.heading, challenge.Value.description);
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
        Debug.Log("Selected: " + id);
    }
}
