using System.IO;
using UnityEditor;
using UnityEngine;


namespace NonsensicalKit.Editor
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
            buildPath = PlayerPrefs.GetString("tb_assestBundleAuxiliaryTool_buildPath", Path.Combine(Application.dataPath,"Editor","AssetBundles"));
            buildPath = EditorGUILayout.TextField("目标文件夹路径：", buildPath);
            PlayerPrefs.SetString("tb_assestBundleAuxiliaryTool_buildPath", buildPath);

            buildOption = (BuildAssetBundleOptions)PlayerPrefs.GetInt("tb_assestBundleAuxiliaryTool_buildOption", 0);
            buildOption = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("打包类型：", buildOption);
            PlayerPrefs.SetInt("tb_assestBundleAuxiliaryTool_buildOption", (int)buildOption);

            buildTarget = (BuildTarget)PlayerPrefs.GetInt("tb_assestBundleAuxiliaryTool_buildTarget", -2);
            buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("目标平台：", buildTarget);
            PlayerPrefs.SetInt("tb_assestBundleAuxiliaryTool_buildTarget", (int)buildTarget);

            if (GUILayout.Button("打包"))
            {
                if (Directory.Exists(buildPath)==false)
                {
                    Directory.CreateDirectory(buildPath);
                }

                BuildPipeline.BuildAssetBundles(buildPath, buildOption, buildTarget);

                AssetDatabase.Refresh();
                Debug.Log("打包完成");
            }

            if (GUILayout.Button("ClearCache"))
            {
                Caching.ClearCache();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("设置包名"))
            {
                CheckFileSystemInfo();

                Debug.Log("设置完成");
            }
        }

        public static void CheckFileSystemInfo()  //检查目标目录下的文件系统
        {
            AssetDatabase.RemoveUnusedAssetBundleNames(); //移除没有用的assetbundlename
            Object obj = Selection.activeObject;    // Selection.activeObject 返回选择的物体
            string path = AssetDatabase.GetAssetPath(obj);//选中的文件夹
            CoutineCheck(path);
        }

        public static void CheckFileOrDirectory(FileSystemInfo fileSystemInfo, string path) //判断是文件还是文件夹
        {
            FileInfo fileInfo = fileSystemInfo as FileInfo;
            if (fileInfo != null)
            {
                SetBundleName(path);
            }
            else
            {
                CoutineCheck(path);
            }
        }

        public static void CoutineCheck(string path)   //是文件夹，继续向下
        {
            DirectoryInfo directory = new DirectoryInfo(@path);
            FileSystemInfo[] fileSystemInfos = directory.GetFileSystemInfos();

            foreach (var item in fileSystemInfos)
            {
                // Debug.Log(item);
                int idx = item.ToString().LastIndexOf(@"\");//得到最后一个'\'的索引
                string name = item.ToString().Substring(idx + 1);//截取后面的作为名称

                if (!name.Contains(".meta"))
                {
                    CheckFileOrDirectory(item, path + "/" + name);  //item  文件系统，加相对路径
                }
            }
        }

        public static void SetBundleName(string path)  //设置assetbundle名字
        {
            var importer = AssetImporter.GetAtPath(path);
            string[] strs = path.Split('.');
            string[] dictors = strs[0].Split('/');
            string name = "";

             name = dictors[dictors.Length - 1];

            //for (int i = 1; i < dictors.Length; i++)
            //{
            //    if (i < dictors.Length - 1)
            //    {
            //        name += dictors[i] + "/";
            //    }
            //    else
            //    {
            //        name += dictors[i];
            //    }
            //}
            if (importer != null)
            {
                importer.assetBundleName = name;
                importer.assetBundleVariant = "assetBundle";
            }
            else
                Debug.Log("importer是空的");
        }
    }
}
