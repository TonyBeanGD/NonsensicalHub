using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(new Float3(1, 2, 3)));
    }
}
