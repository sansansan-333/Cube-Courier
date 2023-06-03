using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Data", menuName = "ScriptableObjects/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public List<ColorCubeData> colorCubes;
    public List<StaticCubeData> staticCubes;

    public static LevelData GetEmpty()
    {
        var levelData = CreateInstance<LevelData>();
        levelData.colorCubes = new List<ColorCubeData>();
        levelData.staticCubes = new List<StaticCubeData>();

        return levelData;
    }

    public void AddCubeData(CubeData data)
    {
        if(data is ColorCubeData colorCubeData)
        {
            colorCubes.Add(colorCubeData);
        }
        else if(data is StaticCubeData staticCubeData)
        {
            staticCubes.Add(staticCubeData);
        }
    }
}