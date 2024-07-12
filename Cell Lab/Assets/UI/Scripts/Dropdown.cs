using System;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class Dropdown : MonoBehaviour
{
    GameObject dropdown;
    RectTransform dropdownRect;
    GameObject content;
    RectTransform contentRect;
    public GameObject option;
    public GameObject svgOption;
    TMP_Text titleObject;
    public List<GameObject> options = new List<GameObject>();
    public string selectedOption;
    public int value;
    int optionY = 0;
    public int optionCount = 0;
    bool active = false;
    public bool closeOnSelect = false;
    public bool useSvgs = false;
    public Action<string> callback;
    public AudioClip clickAudio;

    private void Awake()
    {
        dropdown = gameObject;
        titleObject = transform.GetChild(3).GetComponent<TMP_Text>();
        content = transform.GetChild(4).GetChild(0).gameObject;
        dropdownRect = dropdown.GetComponent<RectTransform>();
        contentRect = content.GetComponent<RectTransform>();
        Disable();
    }

    public void Disable()
    {
        Darken.Disable();
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        titleObject.gameObject.SetActive(false);
        transform.GetChild(4).gameObject.SetActive(false);
        active = false;
        dropdown.SetActive(false);
    }

    public void Enable()
    {
        Darken.Enable();
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
        titleObject.gameObject.SetActive(true);
        transform.GetChild(4).gameObject.SetActive(true);
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
        dropdownRect.sizeDelta = new Vector2(dropdownRect.sizeDelta.x, Math.Clamp(height, 0, 800) + 104);
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, height);
    }

    public void AddOption(string optionName = "Option Name", Sprite icon = null)
    {
        GameObject optionObject;
        if (useSvgs)
        {
            optionObject = Instantiate(svgOption, content.transform);
            Transform image = optionObject.transform.GetChild(2);
            if (icon)
            {
                image.gameObject.GetComponent<SVGImage>().sprite = icon;
                RectTransform rectTransform = image.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(icon.rect.size.x / 7, icon.rect.size.y / 7);
            }
            else
            {
                RectTransform rectTransform = optionObject.transform.GetChild(1).GetComponent<RectTransform>();
                rectTransform.localPosition = new Vector2(-37.5f, rectTransform.localPosition.y);
                image.gameObject.SetActive(false);
            }
        }
        else
        {
            optionObject = Instantiate(option, content.transform);
            Transform image = optionObject.transform.GetChild(2);
            if (icon)
            {
                image.gameObject.GetComponent<Image>().sprite = icon;
                RectTransform rectTransform = image.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(icon.rect.size.x / 7, icon.rect.size.y / 7);
            }
            else
            {
                RectTransform rectTransform = optionObject.transform.GetChild(1).GetComponent<RectTransform>();
                rectTransform.localPosition = new Vector2(-37.5f, rectTransform.localPosition.y);
                image.gameObject.SetActive(false);
            }
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

    public void AddOption(string optionName = "Option Name", Material shader = null)
    {
        GameObject optionObject = Instantiate(option, content.transform);
        Transform image = optionObject.transform.GetChild(2);
        if (shader)
        {
            image.gameObject.GetComponent<Image>().material = shader;
            RectTransform rectTransform = image.GetComponent<RectTransform>();
            float scaleConst = shader.GetFloat("scaleConst") / 200 * rectTransform.sizeDelta.x;
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 1000);
            rectTransform.sizeDelta = new Vector2(scaleConst, scaleConst);
        }
        else
        {
            RectTransform rectTransform = optionObject.transform.GetChild(1).GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector2(-37.5f, rectTransform.localPosition.y);
            image.gameObject.SetActive(false);
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

    public void AddOptions(List<string> names, List<Material> shaders)
    {
        for (int i = 0; i < names.Count; i++)
        {
            AddOption(names[i], shaders[i]);
        }
    }

    public void AddOptions(List<string> names)
    {
        for (int i = 0; i < names.Count; i++)
        {
            AddOption(names[i], (Sprite)null);
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
        if (clickAudio)
            Audio.PlayAudio(clickAudio);
        if (callback != null)
            callback.Invoke(selectedOption);
    }
}
