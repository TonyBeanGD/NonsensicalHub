using NonsensicalKit;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CutObject : GranulationObject
{
    protected override void Awake()
    {
        base.Awake();

        gameObject.AddComponent<MeshFilter>().mesh = NonsensicalKit.ModelHelper.GetCube(10f, 2f,10f);
        gameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/white");
    }
}
