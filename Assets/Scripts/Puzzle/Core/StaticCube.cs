using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCube : Cube
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetData(StaticCubeData data)
    {
        position = data.position;
        transform.position = data.position;
        if(Resources.Load<Material>(data.materialName) != null) 
        {
            GetComponent<MeshRenderer>().material = Resources.Load<Material>(data.materialName);
        }
        else
        {
            GetComponent<MeshRenderer>().material = ResourceSystem.Instance.CubeDefaultMaterial;
        }
    }
}
