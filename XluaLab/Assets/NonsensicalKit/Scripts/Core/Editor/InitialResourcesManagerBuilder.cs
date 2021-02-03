using NonsensicalKit.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEngine;


namespace NonsensicalKit.Editor
{
    public class InitialResourcesManagerBuilder : EditorWindow
    {
        private class InitialResourcesManagerTemplate
        {
            public static readonly string ClassTemplate =
@"using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class #类名# : MonoBehaviour
{
    #region ResourceBuilderAttributes
	
    #endregion ResourceBuilderAttributes

    private void Awake()
	{
        #region ResourceBuilderAwake
		
        #endregion ResourceBuilderAwake
	}
}
";
        }

        [MenuItem("TBTools/代码生成/初始资源管理代码生成",false,101)]
        private static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(InitialResourcesManagerBuilder));
        }

        public enum LoadType
        {
            LoadOnAwake,
            LoadOnUse,
            LoadOnFirstUse,
        }

        private static class InitialResourcesManagerBuilderPanel
        {
            public static readonly string targetDirDefaultPath = @"Resources/";
            public static readonly string scriptDefaultPath = @"Scripts/InitialResourcesManager";


            public static string targetDirPath;
            public static string scriptPath;
          
            public static LoadType loadType = LoadType.LoadOnAwake;

            #region 待添加功能

            /// <summary>
            /// 是否使用特性
            /// </summary>
            public static bool checkCharacteristic = true;
            /// <summary>
            /// 是否使用缩略
            /// </summary>
            public static bool checkAbbreviations = true;

            #endregion
        }

        private void OnGUI()
        {
            InitialResourcesManagerBuilderPanel.targetDirPath = EditorPrefs.GetString("tb_resourceBuilder_lastDirPath", InitialResourcesManagerBuilderPanel.targetDirDefaultPath);
            InitialResourcesManagerBuilderPanel.targetDirPath = EditorGUILayout.TextField("目标文件夹路径：", InitialResourcesManagerBuilderPanel.targetDirPath);
            EditorPrefs.SetString("tb_resourceBuilder_lastDirPath", InitialResourcesManagerBuilderPanel.targetDirPath);

            EditorGUILayout.Space();

            InitialResourcesManagerBuilderPanel.scriptPath = EditorPrefs.GetString("tb_resourceBuilder_lastScriptPath", InitialResourcesManagerBuilderPanel.scriptDefaultPath);
            InitialResourcesManagerBuilderPanel.scriptPath = EditorGUILayout.TextField("生成脚本路径：", InitialResourcesManagerBuilderPanel.scriptPath);
            EditorPrefs.SetString("tb_resourceBuilder_lastScriptPath", InitialResourcesManagerBuilderPanel.scriptPath);

            EditorGUILayout.Space();

            InitialResourcesManagerBuilderPanel.loadType = (LoadType)EditorPrefs.GetInt("tb_resourceBuilder_loadType", 0);
            InitialResourcesManagerBuilderPanel.loadType = (LoadType)EditorGUILayout.EnumPopup("LoadType", InitialResourcesManagerBuilderPanel.loadType, GUILayout.MinWidth(100f));
            EditorPrefs.SetInt("tb_resourceBuilder_loadType", (int)InitialResourcesManagerBuilderPanel.loadType);


            EditorGUILayout.Space();

            if (GUILayout.Button("生成/更新脚本"))
            {
                InitialResourcesManagerBuilderPanel.targetDirPath = Path.Combine(Application.dataPath, InitialResourcesManagerBuilderPanel.targetDirPath);
                if (Directory.Exists(InitialResourcesManagerBuilderPanel.targetDirPath) == true)
                {

                    InitialResourcesManagerBuilderPanel.scriptPath = Path.Combine(Application.dataPath, InitialResourcesManagerBuilderPanel.scriptPath);
                    BuildClass();
                }
                else
                {
                    Debug.LogWarning($"目标文件夹\"{InitialResourcesManagerBuilderPanel.targetDirPath}\"不存在");
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("挂载脚本"))
            {
                GameObject go = Selection.gameObjects[0];
                if (go==null)
                {
                    return;
                }

                var scriptType = ReflectionHelper.GetAssembly().GetType(StringHelper.GetFileNameByPath(InitialResourcesManagerBuilderPanel.scriptPath));
                if (scriptType == null)
                {
                    Debug.LogWarning($"脚本{scriptType.Name}不存在");
                }

                var target = go.GetComponent(scriptType);
                if (target == null)
                {
                    go.AddComponent(scriptType);
                }
                else
                {
                    Debug.Log("脚本已存在");
                }

                Debug.Log("脚本挂载成功");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

        }


        private void BuildClass()
        {
            ClassBuilder cb = new ClassBuilder();

            Queue<DirectoryInfo> leftoverDirInfo = new Queue<DirectoryInfo>();
            leftoverDirInfo.Enqueue(new DirectoryInfo(InitialResourcesManagerBuilderPanel.targetDirPath));
            int leftoverCount = 1;
            while (leftoverCount > 0)
            {
                DirectoryInfo crt = leftoverDirInfo.Dequeue();
                leftoverCount--;

                foreach (var item in crt.GetFiles())
                {
                    cb.AddElement(item);
                }

                foreach (var item in crt.GetDirectories())
                {
                    leftoverDirInfo.Enqueue(item);
                    leftoverCount++;
                }
            }

            string[] classNameTemp = InitialResourcesManagerBuilderPanel.scriptPath.Split(new char[] { '/', '\\' });
            string className = classNameTemp[classNameTemp.Length - 1];

            string raw;

            InitialResourcesManagerBuilderPanel.scriptPath += @".cs";

            if (File.Exists(InitialResourcesManagerBuilderPanel.scriptPath) == true)
            {
                FileStream originFile = new FileStream(InitialResourcesManagerBuilderPanel.scriptPath, FileMode.Open);
                StreamReader read = new StreamReader(originFile);
                raw = read.ReadToEnd();
                read.Close();
                originFile.Close();
                File.Delete(InitialResourcesManagerBuilderPanel.scriptPath);
            }
            else
            {
                string classStrTemp = InitialResourcesManagerTemplate.ClassTemplate;
                raw = classStrTemp.Replace("#类名#", className);
            }

            FileStream file = new FileStream(InitialResourcesManagerBuilderPanel.scriptPath, FileMode.CreateNew);
            StreamWriter write = new StreamWriter(file, Encoding.UTF8);
            write.Write(cb.BuildString(raw));
            write.Flush();
            write.Close();
            file.Close();

            Debug.Log($"自动生成脚本{InitialResourcesManagerBuilderPanel.scriptPath}成功!");

        }

        public class AbbreviationAndCharacteristic
        {
            public enum Characteristic
            {
                Base,
                Group,
            }

            private static readonly string defaultType = "GameObject";

            private static readonly string[,] abbreviations = new string[,]
            {
                {"img","Sprite"},
                {"aud","AudioClip"},
                {"txt","TextAsset"}
            };

            private static readonly string[] characteristics = new string[]
            {
                "gr"
            };

            public static string CheckAbbreviationAndCharacteristic(string _rawName, out Characteristic _characteristic)
            {
                _characteristic = Characteristic.Base;

                string _type = defaultType;

                string[] _rawNameTemp = _rawName.Split(new char[] { '_' });

                for (int i = 0; i < _rawNameTemp.Length - 1; i++)
                {
                    string crtPart = _rawNameTemp[i];
                    for (int j = 0; j < characteristics.Length; j++)
                    {
                        if (crtPart.Equals(characteristics[j]))
                        {
                            _characteristic |= (Characteristic)j + 1;
                        }
                    }
                    for (int j = 0; j < abbreviations.GetLength(0); j++)
                    {
                        if (crtPart.Equals(abbreviations[j, 0]))
                        {
                            _type = abbreviations[j, 1];
                        }
                    }
                }

                return _type;
            }
        }


        private class ClassBuilder
        {
            List<NormalElement> Elements;

            public ClassBuilder()
            {
                Elements = new List<NormalElement>();
            }

            public void AddElement(string type, string name, string path, AbbreviationAndCharacteristic.Characteristic chara, int index = -1)
            {
                if (index != -1)
                {
                    IEnumerable<NormalElement> element = from n in Elements
                                                         where n.name.Equals(name)
                                                         select n;

                    if (element.Count() > 0)
                    {
                        GroupElement ge = (GroupElement)element.First();
                        ge.indexs.Add(index);
                        return;
                    }
                    else
                    {

                        GroupElement ge = new GroupElement();
                        ge.indexs = new List<int>();
                        ge.indexs.Add(index);
                        ge.type = type;
                        ge.name = name;
                        ge.path = path;
                        ge.chara = chara;
                        Elements.Add(ge);
                    }
                }
                else
                {
                    NormalElement ne = new NormalElement();

                    ne.type = type;
                    ne.name = name;
                    ne.path = path;
                    ne.chara = chara;

                    Elements.Add(ne);
                }
            }

            public void AddElement(FileInfo info)
            {
                string crtFileName = info.Name;
                string[] crtFileNameTemp = crtFileName.Split(new char[] { '.' });
                if (crtFileNameTemp.Length > 1)
                {
                    crtFileName = crtFileNameTemp[crtFileNameTemp.Length - 2];
                }
                else
                {
                    crtFileName = crtFileNameTemp[0];
                }

                string crtFileFullPath = info.FullName;
                if (crtFileFullPath.Length < 5 || crtFileFullPath.Substring(crtFileFullPath.Length - 5).Equals(".meta") == false)
                {
                    AbbreviationAndCharacteristic.Characteristic chara;

                    string type = AbbreviationAndCharacteristic.CheckAbbreviationAndCharacteristic(crtFileName, out chara);

                    int index = -1;

                    switch (chara)
                    {
                        case AbbreviationAndCharacteristic.Characteristic.Base:
                            break;
                        case AbbreviationAndCharacteristic.Characteristic.Group:
                            {
                                Match match = Regex.Match(crtFileName, "[0-9]+$");
                                if (match.Success == false)
                                {
                                    Debug.LogError($"{crtFileFullPath}命名不规范");
                                    return;
                                }
                                index = int.Parse(match.Value);

                                crtFileName = Regex.Match(crtFileName, ".*?(?=[0-9]+$)").Value;
                            }
                            break;
                        default:
                            Debug.LogError("Undefine Enum");
                            break;
                    }

                    string path = crtFileFullPath.Substring(InitialResourcesManagerBuilderPanel.targetDirPath.Length).Split(new char[] { '.' })[0];
                    switch (chara)
                    {
                        case AbbreviationAndCharacteristic.Characteristic.Base:
                            break;
                        case AbbreviationAndCharacteristic.Characteristic.Group:
                            {
                                path = Regex.Match(path, ".*?(?=[0-9]+$)").Value;
                            }
                            break;
                        default:
                            Debug.LogError("Undefine Enum");
                            break;
                    }

                    AddElement(type, crtFileName, path, chara, index);
                }

                return;
            }

            public string BuildString(string originStr)
            {
                StringBuilder attributesText = new StringBuilder();
                StringBuilder loadText = new StringBuilder();

                foreach (var item in Elements)
                {
                    switch (item.chara)
                    {
                        case AbbreviationAndCharacteristic.Characteristic.Group:
                            {
                                GroupElement ge = (GroupElement)item;

                                int max = ge.indexs.Max();

                                attributesText.Append("    public static ");
                                attributesText.Append(item.type);
                                attributesText.Append("[] ");
                                attributesText.Append(item.name);
                                attributesText.Append(" = new ");
                                attributesText.Append(item.type);
                                attributesText.Append("[");
                                attributesText.Append(max + 1);
                                attributesText.Append("];\n");

                                foreach (var index in ge.indexs)
                                {
                                    loadText.Append("        ");
                                    loadText.Append(item.name);
                                    loadText.Append("[");
                                    loadText.Append(index);
                                    loadText.Append("] = ");
                                    loadText.Append("Resources.Load<");
                                    loadText.Append(item.type);
                                    loadText.Append(">(\"");
                                    loadText.Append(item.path);
                                    loadText.Append(index);
                                    loadText.Append("\");\n");
                                }
                            }
                            break;
                        default:
                            {
                                attributesText.Append("    public static ");
                                attributesText.Append(item.type);
                                attributesText.Append(" ");
                                attributesText.Append(item.name);
                                switch (InitialResourcesManagerBuilderPanel.loadType)
                                {
                                    case LoadType.LoadOnAwake:
                                        attributesText.Append(";\n");

                                        loadText.Append("        ");
                                        loadText.Append(item.name);
                                        loadText.Append(" = ");
                                        loadText.Append("Resources.Load<");
                                        loadText.Append(item.type);
                                        loadText.Append(">(\"");
                                        loadText.Append(item.path);
                                        loadText.Append("\");\n");
                                        break;
                                    case LoadType.LoadOnUse:
                                        attributesText.Append("{ get { return Resources.Load<");
                                        attributesText.Append(item.type);
                                        attributesText.Append(">(\"");
                                        attributesText.Append(item.path);
                                        attributesText.Append("\"); } }\n");
                                        break;
                                    case LoadType.LoadOnFirstUse:
                                        Debug.LogWarning("功能尚未完善");
                                        break;
                                    default:
                                        Debug.LogError("Undefine Enum");
                                        break;
                                }
                                
                            }
                            break;
                    }
                }

                originStr = Regex.Replace(originStr, "#region ResourceBuilderAttributes([\\s\\S]*?)#endregion ResourceBuilderAttributes", $"#region ResourceBuilderAttributes\n\n{attributesText.ToString()}\n    #endregion ResourceBuilderAttributes");
                originStr = Regex.Replace(originStr, "#region ResourceBuilderAwake([\\s\\S]*?)#endregion ResourceBuilderAwake", $"#region ResourceBuilderAwake\n\n{loadText.ToString()}\n        #endregion ResourceBuilderAwake");

                originStr = originStr.Replace(@"\", "/");
                return originStr;
            }

            private class NormalElement
            {
                public AbbreviationAndCharacteristic.Characteristic chara;

                public string type;

                public string name;

                public string path;
            }

            private class GroupElement : NormalElement
            {
                public List<int> indexs;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                foreach (var item in Elements)
                {
                    sb.Append($"特性为{item.chara.ToString()}");
                    sb.Append($"类型为{item.type}");
                    sb.Append($"名字为{item.name}");
                    sb.Append($"路径为{item.path}");

                    if (item.chara == AbbreviationAndCharacteristic.Characteristic.Group)
                    {
                        GroupElement ge = (GroupElement)item;

                        sb.Append($"索引为{ge.indexs}");
                    }

                    sb.Append("\n");
                }

                return sb.ToString();
            }
        }
    }
}
