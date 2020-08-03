using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        MyClass my = new MyClass();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

class MyClass
{
    public MyClass() : this(1) { Debug.Log(2); }
    public MyClass(int i)
    {
        Debug.Log(i);
    }
}