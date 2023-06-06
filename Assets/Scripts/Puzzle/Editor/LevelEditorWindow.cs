using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEditorWindow : EditorWindow
{
    private static Level level;
    private static LevelData levelData; 
    private static PuzzleManager puzzleManager; 
    private static string LevelName
    {
        // save the value so that we can replicate the previous state when exiting and returning to the editor
        get { return EditorPrefs.GetString(nameof(LevelName), ""); }
        set { EditorPrefs.SetString(nameof(LevelName), value); }
    }

    private static GUIStyle panelGroupStyle;

    private static string log;
    private static Vector2 scrollPosition;
    private static int selectedTool = 0;
    private static bool isToolActive;
    private static readonly int TOOL_ADD_CUBE = 0;
    private static readonly int TOOL_EDIT_CUBE = 1;
    private static readonly int TOOL_REMOVE_CUBE = 2;
    private static CubeInfo addingCubeInfo;
    private static CubeInfo editingCubeInfo;


    [MenuItem("Editor/Level Editor %l")]
    public static void OpenEditorWindow()
    {
        Initialize();

        var window = GetWindow<LevelEditorWindow>(false, "Level Editor", true);
        window.Show();

        if(IsCurrentSceneValid()) LoadLevel(false);
    }

    private static void Initialize()
    {
        if(!IsCurrentSceneValid()) return;

        SceneView.duringSceneGui += HandleMouseEvents;
        SceneView.duringSceneGui += DrawActivateTool;

        puzzleManager = PuzzleManager.Instance;
        addingCubeInfo = new CubeInfo();
        editingCubeInfo = new CubeInfo();

        EditorApplication.playModeStateChanged += (PlayModeStateChange state) => {
            if(state == PlayModeStateChange.ExitingEditMode && IsCurrentSceneValid()) CleanLevelInScene();
            if(state == PlayModeStateChange.EnteredEditMode) LoadLevel(false);
        };
    }

    private void OnEnable()
    {
        if (!IsCurrentSceneValid()) return;

        Initialize();
        
        if(!EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying) 
        {
            LoadLevel(false);
        }
    }

    private void OnDestroy() 
    {
        if (!IsCurrentSceneValid() || EditorApplication.isPlaying) return;

        SceneView.duringSceneGui -= HandleMouseEvents;
        SceneView.duringSceneGui -= DrawActivateTool;

        CleanLevelInScene();
    }

    private static bool IsCurrentSceneValid()
    {
        return EditorSceneManager.GetActiveScene().name == "Level";
    }

    #region UI handling

    private void OnGUI()
    {
        if(Application.isPlaying) return;
        if (!IsCurrentSceneValid()) return;

        panelGroupStyle = "GroupBox";

        using(var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
        {
            scrollPosition = scrollView.scrollPosition;

            DrawSaveLoad();
            DrawCubeEditor();
            DrawOtherTools();

            EditorGUILayout.Space();

            DrawLogConsole();
        }
    }

    private static void DrawSaveLoad()
    {
        using (new EditorGUILayout.VerticalScope(panelGroupStyle))
        {
            GUILayout.Label("SAVE/LOAD");
            
            LevelName = GUILayout.TextField(LevelName);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("SAVE"))
                {
                    SaveLevel();
                }
                if (GUILayout.Button("LOAD"))
                {
                    LoadLevel();
                }
                if (GUILayout.Button("NEW"))
                {
                    NewLevel();
                }
            }

            EditorGUILayout.ObjectField("LEVEL DATA (get only)", levelData, typeof(LevelData), false);
        }
    }

    private static void DrawCubeEditor()
    {
        using(new EditorGUILayout.VerticalScope(panelGroupStyle))
        {
            GUILayout.Label("CUBE");

            selectedTool = GUILayout.SelectionGrid(selectedTool, new string[] { "ADD", "EDIT", "REMOVE" }, 3);
            if (selectedTool == TOOL_ADD_CUBE)
            {
                DrawAddCube();
            }
            else if (selectedTool == TOOL_EDIT_CUBE)
            {
                DrawEditCube();
            }
            else if(selectedTool == TOOL_REMOVE_CUBE)
            {
                DrawRemoveCube();
            }
        }
    }

    private static void DrawAddCube()
    {
        using (new EditorGUILayout.VerticalScope(panelGroupStyle))
        {
            addingCubeInfo.cubeType = (CubeType)EditorGUILayout.EnumPopup("Cube Type", addingCubeInfo.cubeType);
            // addingCubeInfo.position = EditorGUILayout.Vector3IntField("Position", addingCubeInfo.position);

            if(addingCubeInfo.cubeType == CubeType.ColorCube)
            {
                addingCubeInfo.color = (CubeColor)EditorGUILayout.EnumPopup("Color", addingCubeInfo.color);
                addingCubeInfo.movable = EditorGUILayout.Toggle("Movable", addingCubeInfo.movable);
            }
            else if(addingCubeInfo.cubeType == CubeType.StaticCube)
            {
                addingCubeInfo.material = (Material)EditorGUILayout.ObjectField(addingCubeInfo.material, typeof(Material), false);
            }
        }
    }

    private static void DrawEditCube()
    {
        using (new EditorGUILayout.VerticalScope(panelGroupStyle))
        {
            EditorGUILayout.LabelField("Not yet");
        }
    }

    private static void DrawRemoveCube()
    {
        using(new EditorGUILayout.VerticalScope(panelGroupStyle))
        {
            EditorGUILayout.LabelField("Click a cube on the scene to remove it.");
        }
    }

    private static void DrawOtherTools()
    {
        using (new EditorGUILayout.VerticalScope(panelGroupStyle))
        {
            EditorGUILayout.LabelField("OTHER");

            // center the camera
            if(GUILayout.Button("Center the Camera"))
            {
                var cameraPivot = FindObjectOfType<CameraPivotControl>();
                if(cameraPivot != null) cameraPivot.CenterCamera();
                else
                {
                    log = "Couldn't center the camera. CameraPivotControl was not found.";
                }
            }
        }
    }

    private static void DrawLogConsole()
    {
        using(new EditorGUILayout.VerticalScope(panelGroupStyle))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Console");

                EditorGUILayout.Space();

                if (GUILayout.Button("clear", GUILayout.Width(50)))
                {
                    log = "";
                }
            }
            GUILayout.TextArea(log);
        }
    }

    private static void DrawActivateTool(SceneView sceneView)
    {
        if (Application.isPlaying) return;

        float width = 200, height = 50;

        Handles.BeginGUI();
        GUILayout.BeginArea(
            new Rect(sceneView.position.width - width, 
                    sceneView.position.height - height, 
                    width, 
                    height), 
            EditorStyles.toolbar);
        {
            string onoff = isToolActive ? "ACTIVE" : "INACTIVE";
            if (GUILayout.Button($"Level editor tool {onoff}"))
            {
                isToolActive = !isToolActive;
                if (!IsDataLoaded())
                {
                    isToolActive = false;
                    log = "Load level data before using tools.";
                }
            }
        }
        GUILayout.EndArea();
        Handles.EndGUI();
    }

    #endregion UI handling

    #region Mouse events

    static void HandleMouseEvents(SceneView sceneView)
    {
        if (Application.isPlaying) return;

        if (isToolActive && IsSceneClicked())
        {
            if (selectedTool == TOOL_ADD_CUBE)
            {
                AddCube();
            }
            else if (selectedTool == TOOL_EDIT_CUBE)
            {
                // TODO
            }
            else if(selectedTool == TOOL_REMOVE_CUBE)
            {
                RemoveCube();
            }
        }
    }

    private static bool IsSceneClicked()
    {
        return Event.current.type == EventType.MouseDown &&
                Event.current.button == 0 &&
                Event.current.alt == false &&
                Event.current.shift == false &&
                Event.current.control == false;
    }

    /// <summary>
    /// Spawn a cube next to a clicked cube.
    /// </summary>
    private static void AddCube()
    {
        if (Camera.current == null) return;

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var cube = hit.collider.gameObject.GetComponent<Cube>();
            if(cube != null)
            {
                addingCubeInfo.position = CubeUtility.CubePositionNextTo(cube, hit.point);
                var cubeData = addingCubeInfo.ToCubeData();
                puzzleManager.SpawnCube(cubeData);
            }
        }
    }

    private static void RemoveCube()
    {
        if (Camera.current == null) return;

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var cube = hit.collider.gameObject.GetComponent<Cube>();
            if (cube != null)
            {
                puzzleManager.DespawnCube(cube);
            }
        }
    }

    #endregion Mouse events

    #region Level creation

    private static void SaveLevel(bool sendMessages = true)
    {
        if (level == null || levelData == null)
        {
            if (sendMessages) log = "Saving failed. There is no current data.";
            return;
        }

        DiscretizeCubePostions();
        UpdateLevelData();
    }
    
    private static void LoadLevel(bool sendMessages = true)
    {
        if (string.IsNullOrEmpty(LevelName))
        {
            if(sendMessages) log = "Puzzle name is empty. Enter puzzle name.";
            return;
        }

        // get puzzle data guid
        string[] guids = AssetDatabase.FindAssets(LevelName, new string[] { ResourceSystem.LEVEL_DATA_PATH });
        if (guids.Length == 0)
        {
            if (sendMessages) log = $"Load failed. Puzzle data was not found in {ResourceSystem.LEVEL_DATA_PATH}.";
            return;
        }

        // confirmation dialog
        if (sendMessages &&
            level != null && levelData != null &&
            !EditorUtility.DisplayDialog(
            "Load puzzle",
            "Changes on the current level that you haven't saved will be lost.\nAre you sure you want to load anyways?",
            "OK",
            "Cancel"))
        {
            return;
        }

        // get puzzle data file and spawn it
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        levelData = AssetDatabase.LoadAssetAtPath<LevelData>(path);

        CleanLevelInScene();
        level = puzzleManager.SpawnLevel(levelData);
        level.transform.SetParent(LevelEditorHelper.Instance.gameObject.transform);
    }

    private static void NewLevel(bool sendMessages = true) 
    {
        if (string.IsNullOrEmpty(LevelName))
        {
            if (sendMessages) log = "Puzzle name is empty. Enter puzzle name.";
            return;
        }

        if (System.IO.File.Exists(ResourceSystem.LEVEL_DATA_PATH + LevelName + ".asset"))
        {
            if (sendMessages) log = "Puzzle data with the same name already exists.";
            return;
        }

        // confirmation dialog
        if (sendMessages &&
            level != null && levelData != null &&
            !EditorUtility.DisplayDialog(
                "New puzzle",
                "Changes on the current level that you haven't saved will be lost.\nAre you sure you want to create new one anyways?",
                "OK",
                "Cancel"))
        {
            return;
        }

        levelData = LevelData.GetEmpty();
        CleanLevelInScene();
        level = puzzleManager.SpawnLevel(levelData);
        level.transform.SetParent(LevelEditorHelper.Instance.gameObject.transform);

        string path = ResourceSystem.LEVEL_DATA_PATH + LevelName + ".asset";
        log = $"Level data created ({path}).";
        AssetDatabase.CreateAsset(levelData, path);
    }

    private static void CleanLevelInScene()
    {
        level = null;
        LevelEditorHelper.Instance.CleanChildren();
    }

    private static void DiscretizeCubePostions()
    {
        foreach(var cube in level.GetCubes())
        {
            if(cube != null)
            {
                cube.position = Vector3Int.FloorToInt(cube.transform.localPosition);
                cube.transform.position = cube.position;
            }
        }
    }

    /// <summary>
    /// Update LevelData using cube objects.
    /// </summary>
    private static void UpdateLevelData()
    {
        levelData.colorCubes = new List<ColorCubeData>();
        levelData.staticCubes = new List<StaticCubeData>();

        foreach(var cube in level.GetCubes())
        {
            if(cube == null) continue;

            if(cube is ColorCube colorCube)
            {
                levelData.colorCubes.Add(
                    new ColorCubeData(
                        colorCube.position,
                        colorCube.movable,
                        colorCube.color
                    )
                );
            }

            if(cube is StaticCube staticCube)
            {
                levelData.staticCubes.Add(
                    new StaticCubeData(
                        staticCube.position,
                        staticCube.GetComponent<MeshRenderer>().sharedMaterial.name
                    )
                );
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.SetDirty(levelData);
    }

    private static bool IsDataLoaded()
    {
        return level != null && levelData != null;
    }

    #endregion Level creation

    private class CubeInfo
    {
        public CubeType cubeType;
        public Vector3Int position;

        // color cube info
        public CubeColor color;
        public bool movable;

        // static cube info
        public Material material;

        public CubeInfo()
        {
            movable = true;
            material = ResourceSystem.Instance.CubeDefaultMaterial;
        }

        public CubeData ToCubeData()
        {
            if(cubeType == CubeType.ColorCube)
            {
                return new ColorCubeData(position, movable, color);
            }
            else if(cubeType == CubeType.StaticCube)
            {
                return new StaticCubeData(position, material.name);
            }
            else
            {
                return null;
            }
        }
    }

    private enum CubeType
    {
        ColorCube,
        StaticCube
    }
}
