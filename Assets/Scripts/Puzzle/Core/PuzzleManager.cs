using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : Singleton<PuzzleManager>
{
    public Level CurrentLevel { get; private set; }
    public LevelData CurrentLevelData { get; private set; }

    public Level SpawnLevel(LevelData levelData)
    {
        if(levelData == null) return null;
    
        DespawnCurrentLevel();

        var prefabSystem = PrefabSystem.Instance;

        CurrentLevel = Instantiate(prefabSystem.Level);
        CurrentLevelData = levelData;
        
        // color cubes
        foreach(var cubeData in levelData.colorCubes)
        {
            SpawnCube(cubeData);
        }

        // static cubes
        foreach(var cubeData in levelData.staticCubes)
        {
            SpawnCube(cubeData);
        }

        return CurrentLevel;
    }

    public Cube SpawnCube(CubeData data)
    {
        var prefabSystem = PrefabSystem.Instance;
        Cube cube;

        if(data is ColorCubeData colorCubeData)
        {
            cube = Instantiate(prefabSystem.ColorCube);
            var colorCube = cube as ColorCube;
            colorCube.SetData(colorCubeData);
            if(!colorCube.movable) colorCube.OnLocked();
        }
        else if(data is StaticCubeData staticCubeData)
        {
            cube = Instantiate(prefabSystem.StaticCube);
            (cube as StaticCube).SetData(staticCubeData);
        }
        else
        {
            return null;
        }

        cube.transform.SetParent(CurrentLevel.transform, true);
        if(!CurrentLevel.TryToAddCube(cube))
        {
#if UNITY_EDITOR
            DestroyImmediate(cube.gameObject);
#else 
            Destroy(cube.gameObject);
#endif
            return null;
        }
        else
        {
            return cube;
        }
    }

    public void DespawnCurrentLevel()
    {
#if UNITY_EDITOR
        if (CurrentLevel != null) DestroyImmediate(CurrentLevel.gameObject);
#else 
        if(CurrentLevel != null) Destroy(CurrentLevel.gameObject);
#endif
    }

    public void DespawnCube(Cube cube)
    {
        CurrentLevel.RemoveCube(cube);
    }
}
