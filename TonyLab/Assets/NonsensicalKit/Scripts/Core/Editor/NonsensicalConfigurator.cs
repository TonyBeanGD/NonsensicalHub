using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace NonsensicalKit.Editor
{
    public class NonsensicalConfigurator : EditorWindow
    {
        [MenuItem("TBTools/配置管理")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(NonsensicalConfigurator));
        }


        private static class NonsensicalConfiguratorPanel
        {
            public static bool jumpFirstOnPlay;
        }

        private void OnGUI()
        {
            NonsensicalConfiguratorPanel.jumpFirstOnPlay  = PlayerPrefs.GetInt("tb_nonsensicalConfigurator_jumpFirstOnPlay", 0)==0?false:true;
            NonsensicalConfiguratorPanel.jumpFirstOnPlay = EditorGUILayout.Toggle("运行时跳转至首个场景", NonsensicalConfiguratorPanel.jumpFirstOnPlay);
            PlayerPrefs.SetInt("tb_nonsensicalConfigurator_jumpFirstOnPlay", NonsensicalConfiguratorPanel.jumpFirstOnPlay?1:0);

        
        }

    }
}