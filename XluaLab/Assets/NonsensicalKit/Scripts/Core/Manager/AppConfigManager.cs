using NonsensicalKit.Custom;
using NonsensicalKit.Utility;
using System.IO;
using UnityEngine;

namespace NonsensicalKit
{
    public class AppConfigManager : Singleton<AppConfigManager>, IManager
    {
        [HideInInspector]
        public AppConfigArgs AppConfig;

        private string ConfigFilePath;

        public void OnInit()
        {
            InitComplete = false;

            ConfigFilePath = Path.Combine(Application.streamingAssetsPath, @"AppConfig.json");
            
            LoadAppConfigByUnityWebRequest();
        }

        public void OnLateInit()
        {
            LateInitComplete = false;
            LateInitComplete = true;
        }
        

        private void LoadAppConfigByUnityWebRequest()
        {
            NonsensicalUnityInstance.Instance.StartCoroutine(HttpHelper.GetText(ConfigFilePath, (content) =>
            {
                if (content != null)
                {

                    AppConfig = LitJson.JsonMapper.ToObject<AppConfigArgs>(content);
                }

                InitComplete = true;
            }));
        }

        public bool InitComplete { get; private set; }
        public bool LateInitComplete { get; private set; }
    }

}
