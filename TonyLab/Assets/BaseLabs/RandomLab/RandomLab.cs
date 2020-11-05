using NonsensicalKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLab : MonoBehaviour
{
    [SerializeField]
    private GameObject cube;

    void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            Instantiate(cube, VectorHelper.GetCirclePoint(20), Quaternion.identity);
        }
    }
}
