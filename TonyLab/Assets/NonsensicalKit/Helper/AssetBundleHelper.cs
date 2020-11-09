using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NonsensicalKit
{
    public class AssetBundleHelper
    {
        public IEnumerator LoadWebAssetbundle(string uri, uint version, Action<AssetBundle> callback, uint crc = 0)
        {
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri, version, crc);
            yield return request.SendWebRequest();
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

            callback(bundle);
        }
    }
}
