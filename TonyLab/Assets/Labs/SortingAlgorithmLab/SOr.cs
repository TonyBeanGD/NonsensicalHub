﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOr : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
//冒泡排序：
// class Program
//{
//    static void Main(string[] args)
//    {
//        Random r = new Random();
//        int[] a = new int[9];
//        int temp = 0, prevtemp = 0;

//        for (int i = 0; i < a.Length; i++)
//        {
//            while (true)
//            {
//                temp = r.Next(100);
//                if (prevtemp != temp)
//                {
//                    a[i] = temp;
//                    prevtemp = temp;
//                    break;
//                }
//            }
//        }

//        Console.WriteLine("原数组:");
//        for (int i = 0; i < a.Length; i++)
//        {
//            Console.Write(a[i] + " ");
//        }
//        Console.WriteLine();

//        for (int i = 0; i < a.Length - 1; i++)
//        {
//            for (int j = 0; j < a.Length - 1 - i; j++)
//            {
//                if (a[j] > a[j + 1])
//                {
//                    temp = a[j];
//                    a[j] = a[j + 1];
//                    a[j + 1] = temp;
//                }
//            }
//        }

//        Console.WriteLine("排序后的数组:");
//        for (int i = 0; i < a.Length; i++)
//        {
//            Console.Write(a[i] + " ");
//        }

//        Console.ReadLine();
//    }
//}

