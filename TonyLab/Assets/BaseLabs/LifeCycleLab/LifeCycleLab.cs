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

    /// <summary>
    /// 被AddComponent时，直接执行Awake和OnEnable,会在下一帧执行Start
    /// </summary>
    void Start()
    {
        Debug.Log(gameObject.name + " Start");

        GameObject go = new GameObject("initTest");

        go.AddComponent<InitTest>().Init();
        GameObject go2 = new GameObject("initTest2");

        go2.AddComponent<InitTest>();
        go2.GetComponent<InitTest>().Init();
    }

    void Update()
    {

        Debug.Log(gameObject.name + " Update");
    }

}
