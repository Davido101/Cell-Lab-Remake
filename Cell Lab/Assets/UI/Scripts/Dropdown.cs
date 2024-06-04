using System;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class Dropdown : MonoBehaviour
{
    public GameObject dropdown;
    public GameObject content;
    public GameObject option;
    public GameObject svgOption;
    public TMP_Text titleObject;
    public Button trigger;
    public List<GameObject> options = new List<GameObject>();
    public string selectedOption;
    public int value;
    public int optionY = 0;
    public int optionCount = 0;
    public bool active = false;
    public bool closeOnSelect = false;
    public bool useSvgs = false;
    public Action<string> callback;
    
    void Start()
    {
        if (trigger != null)
        {
            trigger.onClick.AddListener(() => Toggle());
        }
        dropdown = transform.gameObject;
    }

    public void Disable()
    {
        transform.GetComponent<Image>().color -= new Color(0, 0, 0, 255);
        transform.GetChild(0).gameObject.SetActive(false);
        titleObject.gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        active = false;
        dropdown.SetActive(false);
    }

    public void Enable()
    {
        transform.GetComponent<Image>().color += new Color(0, 0, 0, 255);
        transform.GetChild(0).gameObject.SetActive(true);
        titleObject.gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
        active = true;
        dropdown.SetActive(true);
    }

    public void Toggle()
    {
        if (active)
        {
            Disable();
        }
        else
        {
            Enable();
        }
    }

    public void SetTitle(string title)
    {
        titleObject.text = title;
    }

    public void ResizeContent()
    {
        int height = optionCount * 100;
        if (height <= 500)
        {
            RectTransform rectTransform = content.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(0, 0, 0);
            rectTransform.sizeDelta = new Vector3(450, 500, 0);
        }
        else
        {
            RectTransform rectTransform = content.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(0, -(height / 2), 0);
            rectTransform.sizeDelta = new Vector3(450, height, 0);
        }
    }

    public void AddOption(string optionName = "Option Name", Sprite icon = null)
    {
        GameObject optionObject;
        if (useSvgs)
        {
            optionObject = Instantiate(svgOption, content.transform);
            Transform image = optionObject.transform.GetChild(2);
            image.gameObject.GetComponent<SVGImage>().sprite = icon;
            RectTransform rectTransform = image.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector3(icon.rect.size.x / 7, icon.rect.size.y / 7);
        }
        else
        {
            optionObject = Instantiate(option, content.transform);
            Transform image = optionObject.transform.GetChild(2);
            image.gameObject.GetComponent<Image>().sprite = icon;
            RectTransform rectTransform = image.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector3(icon.rect.size.x / 7, icon.rect.size.y / 7);
        }
        optionObject.transform.localPosition -= new Vector3(0, optionY, 0);
        optionObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = optionName;
        options.Add(optionObject);
        int index = options.Count - 1;
        optionObject.GetComponent<Button>().onClick.AddListener(() => ButtonClicked(index));
        optionY += 100;
        optionCount++;
        ResizeContent();
    }

    public void AddOptions(List<string> names, List<Sprite> icons)
    {
        for (int i = 0; i < names.Count; i++)
        {
            AddOption(names[i], icons[i]);
        }
    }

    public void AddOptions(List<string> names)
    {
        for (int i = 0; i < names.Count; i++)
        {
            AddOption(names[i]);
        }
    }

    public void ClearOptions()
    {
        foreach (GameObject obj in options)
        {
            Destroy(obj);
        }
        options.Clear();
        optionCount = 0;
        optionY = 0;
        ResizeContent();
    }

    void ButtonClicked(int buttonIndex)
    {
        selectedOption = options[buttonIndex].transform.GetChild(1).GetComponent<TMP_Text>().text;
        value = buttonIndex;
        if (closeOnSelect)
            Disable();
        callback.Invoke(selectedOption);
    }
}
