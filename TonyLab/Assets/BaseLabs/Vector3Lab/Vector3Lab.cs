using NonsensicalFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3Lab : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    
    [SerializeField]
    private Transform ball1;
    [SerializeField]
    private Transform ball2;



    private void Update()
    {
        CoordinateSystem cs1 = new CoordinateSystem(transform.position , transform.right, transform.up, transform.forward);
        CoordinateSystem cs2 = new CoordinateSystem(target.position, target.right, target.up, target.forward);

        Float3 data = new Float3(1,1,1);

        Vector3 ball2Pos = cs2.GetWorldPos(data);

        Vector3 ball1Pos = cs1.GetWorldPos( cs1.CoordinateSystemTransform(cs2,data));
        
        ball1.position = ball1Pos;
        ball2.position = ball2Pos;
    }
}
