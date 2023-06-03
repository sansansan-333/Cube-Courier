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
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var materials = property[nameof(PuzzleSettings.colorCubeMaterials)];
        var cubeColors = Enum.GetValues(typeof(CubeColor)).OfType<CubeColor>().ToList();

        using (new EditorGUILayout.VerticalScope())
        {
            // cube colors
            for (int i = 0; i < cubeColors.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(cubeColors[i].ToString());
                    var material = materials.GetArrayElementAtIndex(i);
                    material.objectReferenceValue = EditorGUILayout.ObjectField(material.objectReferenceValue, typeof(Material), false);
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
