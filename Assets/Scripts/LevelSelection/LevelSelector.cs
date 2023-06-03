using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelSelector : MonoBehaviour
{
    public static int SelectedLevelNumber { get; private set; }
    [SerializeField] private int levelNumber;
    [SerializeField] private TMP_Text text;

    void Start()
    {
        text.text = levelNumber.ToString();
    }

    public void OpenScene()
    {
        SelectedLevelNumber = levelNumber;
        SceneManager.LoadScene("Level");
    }
}
