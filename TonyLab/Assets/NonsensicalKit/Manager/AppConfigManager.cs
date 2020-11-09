using System.IO;
using UnityEngine;

namespace NonsensicalKit
{
    public class AppConfigManager : NonsensicalManager<AppConfigManager>,IManager
    {
        [HideInInspector]
        public AppConfigArgs AppConfig;

        private string ConfigFilePath;

        public void Init()
        {
            ConfigFilePath = Path.Combine(Application.streamingAssetsPath, @"Config", @"AppConfig.json");

#if UNITY_WEBGL
            //WebGL打包时必须使用http方式读取
            LoadAppConfigByUnityWebRequest();
#else   
            LoadAppConfigByStreamReader();
#endif
        }

        public void LateInit()
        {

        }
        
        private void LoadAppConfigByStreamReader()
        {
            string fileContent = FileHelper.GetFileString(ConfigFilePath);

            AppConfig = LitJson.JsonMapper.ToObject<AppConfigArgs>(fileContent);
        }

        private void LoadAppConfigByUnityWebRequest()
        {
            StartCoroutine(HttpHelper.GetText(ConfigFilePath, (content)=> 
            {
                AppConfig = LitJson.JsonMapper.ToObject<AppConfigArgs>(content);
            }));
        }
    }

}
