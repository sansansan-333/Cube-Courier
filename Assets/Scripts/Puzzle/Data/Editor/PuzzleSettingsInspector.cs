using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PuzzleSettings))]
public class PuzzleSettingsInspector : Editor
{
    private readonly Dictionary<string, SerializedProperty> property = new Dictionary<string, SerializedProperty>();

    private void OnEnable()
    {
        property.Add(nameof(PuzzleSettings.colorCubeMaterials), serializedObject.FindProperty(nameof(PuzzleSettings.colorCubeMaterials)));
        property.Add(nameof(PuzzleSettings.cubeMovingSpeedCurve), serializedObject.FindProperty(nameof(PuzzleSettings.cubeMovingSpeedCurve)));
        property.Add(nameof(PuzzleSettings.cubeFallingSpeedCurve), serializedObject.FindProperty(nameof(PuzzleSettings.cubeFallingSpeedCurve)));
        property.Add(nameof(PuzzleSettings.defaultCubeTexture), serializedObject.FindProperty(nameof(PuzzleSettings.defaultCubeTexture)));
        property.Add(nameof(PuzzleSettings.lockedCubeTexture), serializedObject.FindProperty(nameof(PuzzleSettings.lockedCubeTexture)));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        using (new EditorGUILayout.VerticalScope())
        {
            // cube colors
            var materials = property[nameof(PuzzleSettings.colorCubeMaterials)];
            var cubeColors = Enum.GetValues(typeof(CubeColor)).OfType<CubeColor>().ToList();

            while(materials.arraySize < cubeColors.Count) materials.arraySize++;

            for (int i = 0; i < cubeColors.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(cubeColors[i].ToString());
                    var material = materials.GetArrayElementAtIndex(i);
                    material.objectReferenceValue = EditorGUILayout.ObjectField(material.objectReferenceValue, typeof(Material), false);
                }
            }

            // speed curves
            var movingCurve = property[nameof(PuzzleSettings.cubeMovingSpeedCurve)];
            var fallingCurve = property[nameof(PuzzleSettings.cubeFallingSpeedCurve)];

            movingCurve.animationCurveValue = EditorGUILayout.CurveField("Moving Speed", movingCurve.animationCurveValue);
            fallingCurve.animationCurveValue = EditorGUILayout.CurveField("Falling Speed", fallingCurve.animationCurveValue);

            // default cube texture
            var defaultTexture = property[nameof(PuzzleSettings.defaultCubeTexture)];
            defaultTexture.objectReferenceValue = EditorGUILayout.ObjectField("Default Cube Texture", defaultTexture.objectReferenceValue, typeof(Texture2D), false);

            // locked cube texture
            var lockedTexture = property[nameof(PuzzleSettings.lockedCubeTexture)];
            lockedTexture.objectReferenceValue = EditorGUILayout.ObjectField("Locked Cube Texture", lockedTexture.objectReferenceValue, typeof(Texture2D), false);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
