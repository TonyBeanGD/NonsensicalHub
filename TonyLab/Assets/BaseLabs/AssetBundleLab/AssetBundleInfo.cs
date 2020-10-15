using NonsensicalFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleInfo : MonoBehaviour {

    private List<string> bundleNames;

	public void Init(string _bundleName)
    {
        bundleNames = new List<string>();

        bundleNames.Add(_bundleName);
    }

    public void Append(string _bundleName)
    {
        bundleNames.Add(_bundleName);
    }

    private void OnDestroy()
    {
        foreach (var item in bundleNames)
        {
           AssetBundleManager_Local.instance.
        }
    }
}
