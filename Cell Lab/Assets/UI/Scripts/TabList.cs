using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabList : MonoBehaviour
{
    public GameObject tabs;
    public List<GameObject> tabList = new List<GameObject>();
    public string currentTab;
    public float glideSpeed;
    GameObject currentTabGameObject;
    void Start()
    {
        RectTransform tabsRect = tabs.GetComponent<RectTransform>();
        int children = tabs.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            Transform child = tabs.transform.GetChild(i);
            RectTransform rect = child.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(tabsRect.sizeDelta.x * i, 0);
            tabList.Add(child.gameObject);
        }
    }

    void Update()
    {
        if (currentTab != "")
        {
            RectTransform listRect = tabs.GetComponent<RectTransform>();
            listRect.anchoredPosition = Vector2.Lerp(listRect.anchoredPosition, new Vector2(-currentTabGameObject.GetComponent<RectTransform>().anchoredPosition.x, listRect.anchoredPosition.y), Time.deltaTime * glideSpeed);
        }
    }

    public void ResetPosition()
    {
        if (currentTab != "")
        {
            RectTransform listRect = tabs.GetComponent<RectTransform>();
            listRect.anchoredPosition = new Vector2(-currentTabGameObject.GetComponent<RectTransform>().anchoredPosition.x, listRect.anchoredPosition.y);
        }
    }

    public void SetTab(string tab)
    {
        foreach (GameObject obj in tabList)
        {
            if (obj.name == tab)
            {
                currentTab = tab;
                currentTabGameObject = obj;
            }
        }
    }
}
