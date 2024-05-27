using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public HorizontalList horizontalList;
    public List<string> tabs = new List<string>() { "Challenges", "Experiments", "Gene Bank", "Settings", "About" };
    void Start()
    {
        horizontalList.AddOptions(tabs);
    }

    void Update()
    {
        
    }
}
