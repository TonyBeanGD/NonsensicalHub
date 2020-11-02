using NonsensicalFrame;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CutObject : GranulationObject
{
    protected override void Awake()
    {
        base.Awake();

        gameObject.AddComponent<MeshFilter>().mesh = NonsensicalFrame.ModelHelper.GetCube(4f, 2f, 4f);
        gameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/white");
    }
}
