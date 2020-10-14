using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelCutLab : MonoBehaviour
{
    [SerializeField]
    private Transform quad;

    private Material mesh;
    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        Plane cut = new Plane(quad.forward, quad.position);
        
        Vector4 dealData = new Vector4(cut.normal.x, cut.normal.y, cut.normal.z, cut.distance);

        // 传递法线与距离至shader中         
        mesh.SetVector("_Clip", dealData);
    }
}
