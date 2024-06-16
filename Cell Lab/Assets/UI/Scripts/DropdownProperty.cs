using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropdownProperty : MonoBehaviour
{
    public string value;
    public TMP_Text option;
    public GameObject dropdownPrefab;
    public Action<GameObject, string> callback;

    Dropdown dropdown;

    public void Initialize(string title, string defaultValue)
    {
        SetValue(defaultValue);

        dropdown = Instantiate(dropdownPrefab, GetComponentInParent<Canvas>().transform).GetComponent<Dropdown>();
        dropdown.closeOnSelect = true;
        dropdown.SetTitle(title);
        dropdown.ClearOptions();
        dropdown.callback = DropdownClicked;
    }

    /// <summary>
    /// Sets the value of the DropdownProperty
    /// </summary>
    /// <param name="value">The value to be set</param>
    public void SetValue(string value)
    {
        this.value = value;
        option.text = value;
    }

    public void AddOptions(List<string> options)
    {
        dropdown.AddOptions(options);
    }

    public void EnableDropdown()
    {
        dropdown.Enable();
    }

    public void DisableDropdown()
    {
        dropdown.Disable();
    }

    public void ToggleDropdown()
    {
        dropdown.Toggle();
    }

    void DropdownClicked(string option)
    {
        value = option;
        if (callback != null)
            callback.Invoke(gameObject, value);
    }

    private void Update()
    {
        option.text = value;
    }
}