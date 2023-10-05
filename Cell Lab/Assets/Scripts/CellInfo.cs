using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;

public class CellInfo
{
    public Type cellType;
    public Sprite cellSprite;

    public CellInfo(Type type, Sprite sprite)
    {
        cellType = type;
        cellSprite = sprite;
    }
}
