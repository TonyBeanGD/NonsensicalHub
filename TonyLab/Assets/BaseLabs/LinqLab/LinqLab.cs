using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LinqLab : MonoBehaviour
{
    void Start()
    {
        List<int> List_1 = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        List<int> List_2 = (from temp in List_1
                            where temp % 2 == 1
                            select temp).ToList();
        foreach (var item in List_2)
        {
            Debug.Log(item);
        }
    }
}
