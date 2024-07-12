using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TabList : MonoBehaviour
{
    GameObject tabs;

    [field: SerializeField]
    public GameObject tabPrefab { get; private set; }

    List<GameObject> tabList = new List<GameObject>();
    public string currentTab;
    public float glideSpeed;
    GameObject currentTabGameObject;

    private void Awake()
    {
        tabs = transform.GetChild(0).gameObject;
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

    public void AddTab(string tab)
    {
        GameObject tabObject = Instantiate(tabPrefab, tabs.transform);
        tabObject.name = tab;
        if (tabList.Count == 0)
        {
            currentTab = tab;
            currentTabGameObject = tabObject;
        }
        else
        {
            RectTransform lastTabRect = tabList.Last().GetComponent<RectTransform>();
            RectTransform tabRect = tabObject.GetComponent<RectTransform>();
            tabRect.anchoredPosition = lastTabRect.anchoredPosition + new Vector2(1920, 0);
        }
        tabList.Add(tabObject);
    }

    public void AddTabs(List<string> tabs)
    {
        foreach (string tab in tabs)
        {
            AddTab(tab);
        }
    }

    public void AddTabs(List<string> tabs, string defaultTab)
    {
        foreach (string tab in tabs)
        {
            AddTab(tab);
        }
        SetTab(defaultTab);
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

    /// <summary>
    /// Gets a tab's GameObject from its name
    /// </summary>
    /// <param name="tab">The name of the tab to get</param>
    /// <returns>The tab's GameObject or null if not found</returns>
    public GameObject? GetTab(string tab)
    {
        foreach (GameObject obj in tabList)
        {
            if (obj.name == tab)
            {
                return obj;
            }
        }
        return null;
    }
}
