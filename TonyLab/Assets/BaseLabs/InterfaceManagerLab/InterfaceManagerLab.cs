using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using NonsensicalKit;
using NonsensicalKit.Utility;

public class InterfaceManagerLab : NonsensicalMono, ITestClass,ITestParameter
{
    void Start()
    {
        Subscribe<int>(1, OnDoit);
    }

    private void OnDoit(int i)
    {
        Debug.Log(i);
    }

    public void DoSomething()
    {
        Debug.Log("JustDoSomething");
    }

    public void OnDoSomething(bool b)
    {
        Debug.Log(b);
    }
}




public interface ITestClass : ICustomEventHandler
{
    void DoSomething();
}

public interface ITestParameter : ICustomEventHandler
{
    void OnDoSomething(bool b);
}
