using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeCycleLab : MonoBehaviour
{

    private void Awake()
    {
        Debug.Log(gameObject.name + " Awake");
    }

    private void OnEnable()
    {
        Debug.Log(gameObject.name + " OnEnable");
    }

    void Start()
    {

        Debug.Log(gameObject.name + " Start");
    }

    void Update()
    {

        Debug.Log(gameObject.name + " Update");
    }
}
