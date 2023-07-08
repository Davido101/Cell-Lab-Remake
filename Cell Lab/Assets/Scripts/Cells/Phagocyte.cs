using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phagocyte : Cell
{
    void Awake()
    {
        reactsToFood = true;
    }
}
