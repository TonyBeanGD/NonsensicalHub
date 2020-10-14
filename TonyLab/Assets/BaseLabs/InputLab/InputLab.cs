using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputLab : MonoBehaviour
{
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Debug.Log($"h:{h},v:{v}");
        float hr = Input.GetAxisRaw("Horizontal");
        float vr = Input.GetAxisRaw("Vertical");
        Debug.Log($"hr:{hr},vr:{vr}");
    }
}
