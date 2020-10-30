using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelLab : MonoBehaviour
{

    void Start()
    {
        transform.GetComponent<MeshFilter>().mesh = NonsensicalFrame.ModelHelper.GetCube(0.1f, 0.1f, 0.1f);
    }
}
