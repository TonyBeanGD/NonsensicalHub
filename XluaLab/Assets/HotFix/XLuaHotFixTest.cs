using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[Hotfix]
public class XLuaHotFixTest : MonoBehaviour
{
    private void Update()
    {
        if (Time.frameCount % 50 == 0)
        {
            Debug.Log(GetMessage());
        }
    }

    public string GetMessage()
    {
        return "RawMessage";
    }
}
