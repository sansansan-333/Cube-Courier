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
        Debug.Log("on selected");
        var material = GetComponent<MeshRenderer>().material;
        var color = material.color;
        color.a = 0.7f;
        material.color = color;
    }

    public void OnDeselected()
    {
        Debug.Log("on deselected");
        var material = GetComponent<MeshRenderer>().material;
        var color = material.color;
        color.a = 1;
        material.color = color;   
    }
}
