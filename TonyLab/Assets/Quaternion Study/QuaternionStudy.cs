using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaternionStudy : MonoBehaviour
{
    enum state
    {
        Rzzz,
        zRzz,
        zzRz,
        zzzR,
    }

    state crtstate;

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 50, 30), "Rotate,0,0,0"))
        {
            crtstate = state.Rzzz;
        }
        if (GUI.Button(new Rect(10, 50, 50, 30), "0,Rotate,0,0"))
        {
            crtstate = state.zzzR;
        }
        if (GUI.Button(new Rect(10, 90, 50, 30), "0,0,Rotate,0"))
        {
            crtstate = state.zzzR;
        }
        if (GUI.Button(new Rect(10, 130, 50, 30), "0,0,0,Rotate"))
        {
            crtstate = state.zzzR;
        }
    }

    private void Update()
    {
        Debug.Log((Time.frameCount % 50f) / 50f);
        switch (crtstate)
        {
            case state.Rzzz:
                transform.rotation = new Quaternion((Time.frameCount % 50f) / 50f, 0, 0, 0);
                break;
            case state.zRzz:
                transform.rotation = new Quaternion(0, (Time.frameCount % 50f) / 50f, 0, 0);
                break;
            case state.zzRz:
                transform.rotation = new Quaternion(0, (Time.frameCount % 50f) / 50f, 0, 0);
                break;
            case state.zzzR:
                transform.rotation = new Quaternion(0, 0, 0, (Time.frameCount % 50f) / 50f);
                break;
            default:
                break;
        }
    }
}
