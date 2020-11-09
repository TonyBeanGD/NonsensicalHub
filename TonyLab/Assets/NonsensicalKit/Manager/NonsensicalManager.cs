using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NonsensicalKit
{
    public class NonsensicalManager : MonoBehaviour
    {
        [SerializeField]
        private bool UseAppConfigManager = true;

        [SerializeField]
        private bool UseAssetBundleManager_Local = true;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (UseAssetBundleManager_Local)
            {
                gameObject.AddComponent<AssetBundleManager_Local>().Init();
            }
            if (UseAppConfigManager)
            {
                gameObject.AddComponent<AppConfigManager>().Init();
            }

            string logLock = Path.Combine(Application.streamingAssetsPath, "Nonsensical");

            if (File.Exists(logLock))
            {
                File.Delete(logLock);
                gameObject.AddComponent<DebugConsole>();
            }
        }
    }
}
