using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// repository for all prefabs
public class PrefabSystem : Singleton<PrefabSystem>
{
    public Level Level => _level;
    public ColorCube ColorCube => _colorCube;
    public StaticCube StaticCube => _staticCube;

    [SerializeField] private Level _level;
    [SerializeField] private ColorCube _colorCube;
    [SerializeField] private StaticCube _staticCube;
}
