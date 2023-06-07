using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Puzzle Settings", menuName = "ScriptableObjects/Puzzle Settings", order = 1)]
public class PuzzleSettings : ScriptableObject
{
    public List<Material> colorCubeMaterials;
    public AnimationCurve cubeMovingSpeedCurve;
    public AnimationCurve cubeFallingSpeedCurve;
    public Texture2D lockedCubeTexture;

    public void Awake()
    {
        if (colorCubeMaterials == null)
        {
            colorCubeMaterials = new List<Material>();
            foreach (var _ in Enum.GetValues(typeof(CubeColor))) colorCubeMaterials.Add(ResourceSystem.Instance.CubeDefaultMaterial);
        }
    }

    public Material GetCubeMaterial(CubeColor cubeColor)
    {
        return colorCubeMaterials[(int)cubeColor];
    }
}