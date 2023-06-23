using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCube : Cube
{
    public bool movable;
    public CubeColor color;

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void SetData(ColorCubeData data)
    {
        position = data.position;
        transform.position = data.position;
        movable = data.movable;
        color = data.color;
        GetComponent<MeshRenderer>().material = ResourceSystem.Instance.PuzzleSettings.GetCubeMaterial(data.color);
    }

    public void OnSelected()
    {
        var material = GetComponent<MeshRenderer>().material;
        var color = material.color;
        color.a = 0.7f;
        material.color = color;

        GetComponent<BoxCollider>().enabled = false;
    }

    public void OnDeselected()
    {
        var material = GetComponent<MeshRenderer>().material;
        var color = material.color;
        color.a = 1;
        material.color = color;

        GetComponent<BoxCollider>().enabled = true;
    }

    public void OnLocked()
    {
        var tempMaterial = new Material(GetComponent<MeshRenderer>().sharedMaterial);
        tempMaterial.mainTexture = ResourceSystem.Instance.PuzzleSettings.lockedCubeTexture;
        GetComponent<MeshRenderer>().material = tempMaterial;
    }

    public void OnUnlocked()
    {
        var tempMaterial = new Material(GetComponent<MeshRenderer>().sharedMaterial);
        tempMaterial.mainTexture = ResourceSystem.Instance.PuzzleSettings.defaultCubeTexture;
        GetComponent<MeshRenderer>().material = tempMaterial;
    }
}
