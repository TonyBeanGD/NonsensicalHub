using NonsensicalKit;
using NonsensicalKit.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class StringLab : MonoBehaviour
{
    void Start()
    {
        string str1 = "(0_1_2_3_1_4_5_6_7_8_9)+(9_8_7_6_5_4_3_2_1_0)";
        Debug.Log("原字符串："+ str1);
        //替换：
       string str2 = str1.Replace("(", "").Replace(")", "");
        Debug.Log("将括号替换成空字符串后："+ str2);
        //切割
        string[] strArr = str2.Split('_');
        Debug.Log("按照'_'分割后的字符串数组：" + StringHelper.GetSetString(strArr));
    }
}