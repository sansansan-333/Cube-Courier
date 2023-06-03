using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

// controls game flow
public class GameManager : Singleton<GameManager>
{
    private Player player;
    private CameraPivotControl cameraPivotControl;

    void Start()
    {
        cameraPivotControl = FindObjectOfType<CameraPivotControl>();

        LoadLevel(LevelSelector.SelectedLevelNumber);
    }

    void Update()
    {
        if(player != null) player.ManagedUpdate();
    }

    public void LoadLevel(int levelNumber)
    {
        if (1 <= levelNumber && levelNumber <= ResourceSystem.Instance.Levels.Count)
        {
            PuzzleManager.Instance.SpawnLevel(ResourceSystem.Instance.Levels[levelNumber - 1]);
            if(cameraPivotControl != null) cameraPivotControl.CenterCamera();
            player = new Player();
        }
    }

    public void GoToNextLevel()
    {
        var currentLevelData = PuzzleManager.Instance.CurrentLevelData;
        PuzzleManager.Instance.DespawnCurrentLevel();
        LoadLevel(ResourceSystem.Instance.GetLevelNumber(currentLevelData) + 1);
    }
}
