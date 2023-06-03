using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
    private static Vector3 v;

    public static void X(this Transform transform, float x)
    {
        v.Set(x, transform.position.y, transform.position.z);
        transform.position = v;
    }

    public static void Y(this Transform transform, float y)
    {
        v.Set(transform.position.x, y, transform.position.z);
        transform.position = v;
    }

    public static void Z(this Transform transform, float z)
    {
        v.Set(transform.position.x, transform.position.y, z);
        transform.position = v;
    }

    public static void LocalX(this Transform transform, float x)
    {
        v.Set(x, transform.position.y, transform.position.z);
        transform.localPosition = v;
    }

    public static void LocalY(this Transform transform, float y)
    {
        v.Set(transform.position.x, y, transform.position.z);
        transform.localPosition = v;
    }

    public static void LocalZ(this Transform transform, float z)
    {
        v.Set(transform.position.x, transform.position.y, z);
        transform.localPosition = v;
    }
}
