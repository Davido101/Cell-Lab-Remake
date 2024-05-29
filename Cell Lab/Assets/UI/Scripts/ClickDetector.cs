using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDetector : MonoBehaviour, IPointerClickHandler
{
    public Action<GameObject, string> callback;
    public bool returnText = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (returnText)
            callback.Invoke(eventData.pointerCurrentRaycast.gameObject, eventData.pointerCurrentRaycast.gameObject.GetComponent<TMP_Text>().text);
        else
            callback.Invoke(eventData.pointerCurrentRaycast.gameObject, eventData.pointerCurrentRaycast.gameObject.name);
    }
}
