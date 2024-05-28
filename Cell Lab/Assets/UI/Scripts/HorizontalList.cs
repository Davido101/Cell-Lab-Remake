using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HorizontalList : MonoBehaviour
{
    public GameObject horizontalList;
    public GameObject horizontalListElement;
    public float optionOffset;
    public float selectionOffset;
    public float glideSpeed;
    public GameObject list;
    GameObject selection;
    RectTransform rectSelection;
    List<GameObject> options = new List<GameObject>();
    public string selectedOption;
    GameObject selectedOptionGameObject;

    void Start()
    {
        selection = horizontalList.transform.GetChild(2).gameObject;
        rectSelection = selection.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (options.Count == 0)
        {
            if (selection.activeSelf)
                selection.SetActive(false);
        }
        else
        {
            if (!selection.activeSelf)
                selection.SetActive(true);
            TMP_Text selectedText = selectedOptionGameObject.GetComponent<TMP_Text>();
            Vector2 optionSize = UITools.GetRenderedValues(selectedText, selectedText.text);
            if (rectSelection.sizeDelta.x < 0)
                rectSelection.sizeDelta = new Vector2(0, rectSelection.sizeDelta.y);
            rectSelection.sizeDelta = Vector2.Lerp(rectSelection.sizeDelta, new Vector2(optionSize.x + selectionOffset, selection.transform.localScale.y), Time.deltaTime * glideSpeed);
        
            RectTransform listRect = list.GetComponent<RectTransform>();
            listRect.anchoredPosition = Vector2.Lerp(listRect.anchoredPosition, new Vector2(-selectedOptionGameObject.GetComponent<RectTransform>().anchoredPosition.x, listRect.anchoredPosition.y), Time.deltaTime * glideSpeed);

            foreach (GameObject option in options)
            {
                RectTransform rect = option.GetComponent<RectTransform>();
                TMP_Text text = option.GetComponent<TMP_Text>();
                rect.sizeDelta = new Vector2(UITools.GetRenderedValues(text, text.text).x, rect.sizeDelta.y);
            }
        }
    }

    public void AddOption(string option)
    {
        GameObject optionObject = Instantiate(horizontalListElement, list.transform);
        optionObject.GetComponent<ClickDetector>().callback = OptionClicked;
        optionObject.GetComponent<TMP_Text>().text = option;
        if (options.Count == 0)
        {
            selectedOption = option;
            selectedOptionGameObject = optionObject;
        }
        else
        {
            TMP_Text lastOption = options.Last().GetComponent<TMP_Text>();
            RectTransform lastRect = lastOption.GetComponent<RectTransform>();
            Vector2 size = UITools.GetRenderedValues(lastOption, lastOption.text);
            RectTransform optionRect = optionObject.GetComponent<RectTransform>();
            optionRect.anchoredPosition = new Vector2(lastRect.anchoredPosition.x + size.x / 2 + optionOffset, 1.9196f);
        }
        options.Add(optionObject);
    }

    public void AddOptions(List<string> optionList, int defaultOption = 0)
    {
        for (int i = 0; i < optionList.Count; i++)
        {
            AddOption(optionList[i]);
        }
        selectedOptionGameObject = options[defaultOption];
        selectedOption = selectedOptionGameObject.GetComponent<TMP_Text>().text;
    }

    public void ClearOptions()
    {
        foreach (GameObject option in options)
        {
            Destroy(option);
        }
        options.Clear();
        selectedOption = null;
        selectedOptionGameObject = null;
    }

    void OptionClicked(GameObject optionObject, string option)
    {
        selectedOption = option;
        selectedOptionGameObject = optionObject;
    }
}
