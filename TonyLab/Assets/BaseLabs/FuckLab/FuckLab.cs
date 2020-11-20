using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuckLab : MonoBehaviour
{
    void Start()
    {
        TestStruct t1 = new TestStruct(1);

        TestStruct t2 = t1;

        t2.index = 2;
        t2.indexs[0] = 2;

        Debug.Log(t1.index);
        Debug.Log(t1.indexs[0]);
    }
}
public struct TestStruct
{
  public  int index;
  public  List<int> indexs;

    public TestStruct(int index)
    {
        this.index = index;
        indexs = new List<int>();
        indexs.Add(index);
    }
}
