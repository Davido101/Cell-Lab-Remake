using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public HorizontalList horizontalList;
    public TabList tabList;
    public List<string> tabs = new List<string>() { "Gene Bank", "Experiments", "Challenges", "Settings", "About" };
    void Start()
    {
        horizontalList.AddOptions(tabs, 2);
        tabList.SetTab("Challenges");
    }

    void Update()
    {
        tabList.SetTab(horizontalList.selectedOption);
    }
}
