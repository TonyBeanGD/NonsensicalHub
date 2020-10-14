using NonsensicalFrame;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetBundleLab : MonoBehaviour
{
    
    private void Awake()
    {
        AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine (Application.streamingAssetsPath, "AssetBundles"));
        AssetBundleManifest abm = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        Debug.Log(StringHelper.GetSetString(abm.GetAllAssetBundles()));
    }
}
