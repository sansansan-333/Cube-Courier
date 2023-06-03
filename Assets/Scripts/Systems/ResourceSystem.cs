using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// one repository for all scriptable objects
public class ResourceSystem : Singleton<ResourceSystem>
{
    public PuzzleSettings PuzzleSettings => _puzzleSettings;
    public Material CubeDefaultMaterial => _cubeDefaultMaterial;
    public List<LevelData> Levels => _levels;
    [SerializeField] private PuzzleSettings _puzzleSettings;
    [SerializeField] private Material _cubeDefaultMaterial;
    [SerializeField] private List<LevelData> _levels;

    public static readonly string LEVEL_DATA_PATH = "Assets/Resources/LevelData/";

    public int GetLevelNumber(LevelData levelData)
    {
        return _levels.IndexOf(levelData) + 1;
    }
}
