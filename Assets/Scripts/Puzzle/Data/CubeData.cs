using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class CubeData
{
    public Vector3Int position;
}

[Serializable]
public class ColorCubeData : CubeData
{
    public bool movable;
    public CubeColor color;

    public ColorCubeData(Vector3Int position, bool movable, CubeColor color)
    {
        this.position = position;
        this.movable = movable;
        this.color = color;
    }
}

[Serializable]
public class StaticCubeData : CubeData
{
    public string materialName;

    public StaticCubeData(Vector3Int position, string materialName)
    {
        this.position = position;
        this.materialName = materialName;
    }
}