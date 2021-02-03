using NonsensicalKit.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixLab : MonoBehaviour {

    [SerializeField] Transform point;
    private void Update()
    {
        NonsensicalDebugger.Log(transform.localToWorldMatrix);
        NonsensicalDebugger.Log(transform.up);
        NonsensicalDebugger.Log(transform.localToWorldMatrix.GetColumn(3));
        NonsensicalDebugger.Log(MeshHelper.Operator( transform .localToWorldMatrix, new Vector4(0.5f, 0.5f, 0.5f,1)), point.position);
    }
}
