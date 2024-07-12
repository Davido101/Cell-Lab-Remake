using System;
using UnityEngine;

public class CellInfo
{
    public Type cellType;
    public Material cellShader;

    public CellInfo(Type type, Material shader)
    {
        cellType = type;
        cellShader = shader;
    }
}
