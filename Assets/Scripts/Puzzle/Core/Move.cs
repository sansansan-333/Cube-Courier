using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMove 
{
    public ColorCube colorCube;
    public Vector3Int startPosition;
    public Vector3Int endPosition;

    public CubeMove(ColorCube colorCube, Vector3Int startPosition, Vector3Int endPosition)
    {
        this.colorCube = colorCube;
        this.startPosition = startPosition;
        this.endPosition = endPosition;
    }
}
