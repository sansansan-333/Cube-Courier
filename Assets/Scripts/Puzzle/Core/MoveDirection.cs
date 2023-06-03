using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents one of four directions or zero, namely (1, 0), (-1, 0), (0, 1), (0, -1), and (0, 0).
/// </summary>
public class MoveDirection 
{
    public int X { get; private set; }
    public int Z { get; private set; }

    public MoveDirection()
    {
        X = 0;
        Z = 0;
    }

    public MoveDirection(Vector3 v)
    {
        if(Mathf.Abs(v.x) > Mathf.Abs(v.z))
        {
            X = Math.Sign(v.x);
            Z = 0;
        }
        else 
        {
            X = 0;
            Z = Math.Sign(v.z);
        }
    }

    public override string ToString()
    {
        return $"({X}, {Z})";
    }

    #region operator

    public static Vector3 operator+ (Vector3 v, MoveDirection direction)
    {
        v.x += direction.X;
        v.z += direction.Z;
        return v;
    }

    public static Vector3 operator +(MoveDirection direction, Vector3 v)
    {
        return v + direction;
    }

    public static Vector3Int operator +(Vector3Int v, MoveDirection direction)
    {
        v.x += direction.X;
        v.z += direction.Z;
        return v;
    }

    public static Vector3Int operator +(MoveDirection direction, Vector3Int v)
    {
        return v + direction;
    }

    #endregion operator
}
