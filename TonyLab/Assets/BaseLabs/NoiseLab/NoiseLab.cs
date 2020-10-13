using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseLab : MonoBehaviour
{
    void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                float y = Mathf.PerlinNoise(i, j);
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.position = new Vector3(i, y, j);
            }
        }
    }

    private void Update()
    {
        //Debug.Log(Mathf.PerlinNoise(Time.time, 0));
        Debug.Log(Mathf.PerlinNoise(0.5f ,0.5f));
        Debug.Log(Mathf.PerlinNoise(0.5f ,1.5f));
        Debug.Log(Mathf.PerlinNoise(1.5f ,0.5f));
        Debug.Log(Mathf.PerlinNoise(1.5f ,1.5f));
    }
}
