using NonsensicalKit.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingAlgorithmLab : MonoBehaviour
{
    void Start()
    {
        System. Random r = new System.Random();
        int[] a = new int[9];
        int temp = 0, prevtemp = 0;

        for (int i = 0; i < a.Length; i++)
        {
            while (true)
            {
                temp = r.Next(100);
                if (prevtemp != temp)
                {
                    a[i] = temp;
                    prevtemp = temp;
                    break;
                }
            }
        }
        Debug.Log("原数组："+StringHelper.GetSetString(a));

        for (int i = 0; i < a.Length - 1; i++)
        {
            for (int j = 0; j < a.Length - 1 - i; j++)
            {
                if (a[j] > a[j + 1])
                {
                    temp = a[j];
                    a[j] = a[j + 1];
                    a[j + 1] = temp;
                }
            }
        }
        
        Debug.Log("排序后的数组：" + StringHelper.GetSetString(a));
    }
}
