using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NonsensicalKit;
using DG.Tweening;

public class Vector3Test : MonoBehaviour
{
    [SerializeField]
    private GameObject cube;
    private void Start()
    {
        transform.DOMove(new Vector3(10,10,10),10);
    }
}
