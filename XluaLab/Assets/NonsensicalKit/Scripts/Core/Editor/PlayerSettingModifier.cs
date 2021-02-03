using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NonsensicalKit.Editor
{

    public class PlayerSettingModifier : EditorWindow
    {
        public static bool isTest;
        public static string version;


        [MenuItem("TBTools/PlayerSetting修改器")]
        static void ShowWindows()
        {
            EditorWindow.GetWindow(typeof(PlayerSettingModifier));
        }

        private void OnGUI()
        {
            isTest = PlayerPrefs.GetInt("tb_playerSettingModifier_isTest", 0) == 1 ? true : false;
            isTest = EditorGUILayout.Toggle("是否测试版本", isTest, GUILayout.MinWidth(100f));
            PlayerPrefs.SetInt("tb_playerSettingModifier_isTest", isTest ? 1 : 0);

            version = PlayerPrefs.GetString("tb_playerSettingModifier_version", "1.0.0");
            version = EditorGUILayout.TextField("版本：", version);
            PlayerPrefs.SetString("tb_playerSettingModifier_version", version);

            if (GUILayout.Button("更改设置"))
            {
                Debug.Log("自动更改PlayerSetting完成，请手动修改图片等信息");
            }
        }

        private static void SetBwPlayerSetting(BuildInfo buildInfo)
        {
            PlayerSettings.companyName = buildInfo.companyname;
            PlayerSettings.productName = buildInfo.productName;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, buildInfo.applicationIndentifier);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, buildInfo.applicationIndentifier);
            PlayerSettings.bundleVersion = buildInfo.bundleVersion;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, buildInfo.scriptingDefineSymbols);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, buildInfo.scriptingDefineSymbols);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, buildInfo.scriptingDefineSymbols);
        }
        class BuildInfo
        {
            public string companyname;
            public string productName;
            public string applicationIndentifier;
            public string bundleVersion;
            public string scriptingDefineSymbols;

            public BuildInfo(string companyname, string productName, string applicationIndentifier, string bundleVersion, string scriptingDefineSymbols)
            {
                this.companyname = companyname;
                this.productName = productName;
                this.applicationIndentifier = applicationIndentifier;
                this.bundleVersion = bundleVersion;
                this.scriptingDefineSymbols = scriptingDefineSymbols;
            }
        }
    }

}
