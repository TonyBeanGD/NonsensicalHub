using NonsensicalFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingObject : GranulationObject
{
    protected override void Awake()
    {
        base.Awake();

        gameObject.AddComponent<MeshFilter>().mesh = NonsensicalFrame.ModelHelper.GetCube(1f, 1f, 1f);
        gameObject.AddComponent<MeshRenderer>().material=Resources.Load<Material>("Materials/white");
    }
}
