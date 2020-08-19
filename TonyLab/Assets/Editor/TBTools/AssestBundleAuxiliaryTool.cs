using System.IO;
using UnityEditor;
using UnityEngine;


namespace TonyBeanTools
{
    public class AssestBundleAuxiliaryTool : EditorWindow
    {
        [MenuItem("TBTools/AssestBundle辅助工具")]
        static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(AssestBundleAuxiliaryTool));
        }

        string buildPath;

        BuildAssetBundleOptions buildOption;

        BuildTarget buildTarget;

        private void OnGUI()
        {
            buildPath = EditorPrefs.GetString("tb_assestBundleAuxiliaryTool_buildPath", Path.Combine(Application.dataPath,"Editor","AssetBundles"));
            buildPath = EditorGUILayout.TextField("目标文件夹路径：", buildPath);
            EditorPrefs.SetString("tb_assestBundleAuxiliaryTool_buildPath", buildPath);

            buildOption = (BuildAssetBundleOptions)EditorPrefs.GetInt("tb_assestBundleAuxiliaryTool_buildOption", 0);
            buildOption = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("打包类型：", buildOption);
            EditorPrefs.SetInt("tb_assestBundleAuxiliaryTool_buildOption", (int)buildOption);

            buildTarget = (BuildTarget)EditorPrefs.GetInt("tb_assestBundleAuxiliaryTool_buildTarget", -2);
            buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("目标平台：", buildTarget);
            EditorPrefs.SetInt("tb_assestBundleAuxiliaryTool_buildTarget", (int)buildTarget);

            if (GUILayout.Button("打包"))
            {
                if (Directory.Exists(buildPath)==false)
                {
                    Directory.CreateDirectory(buildPath);
                }

                BuildPipeline.BuildAssetBundles(buildPath, buildOption, buildTarget);

                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("ClearCache"))
            {
                Caching.ClearCache();
                AssetDatabase.Refresh();
            }
        }
    }
}
