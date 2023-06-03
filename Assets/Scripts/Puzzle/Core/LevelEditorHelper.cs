using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorHelper : Singleton<LevelEditorHelper>
{
    private void Start() {
        Destroy(gameObject);
    }

    public void CleanChildren()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
