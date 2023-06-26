using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Substrate substrate;
    
    void Start()
    {

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
