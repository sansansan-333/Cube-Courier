using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move 
{
    public Cube cube;
    public Vector3Int originalPosition;
    public MoveDirection direction;
    public Vector3Int endPosition;

    public Move(Cube cube, Vector3Int originalPosition, MoveDirection direction, Vector3Int endPosition)
    {
        this.cube = cube;
        this.originalPosition = originalPosition;
        this.direction = direction;
        this.endPosition = endPosition;
    }
}
