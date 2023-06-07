using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelScreen : MonoBehaviour
{
    [SerializeField] private GameObject completedPopup;

    void Start()
    {
        CloseCompletedPopup();
    }

    public void GoToLevelSelection()
    {
        SceneManager.LoadScene("LevelSelection");
    }

    public void UndoMove()
    {
        if(PuzzleManager.Instance.CurrentLevel != null)
        {
            PuzzleManager.Instance.CurrentLevel.UndoMove();
        }
    }

    public void ShowCompletedPopup()
    {
        completedPopup.SetActive(true);
    }

    public void CloseCompletedPopup()
    {
        completedPopup.SetActive(false);
    }

    public void GoToNextLevel()
    {
        CloseCompletedPopup();
        GameManager.Instance.GoToNextLevel();
    }

    public void DeselectCube()
    {
        if (PuzzleManager.Instance.CurrentLevel != null)
        {
            PuzzleManager.Instance.CurrentLevel.DeselectCube();
        }
    }
}
