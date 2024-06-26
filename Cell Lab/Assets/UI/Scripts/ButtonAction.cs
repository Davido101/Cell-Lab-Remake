using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAction : MonoBehaviour
{
    public Action triggered;
 
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => triggered.Invoke());
    }
}
