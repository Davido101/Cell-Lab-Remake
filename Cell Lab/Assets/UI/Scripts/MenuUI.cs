using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public HorizontalList horizontalList;
    public List<string> tabs = new List<string>() { "Gene Bank", "Experiments", "Challenges", "Settings", "About" };
    void Start()
    {
        horizontalList.AddOptions(tabs, 2);
    }

    void Update()
    {
        
    }
}
