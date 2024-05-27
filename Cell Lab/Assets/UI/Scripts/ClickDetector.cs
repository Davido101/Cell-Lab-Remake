using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDetector : MonoBehaviour, IPointerClickHandler
{
    public Action<GameObject, string> callback;

    public void OnPointerClick(PointerEventData eventData)
    {
        callback.Invoke(eventData.pointerCurrentRaycast.gameObject, eventData.pointerCurrentRaycast.gameObject.GetComponent<TMP_Text>().text);
    }
}
