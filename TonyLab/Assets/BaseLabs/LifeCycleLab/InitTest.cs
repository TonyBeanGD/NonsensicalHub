using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitTest : MonoBehaviour
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

    public void Init()
    {
        Debug.Log(gameObject.name + " Init");
    }
}
