using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NonsensicalKit;
using NonsensicalKit.Helper;

public class Vector3Test : MonoBehaviour
{
    [SerializeField]
    private GameObject cube;
    private void Start()
    {
        transform.DoMove(new Vector3(10,10,10),10);
    }
}
