using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLab : MonoBehaviour {

    private void Update()
    {
        Stack<Vector3> stack = new Stack<Vector3>();

        for (int i = 0; i < 100000; i++)
        {
            stack.Push(new Vector3(1, 1, 1));
        }
        List<Vector3> list = new List<Vector3>();

        for (int i = 0; i < 100000; i++)
        {
            list.Add(new Vector3(1,1,1));
        }

    }
}
