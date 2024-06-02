using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class VerticalList : MonoBehaviour
{
    public enum ElementType
    {
        Heading,
        DropdownProperty,
        ToggleProperty,
        Element
    }

    [Header("Prefabs")]
    public GameObject elementPrefab;
    public GameObject headingPrefab;

    [Header("Properties")]
    public GameObject content;
    public Action<string> clickedCallback;
    Dictionary<GameObject, string> elements = new Dictionary<GameObject, string>();
    

    // AddElement for ElementType.Heading
    public void AddElement(ElementType type, string id, string heading)
    {
        GameObject element = Instantiate(headingPrefab, content.transform);
        ClickDetector clickDetector = element.GetComponent<ClickDetector>();
        clickDetector.returnText = false;
        clickDetector.callback = ElementClicked;
        element.transform.GetChild(0).GetComponent<TMP_Text>().text = heading;
        elements.Add(element, id);
    }

    // AddElement for ElementType.DropdownProperty
    public void AddElement(ElementType type, string id, string heading, string description, string defaultValue)
    {

    }

    // AddElement for ElementType.ToggleProperty
    public void AddElement(ElementType type, string id, string heading, string description, bool defaultValue)
    {

    }

    // AddElement for ElementType.Element
    public void AddElement(ElementType type, string id, string heading, string description)
    {
        GameObject element = Instantiate(elementPrefab, content.transform);
        ClickDetector clickDetector = element.GetComponent<ClickDetector>();
        clickDetector.returnText = false;
        clickDetector.callback = ElementClicked;
        element.transform.GetChild(0).GetComponent<TMP_Text>().text = heading;
        element.transform.GetChild(1).GetComponent<TMP_Text>().text = description;
        elements.Add(element, id);
    }

    void ElementClicked(GameObject obj, string name)
    {
        if (name != "Element")
            obj = obj.transform.parent.gameObject;
        if (elements.ContainsKey(obj))
            clickedCallback.Invoke(elements[obj]);
    }
}
