using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Substrate substrate;
    public Camera camera;

    void Start()
    {
        camera = (Camera)GameObject.FindObjectOfType(typeof(Camera));
    }

    void Update()
    {
        substrate.update();
    }

    void FixedUpdate()
    {
        substrate.fixedupdate();
    }
}
