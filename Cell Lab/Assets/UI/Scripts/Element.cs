using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    RectTransform rectTransform;
    RectTransform fillRect;
    public int childId;

    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        fillRect = transform.GetChild(childId).GetChild(1).GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, fillRect.rect.height);
    }
}
