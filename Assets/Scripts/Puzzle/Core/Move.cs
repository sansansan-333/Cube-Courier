using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move 
{
    public ColorCube colorCube;
    public Vector3Int startPosition;
    public MoveDirection direction;
    public Vector3Int endPosition;

    public Move(ColorCube colorCube, Vector3Int startPosition, MoveDirection direction, Vector3Int endPosition)
    {
        this.colorCube = colorCube;
        this.startPosition = startPosition;
        this.direction = direction;
        this.endPosition = endPosition;
    }
}
