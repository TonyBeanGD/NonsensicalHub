﻿using NonsensicalFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    AssetBundle test;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AssetBundleManager_Local.instance.LoadResource<GameObject>("Cuball", "cuball.prefab", (go) =>
            {
                Instantiate(go);
                Debugger.Log(go.name);
            });
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AssetBundleManager_Local.instance.LoadResource<GameObject>("GreyCube", "greycube.prefab", (go) =>
            {
                Instantiate(go);
                Debugger.Log(go.name);
            });
        }
    }
}
