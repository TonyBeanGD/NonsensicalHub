using System.IO;
using UnityEngine;

namespace NonsensicalKit
{
    public class AppConfigManager : MonoBehaviour
    {
        public static AppConfigArgs AppConfig;

        private string ConfigFilePath;

        private void Awake()
        {
            ConfigFilePath = Path.Combine(Application.streamingAssetsPath, @"Json", @"AppConfig.json");

#if UNITY_WEBGL
            //WebGL打包时必须使用http方式读取
            LoadAppConfigByUnityWebRequest();
#else   
            LoadAppConfigByStreamReader();
#endif
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
