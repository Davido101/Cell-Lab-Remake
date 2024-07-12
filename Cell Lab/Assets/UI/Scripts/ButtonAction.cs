using System;
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
