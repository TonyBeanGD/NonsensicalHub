using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ZTest : MonoBehaviour
{
    void Start()
    {
        //获取变量名：
        string a = "Value";
        string s = GetVarName(p => a);
        Debug.Log(s);
    }

    /// <summary>
    /// 获取变量名
    /// 用法：
    /// string a = "Value";
    /// string s = GetVarName(p => a);
    /// </summary>
    /// <param name="exp"></param>
    /// <returns></returns>
    public static string GetVarName(System.Linq.Expressions.Expression<Func<string, string>> exp)
    {
        return ((System.Linq.Expressions.MemberExpression)exp.Body).Member.Name;
    }

    private void Test(string input)
    {
        //字节数组和字符串的相互转换
        Byte[] buffer = Encoding.UTF8.GetBytes(input);
        for (int i = 0; i < buffer.Length; i++)
        {
           Debug.Log(buffer[i]);
        }
        Debug.Log(new UTF8Encoding().GetString(buffer, 0, buffer.Length));
    }
}
/* 
		    unity3d提供了一个用于本地持久化保存与读取的类——PlayerPrefs。工作原理非常简单，以键值对的形式将数据保存在文件中，然后程序可以根据这个名称取出上次保存的数值。
		    PlayerPrefs类支持3中数据类型的保存和读取，浮点型，整形，和字符串型。
		    分别对应的函数为：
		    SetInt();保存整型数据；
		GetInt();读取整形数据；
		SetFloat();保存浮点型数据；
		    GetFlost();读取浮点型数据；
		    SetString();保存字符串型数据；
		    GetString();读取字符串型数据；
		这些函数的用法基本一致使用Set进行保存，使用Get进行读取。
		
		PlayerPrefs.SetString("_NAME", set_NAME); 这个方法中第一个参数表示存储数据的名称，第二的参数表示具体存储的数值。
		
		get_NAME=PlayerPrefs.GetString("_NAME"); 这个方法中第一个数据表示读取数据的名称，本来还有第二的参数，表示默认值，如果通过数据名称没有找到对应的值，那么就返回默认值，这个值也可以写，则返回空值。
		
		在PlayerPrefs 类中还提供了
		PlayerPrefs.DeleteKey (key : string)删除指定数据；
		PlayerPrefs.DeleteAll() 删除全部键 ;
		PlayerPrefs.HasKey (key : string)判断数据是否存在的方法；
 
 */
