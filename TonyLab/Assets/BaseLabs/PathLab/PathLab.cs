using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLab : MonoBehaviour
{
    void Start()
    {
        foreach (Environment.SpecialFolder s in
                       Enum.GetValues(typeof(Environment.SpecialFolder)))
        {
            Debug.LogFormat("{0} folder : {1}",
                 s, Environment.GetFolderPath(s));
        }
    }
}
