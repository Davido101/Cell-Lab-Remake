using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleProperty : MonoBehaviour
{
    public bool toggleState;
    Image activation;

    public void Awake()
    {
        activation = transform.GetChild(2).GetChild(0).GetComponent<Image>();
    }

    /// <summary>
    /// Sets the state of the ToggleProperty
    /// </summary>
    /// <param name="state">The state to be set</param>
    public void SetState(bool state)
    {
        toggleState = state;
        if (toggleState)
            activation.color = new Color(1, 0.69f, 0, 1);
        else
            activation.color = new Color(0, 0, 0, 1);
    }

    /// <summary>
    /// Toggles the state of the ToggleProperty
    /// </summary>
    /// <returns>Returns the state toggled to</returns>
    public bool Toggle()
    {
        SetState(!toggleState);
        return toggleState;
    }
}
