using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

// controls game flow
public class GameManager : Singleton<GameManager>
{
    public Player Player { get; private set; }
    private CameraPivotControl cameraPivotControl;

    void Start()
    {
        cameraPivotControl = FindObjectOfType<CameraPivotControl>();

        LoadLevel(LevelSelector.SelectedLevelNumber);
    }

    void Update()
    {
        if(Player != null) Player.ManagedUpdate();
    }

    public void LoadLevel(int levelNumber)
    {
        if (1 <= levelNumber && levelNumber <= ResourceSystem.Instance.Levels.Count)
        {
            PuzzleManager.Instance.SpawnLevel(ResourceSystem.Instance.Levels[levelNumber - 1]);
            if(cameraPivotControl != null) cameraPivotControl.CenterCamera();
            Player = new Player();
        }
    }

    public void GoToNextLevel()
    {
        var currentLevelData = PuzzleManager.Instance.CurrentLevelData;
        PuzzleManager.Instance.DespawnCurrentLevel();
        LoadLevel(ResourceSystem.Instance.GetLevelNumber(currentLevelData) + 1);
    }
}
