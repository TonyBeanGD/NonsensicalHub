using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LitJsonHelper
{
    /// <summary>
    /// litjson获取集合键名
    /// </summary>
    /// <param name="originString"></param>
    private void GetKeyName(string originString)
    {
        JsonData jsonData = JsonMapper.ToObject(originString);

        IDictionary idic = (IDictionary)jsonData;
        foreach (var item in idic.Keys)
        {
            Debug.Log(item.ToString());
        }
    }


    /// <summary>
    /// 动态生成json(使用递归)
    /// 输入：JsonData jd=test(new string[]{"1","2","3"},10086);
    /// 结果：Debug.Log(jd["0"]["1"]["2"]);    //=10086
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private JsonData Test(string[] a, int b, int pos = 0)
    {
        if (pos == a.Length)
        {
            return b;
        }
        else
        {
            JsonData jd = new JsonData();

            jd[a[(int)pos]] = Test(a, b, pos + 1);

            return jd;
        }
    }

    /// <summary>
    /// 动态生成json(不使用递归)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private JsonData Test2(string[] a, int b)
    {
        JsonData jd = new JsonData();

        int pos = a.Length - 1;

        while (pos >= 0)
        {
            if (pos == a.Length - 1)
            {
                jd[a[pos]] = b;
            }
            else
            {
                //直接使用jd[a[pos]] = temp;会导致堆栈溢出异常
                JsonData temp = jd;
                jd = new JsonData();
                jd[a[pos]] = temp;
            }

            pos--;
        }

        return jd;
    }
}
