using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinqHelper : MonoBehaviour
{
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

}
