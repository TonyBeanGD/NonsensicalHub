using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectLab : MonoBehaviour {

    private void Awake()
    {
        Transform[] ms = transform.GetComponentsInChildren<Transform>();

        foreach (var item in ms)
        {
            Debug.Log(item.gameObject.name);
        }
    }
}
