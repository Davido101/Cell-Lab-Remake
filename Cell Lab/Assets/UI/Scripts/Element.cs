using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    RectTransform rectTransform;
    RectTransform fillRect;
    int frameCount;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        fillRect = transform.GetChild(1).GetChild(1).GetComponent<RectTransform>();
        frameCount = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, fillRect.rect.height);
    }
}
