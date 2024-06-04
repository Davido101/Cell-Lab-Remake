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
    public GameObject togglePrefab;

    [Header("Properties")]
    public GameObject content;
    public Action<string, ClickedEventData> clickedCallback;
    Dictionary<GameObject, string> elements = new Dictionary<GameObject, string>();
    
    public struct ClickedEventData
    {
        public ClickedEventData(string value, GameObject obj)
        {
            this.value = value;
            this.toggle = false;
            this.obj = obj;
            this.type = ElementType.DropdownProperty;
        }

        public ClickedEventData(bool toggle, GameObject obj)
        {
            this.value = "";
            this.toggle = toggle;
            this.obj = obj;
            this.type = ElementType.ToggleProperty;
        }

        public ClickedEventData(ElementType type, GameObject obj)
        {
            this.value = "";
            this.toggle = false;
            this.obj = obj;
            this.type = type;
        }

        public string value;
        public bool toggle;
        public GameObject obj;
        public ElementType type;
    }

    /// <summary>
    /// Gets the element type as a string
    /// </summary>
    /// <param name="type">The element type</param>
    /// <returns>A string that represents the element type</returns>
    public static string GetElementType(ElementType type)
    {
        switch (type)
        {
            case ElementType.Heading:
                return "Heading";
            case ElementType.DropdownProperty:
                return "DropdownProperty";
            case ElementType.ToggleProperty:
                return "ToggleProperty";
            case ElementType.Element:
                return "Element";
            default:
                return "Unknown";
        }
    }

    // AddElement for ElementType.Heading
    public void AddElement(ElementType type, string id, string heading)
    {
        GameObject element = Instantiate(headingPrefab, content.transform);
        ClickDetector clickDetector = element.GetComponent<ClickDetector>();
        clickDetector.returnText = false;
        clickDetector.callback = ElementClicked;
        element.transform.GetChild(0).GetComponent<TMP_Text>().text = heading;
        element.name = id;
        elements.Add(element, id);
    }

    // AddElement for ElementType.DropdownProperty
    public void AddElement(ElementType type, string id, string heading, string description, string defaultValue)
    {
        //GameObject element = Instantiate(togglePrefab, content.transform);
        //ClickDetector clickDetector = element.GetComponent<ClickDetector>();
        //clickDetector.returnText = false;
        //clickDetector.callback = ElementClicked;
        //ToggleProperty toggleProperty = element.GetComponent<ToggleProperty>();
        //toggleProperty.SetState(defaultValue);
        //element.transform.GetChild(0).GetComponent<TMP_Text>().text = heading;
        //element.transform.GetChild(1).GetComponent<TMP_Text>().text = description;
        //element.name = id;
        //elements.Add(element, id);
    }

    // AddElement for ElementType.ToggleProperty
    public void AddElement(ElementType type, string id, string heading, string description, bool defaultValue)
    {
        GameObject element = Instantiate(togglePrefab, content.transform);
        ClickDetector clickDetector = element.GetComponent<ClickDetector>();
        clickDetector.returnText = false;
        clickDetector.callback = ElementClicked;
        ToggleProperty toggleProperty = element.GetComponent<ToggleProperty>();
        toggleProperty.SetState(defaultValue);
        element.transform.GetChild(0).GetComponent<TMP_Text>().text = heading;
        element.transform.GetChild(1).GetComponent<TMP_Text>().text = description;
        element.name = id;
        elements.Add(element, id);
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
        element.name = id;
        elements.Add(element, id);
    }

    void ElementClicked(GameObject obj, string name)
    {
        if (elements.ContainsKey(obj))
        {
            ClickedEventData eventData;
            ToggleProperty toggleProperty = obj.GetComponent<ToggleProperty>();
            if (toggleProperty != null)
            {
                toggleProperty.Toggle();
                eventData = new ClickedEventData(toggleProperty.toggleState, obj);
            }
            else
            {
                Element element = obj.GetComponent<Element>();
                if (element != null)
                {
                    eventData = new ClickedEventData(ElementType.Element, obj);
                }
                else
                {
                    eventData = new ClickedEventData(ElementType.Heading, obj);
                }
            }
            clickedCallback.Invoke(elements[obj], eventData);
        }
    }
}
