using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [field: SerializeField]
    public GameObject _dropdownPrefab { get; private set; }
    static GameObject dropdownPrefab;

    [field: SerializeField]
    public GameObject _horizontalListPrefab { get; private set; }
    static GameObject horizontalListPrefab;

    [field: SerializeField]
    public GameObject _verticalListPrefab { get; private set; }
    static GameObject verticalListPrefab;

    [field: SerializeField]
    public GameObject _tabListPrefab { get; private set; }
    static GameObject tabListPrefab;

    [field: SerializeField]
    public GameObject _buttonActionPrefab { get; private set; }
    static GameObject buttonActionPrefab;

    public static Canvas mainCanvas;
    public static Canvas overlayCanvas;
    static GameObject topBarList;
    static UI instance;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            dropdownPrefab = _dropdownPrefab;
            horizontalListPrefab = _horizontalListPrefab;
            verticalListPrefab = _verticalListPrefab;
            tabListPrefab = _tabListPrefab;
            buttonActionPrefab = _buttonActionPrefab;
            mainCanvas = GameObject.Find("UI").GetComponent<Canvas>();
            overlayCanvas = GameObject.Find("Overlay").GetComponent<Canvas>();
            if (SceneManager.GetActiveScene().name == "Plate")
            {
                topBarList = GameObject.Find("TopBar").transform.GetChild(1).gameObject;
            }
        }
    }

    /// <summary>
    /// Creates a Dropdown
    /// </summary>
    /// <param name="parent">The parent of the Dropdown</param>
    /// <returns>The Dropdown created</returns>
    public static Dropdown CreateDropdown(Transform parent) {
        return Instantiate(dropdownPrefab, parent).GetComponent<Dropdown>();
    }

    /// <summary>
    /// Creates a HorizontalList
    /// </summary>
    /// <param name="parent">The parent of the HorizontalList</param>
    /// <param name="position">The position of the HorizontalList</param>
    /// <param name="scale">The scale of the HorizontalList</param>
    /// <param name="optionOffset">Distance in pixels between the options</param>
    /// <param name="selectionOffset">How much bigger the selection is than the selected option in pixels</param>
    /// <param name="glideSpeed">How fast the options move when selected</param>
    /// <param name="isStatic">If true then the options move around the bar, if false the bar moves around the options</param>
    /// <returns>The HorizontalList created</returns>
    public static HorizontalList CreateHorizontalList(Transform parent, Vector2? position = null, Vector2? scale = null, float optionOffset = 250, float selectionOffset = 80, float glideSpeed = 10, bool isStatic = true)
    {
        // Set default values
        if (position == null)
        {
            position = Vector2.zero;
        }
        if (scale == null)
        {
            scale = new Vector2(1920, 43);
        }

        // Create the HorizontalList
        HorizontalList horizontalList = Instantiate(horizontalListPrefab, parent).GetComponent<HorizontalList>();
        RectTransform rectTransform = horizontalList.GetComponent<RectTransform>();
        rectTransform.localPosition = (Vector2)position;
        rectTransform.sizeDelta = (Vector2)scale;
        horizontalList.optionOffset = optionOffset;
        horizontalList.selectionOffset = selectionOffset;
        horizontalList.glideSpeed = glideSpeed;
        horizontalList.staticBar = isStatic;

        return horizontalList;
    }

    /// <summary>
    /// Creates a VerticalList
    /// </summary>
    /// <param name="parent">The parent of the VerticalList</param>
    /// <param name="position">The position of the VerticalList</param>
    /// <param name="scale">The scale of the VerticalList</param>
    /// <returns>The VerticalList created</returns>
    public static VerticalList CreateVerticalList(Transform parent, Vector2? position = null, Vector2? scale = null)
    {
        // Set default values
        if (position == null)
        {
            position = Vector2.zero;
        }
        if (scale == null)
        {
            scale = new Vector2(1920, 43);
        }

        VerticalList verticalList = Instantiate(verticalListPrefab, parent).GetComponent<VerticalList>();
        RectTransform rectTransform = verticalList.GetComponent<RectTransform>();
        rectTransform.localPosition = (Vector2)position;
        rectTransform.sizeDelta = (Vector2)scale;

        return verticalList;
    }

    public static TabList CreateTabList(Transform parent, Vector2? position = null, Vector2? scale = null, float glideSpeed = 8)
    {
        // Set default values
        if (position == null)
        {
            position = Vector2.zero;
        }
        if (scale == null)
        {
            scale = new Vector2(1920, 43);
        }

        TabList tabList = Instantiate(tabListPrefab, parent).GetComponent<TabList>();
        RectTransform rectTransform = tabList.GetComponent<RectTransform>();
        rectTransform.localPosition = (Vector2)position;
        rectTransform.sizeDelta = (Vector2)scale;
        tabList.glideSpeed = glideSpeed;

        return tabList;
    }

    /// <summary>
    /// Loads a Sprite from the Resources folder
    /// </summary>
    /// <param name="resourcePath">The path to the Sprite</param>
    /// <returns>The Sprite loaded</returns>
    public static Sprite LoadSprite(string resourcePath)
    {
        return Resources.Load<Sprite>(resourcePath);
    }

    /// <summary>
    /// Creates a ButtonAction
    /// </summary>
    /// <param name="backgroundImage">The background image of the ButtonAction</param>
    /// <param name="foregroundImage">The foreground image of the ButtonAction</param>
    /// <returns>The ButtonAction created</returns>
    public static ButtonAction CreateButtonAction(Sprite backgroundImage, Sprite foregroundImage)
    {
        ButtonAction buttonAction = Instantiate(buttonActionPrefab, topBarList.transform).GetComponent<ButtonAction>();
        Image background = buttonAction.GetComponent<Image>();
        Image foreground = buttonAction.transform.GetChild(0).GetComponent<Image>();
        background.sprite = backgroundImage;
        foreground.sprite = foregroundImage;

        return buttonAction;
    }
}
