using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class CubeUtility
{
    /// <summary>
    /// Return a position which another cube can be put at next to a given cube.
    /// </summary>
    /// <param name="cube"></param>
    /// <param name="pointOnCubeSurface"></param>
    /// <returns></returns>
    public static Vector3Int CubePositionNextTo(Cube cube, Vector3 pointOnCubeSurface)
    {
        // get 6 center points of cube planes
        var cubeCenter = cube.transform.position;
        float cubeSize = 1;
        var centers = new Vector3[6]{
            cubeCenter + new Vector3(cubeSize / 2, 0, 0),
            cubeCenter + new Vector3(-cubeSize / 2, 0, 0),
            cubeCenter + new Vector3(0, cubeSize / 2, 0),
            cubeCenter + new Vector3(0, -cubeSize / 2, 0),
            cubeCenter + new Vector3(0, 0, cubeSize / 2),
            cubeCenter + new Vector3(0, 0, -cubeSize / 2),
        };

        // find the closest center point from the given point on cube surface
        float minDistance = float.MaxValue;
        var closest = Vector3.zero;
        foreach(var center in centers)
        {
            float distance = (pointOnCubeSurface - center).sqrMagnitude;
            if(distance < minDistance) 
            {
                minDistance = distance;
                closest = center;
            }
        }

        // return the another cube's position
        var normal = (closest - cubeCenter).normalized;
        return Vector3Int.FloorToInt(cubeCenter + normal * cubeSize);
    }
}
