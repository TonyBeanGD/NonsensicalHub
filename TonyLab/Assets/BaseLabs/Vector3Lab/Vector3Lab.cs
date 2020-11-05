using NonsensicalKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3Lab : MonoBehaviour
{
    [SerializeField]
    private Transform cube1;
    [SerializeField]
    private Transform cube2;
    [SerializeField]
    private Transform ball1;
    [SerializeField]
    private Transform ball2;

    private void Start()
    {
        CoordinateSystem cs1 = new CoordinateSystem(cube1);
        CoordinateSystem cs2 = new CoordinateSystem(cube2);

        var t1F = cs1.GetCoordinate(ball1.position);

        CoordinateSystemDiff csd = new CoordinateSystemDiff(cs1, cs2);
        
        var t2F = csd.GetCoordinate( t1F) ;

        var t2WorldPos = cs2.GetWorldPos(t2F);

        ball2.position = t2WorldPos;
    }
}
