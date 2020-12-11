using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using LitJson;
using System.Text;
using System;
using System.Text.RegularExpressions;
using System.Reflection;

namespace NonsensicalKit.Editor
{
    public class NodeBuilder : EditorWindow
    {
        public static readonly string ClassTemplate =
@"using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class #类名# : MonoBehaviour
{    
    #region AUTO_NodeBuilderVariable
	    
    #endregion AUTO_NodeBuilderVariable

    #region AUTO_NodeBuilderAttributes
	    
    #endregion AUTO_NodeBuilderAttributes

    private void OnAwake()
    {
        #region AUTO_NodeBuilderAwake
	        
        #endregion AUTO_NodeBuilderAwake
    }
}
";

        private static EditorWindow window;

        private static readonly string defaultScriptPath = @"Scripts/NodeManager";

        private static string scriptPath;

        private static bool autoPath;

        private static bool autoMount;

        private static ClassBuilder classBuilder;

        private static Vector2 tablePos;

        private static Vector2 viewPos;

        private static string className
        {
            get
            {
                return TBTManager.GetFileName(scriptPath);
            }
        }

        private static string jsonFilePath
        {
            get
            {
                return Path.Combine(Application.dataPath, "Editor", "TBTools", "SaveData", className + ".json");
            }
        }

        private static string scriptFullPath
        {
            get
            {
                return Path.Combine(Application.dataPath, scriptPath + ".cs");
            }
        }

        [MenuItem("TBTools/代码生成/节点管理代码生成",false,101)]
        private static void ShowWindow()
        {
            CreateClassBuilder();

            scriptPath = EditorPrefs.GetString("tb_nodeBuilder_lastScriptPath", defaultScriptPath);

            window = EditorWindow.GetWindow(typeof(NodeBuilder));
        }

        private static void CreateClassBuilder()
        {
            string raw;

            if (File.Exists(scriptPath) == true)
            {
                FileStream originFile = new FileStream(scriptPath, FileMode.Open);
                StreamReader read = new StreamReader(originFile);
                raw = read.ReadToEnd();
                read.Close();
                originFile.Close();
                File.Delete(scriptPath);
            }
            else
            {
                string classStrTemp = ClassTemplate;
                raw = classStrTemp.Replace("#类名#", className);
            }

            if (TBTManager.selectTransform != null)
            {

                if (File.Exists(jsonFilePath))
                {
                    if (File.Exists(scriptFullPath) == true)
                    {
                        string jsonData = File.ReadAllText(jsonFilePath);
                        string scriptData = File.ReadAllText(scriptFullPath);
                        classBuilder = new ClassBuilder(scriptData, jsonData);
                    }
                    else
                    {
                        File.Delete(jsonFilePath);
                        AssetDatabase.Refresh();
                        AssetDatabase.SaveAssets();
                        classBuilder = new ClassBuilder(raw);
                    }
                }
                else
                {
                    classBuilder = new ClassBuilder(raw);
                }
            }
        }

        [InitializeOnLoadMethod]
        private static void App()
        {
            scriptPath = EditorPrefs.GetString("tb_nodeBuilder_lastScriptPath", defaultScriptPath);

            TBTManager.selectChanged += () =>
            {
                if (window != null)
                {

                    if (TBTManager.selectTransform == null)
                    {
                        EditorPrefs.SetString("tb_nodeBuilder_lastScriptPath", defaultScriptPath);
                    }
                    else
                    {
                        EditorPrefs.SetString("tb_nodeBuilder_lastScriptPath", Path.Combine(@"Scripts", $"{TBTManager.selectTransform.name}Manager"));
                    }

                    if (autoPath == true)
                    {
                        scriptPath = EditorPrefs.GetString("tb_nodeBuilder_lastScriptPath", defaultScriptPath);
                    }

                    CreateClassBuilder();
                    window.Repaint();
                }
            };
        }

        private void OnGUI()
        {
            if (classBuilder==null)
            {
                CreateClassBuilder();
            }

            EditorGUILayout.BeginHorizontal();
            DrawLeft();
            DrawRight();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawLeft()
        {
            EditorGUILayout.BeginVertical();

            autoPath = EditorPrefs.GetBool("tb_nodeBuilder_manualPath", true);
            autoPath = EditorGUILayout.Toggle("自动路径", autoPath);
            EditorPrefs.SetBool("tb_nodeBuilder_manualPath", autoPath);

            {
                GUI.enabled = !autoPath;

                if (autoPath == true)
                {
                    scriptPath = EditorPrefs.GetString("tb_nodeBuilder_lastScriptPath", defaultScriptPath);
                }
                scriptPath = EditorGUILayout.TextField("生成脚本路径：", scriptPath);
                EditorPrefs.SetString("tb_nodeBuilder_lastScriptPath", scriptPath);

                GUI.enabled = true;
            }

            EditorGUILayout.Space();

            {
                GUI.enabled = TBTManager.selectTransform != null;

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("生成/更新脚本"))
                {
                    string jsonData = JsonMapper.ToJson(classBuilder.rootNode);

                    File.WriteAllText(jsonFilePath, jsonData, Encoding.UTF8);

                    File.WriteAllText(scriptFullPath, classBuilder.ToString(), Encoding.UTF8);

                    AssetDatabase.Refresh();
                }

                if (GUILayout.Button("挂载脚本"))
                {
                    MountScript();
                    AssetDatabase.Refresh();
                }

                EditorGUILayout.EndHorizontal();

                GUI.enabled = true;
            }

            EditorGUILayout.Space();

            DrawTable();

            EditorGUILayout.EndVertical();
        }

        public void MountScript()
        {
            if (TBTManager.selectTransform == false)
            {
                return;
            }

            var scriptType = TBTManager.GetAssembly().GetType(className);
            if (scriptType == null)
            {
                Debug.LogWarning($"脚本{className}不存在");
            }

            var target = TBTManager.selectTransform.GetComponent(scriptType);

            if (target == null)
            {
                target = TBTManager.selectTransform.gameObject.AddComponent(scriptType);
            }


            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            var infos = classBuilder.GetFieldInfos();


            foreach (var info in infos)
            {
                if (info.type == "GameObject")
                {
                    scriptType.InvokeMember(info.name,
                             BindingFlags.SetField |
                             BindingFlags.Instance |
                             BindingFlags.NonPublic,
                             null, target, new object[] { TBTManager.selectTransform.Find(info.path).gameObject }, null, null, null);
                }
                else if (info.type == "Transform")
                {
                    scriptType.InvokeMember(info.name,
                           BindingFlags.SetField |
                           BindingFlags.Instance |
                           BindingFlags.NonPublic,
                           null, target, new object[] { TBTManager.selectTransform.Find(info.path).transform }, null, null, null);
                }
                else
                {
                    scriptType.InvokeMember(info.name,
                              BindingFlags.SetField |
                              BindingFlags.Instance |
                              BindingFlags.NonPublic,
                              null, target, new object[] { TBTManager.selectTransform.Find(info.path).GetComponent(info.type) }, null, null, null);
                }
            }


            var obj = PrefabUtility.GetPrefabParent(TBTManager.selectTransform.gameObject);
            if (obj != null)
            {
                PrefabUtility.ReplacePrefab(TBTManager.selectTransform.gameObject, obj, ReplacePrefabOptions.ConnectToPrefab);
            }


            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private void DrawTable()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(" 全选变量 ")) classBuilder.VariableSwitch(true);
                if (GUILayout.Button(" 全选属性 ")) classBuilder.AttributesSwitch(true);
                if (GUILayout.Button(" 全选事件 ")) classBuilder.EventSwitch(true);
                if (GUILayout.Button("全查找赋值")) classBuilder.FindSwitch(true);
                if (GUILayout.Button("  全展开  ")) classBuilder.OpenSwitch(true);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("全取消变量")) classBuilder.VariableSwitch(false);
                if (GUILayout.Button("全取消属性")) classBuilder.AttributesSwitch(false);
                if (GUILayout.Button("全取消事件")) classBuilder.EventSwitch(false);
                if (GUILayout.Button("全取消查找")) classBuilder.FindSwitch(false);
                if (GUILayout.Button("  全折叠  ")) classBuilder.OpenSwitch(false);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            tablePos = EditorGUILayout.BeginScrollView(tablePos);
            {
                if (TBTManager.selectTransform != null)
                {
                    classBuilder.DrawTable(classBuilder.rootNode);
                }
            }
            EditorGUILayout.EndScrollView();
        }


        private void DrawRight()
        {
            EditorGUILayout.BeginVertical();
            {
                var rect = EditorGUILayout.GetControlRect();
                rect.height = 35;
                GUI.Box(rect, "代码预览", "GroupBox");
                GUILayout.Space(20);

                {
                    viewPos = EditorGUILayout.BeginScrollView(viewPos, GUILayout.Width(position.width * 0.5f));
                    {
                        if (TBTManager.selectTransform != null)
                        {
                            var str = classBuilder.ToString();
                            var array = TBTManager.GetStringList(str);
                            EditorGUILayout.BeginVertical();
                            {
                                foreach (var item in array)
                                {
                                    GUILayout.Label(item);
                                }
                            }
                            EditorGUILayout.EndVertical();

                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private class ClassBuilder
        {

            private string raw;

            public NodeInfo rootNode;

            private StringBuilder scriptBuilder;

            private List<FieldInfo> fieldInfos;

            public ClassBuilder()
            {

            }

            public ClassBuilder(string raw)
            {
                this.raw = raw;
                rootNode = new NodeInfo(TBTManager.selectTransform, string.Empty);
            }

            public ClassBuilder(string raw, string jsonText)
            {
                this.raw = raw;
                rootNode = JsonMapper.ToObject<NodeInfo>(jsonText);
            }

            public List<FieldInfo> GetFieldInfos()
            {
                fieldInfos = new List<FieldInfo>();

                AddFieldInfo(rootNode);

                return fieldInfos;
            }

            private void AddFieldInfo(NodeInfo nodeInfo)
            {
                if (nodeInfo.useVariable == true && nodeInfo.useFind == false)
                {
                    fieldInfos.Add(new FieldInfo(nodeInfo.name, nodeInfo.components[nodeInfo.choiceComponent], nodeInfo.path));
                }

                foreach (var item in nodeInfo.childs)
                {
                    AddFieldInfo(item);
                }
            }

            public override string ToString()
            {
                scriptBuilder = new StringBuilder();

                ScriptVariavleBuilder(rootNode);

                raw = Regex.Replace(raw, "#region AUTO_NodeBuilderVariable([\\s\\S]*?)#endregion AUTO_NodeBuilderVariable", $"#region AUTO_NodeBuilderVariable\n\n{scriptBuilder.ToString()}\n    #endregion AUTO_NodeBuilderVariable");

                scriptBuilder = new StringBuilder();

                ScriptAttributesBuilder(rootNode);

                raw = Regex.Replace(raw, "#region AUTO_NodeBuilderAttributes([\\s\\S]*?)#endregion AUTO_NodeBuilderAttributes", $"#region AUTO_NodeBuilderAttributes\n\n{scriptBuilder.ToString()}\n    #endregion AUTO_NodeBuilderAttributes"); 
                
                scriptBuilder = new StringBuilder();

                ScriptAwakeBuilder(rootNode);

                raw = Regex.Replace(raw, "#region AUTO_NodeBuilderAwake([\\s\\S]*?)#endregion AUTO_NodeBuilderAwake", $"#region AUTO_NodeBuilderAwake\n\n{scriptBuilder.ToString()}\n    #endregion AUTO_NodeBuilderAwake");

                return raw;
            }

            private void ScriptVariavleBuilder(NodeInfo nodeInfo)
            {
                scriptBuilder.Append(nodeInfo.GetVariableString());

                foreach (var item in nodeInfo.childs)
                {
                    ScriptVariavleBuilder(item);
                }
            }

            private void ScriptAttributesBuilder(NodeInfo nodeInfo)
            {
                scriptBuilder.Append(nodeInfo.GetAttributesString());

                foreach (var item in nodeInfo.childs)
                {
                    ScriptAttributesBuilder(item);
                }
            }

            private void ScriptAwakeBuilder(NodeInfo nodeInfo)
            {
                scriptBuilder.Append(nodeInfo.GetAwakeString());

                foreach (var item in nodeInfo.childs)
                {
                    ScriptAwakeBuilder(item);
                }
            }

            public void DrawTable(NodeInfo nodeInfo, int depth = 0)
            {
                nodeInfo.DrawLine(depth);

                if (nodeInfo.isOpen == true)
                {
                    foreach (var item in nodeInfo.childs)
                    {
                        DrawTable(item, depth + 1);
                    }
                }
            }

            public void VariableSwitch(bool isOn)
            {
                Queue<NodeInfo> nodeInfos = new Queue<NodeInfo>();
                nodeInfos.Enqueue(rootNode);

                while (nodeInfos.Count > 0)
                {
                    NodeInfo nodeInfo = nodeInfos.Dequeue();
                    nodeInfo.useVariable = isOn;

                    foreach (var item in nodeInfo.childs)
                    {
                        nodeInfos.Enqueue(item);
                    }
                }
            }

            public void AttributesSwitch(bool isOn)
            {
                Queue<NodeInfo> nodeInfos = new Queue<NodeInfo>();
                nodeInfos.Enqueue(rootNode);

                while (nodeInfos.Count > 0)
                {
                    NodeInfo nodeInfo = nodeInfos.Dequeue();
                    nodeInfo.useAttributes = isOn;

                    foreach (var item in nodeInfo.childs)
                    {
                        nodeInfos.Enqueue(item);
                    }
                }
            }

            public void EventSwitch(bool isOn)
            {
                Queue<NodeInfo> nodeInfos = new Queue<NodeInfo>();
                nodeInfos.Enqueue(rootNode);

                while (nodeInfos.Count > 0)
                {
                    NodeInfo nodeInfo = nodeInfos.Dequeue();
                    nodeInfo.useEvent = isOn;

                    foreach (var item in nodeInfo.childs)
                    {
                        nodeInfos.Enqueue(item);
                    }
                }
            }

            public void OpenSwitch(bool isOn)
            {
                Queue<NodeInfo> nodeInfos = new Queue<NodeInfo>();
                nodeInfos.Enqueue(rootNode);

                while (nodeInfos.Count > 0)
                {
                    NodeInfo nodeInfo = nodeInfos.Dequeue();
                    nodeInfo.isOpen = isOn;

                    foreach (var item in nodeInfo.childs)
                    {
                        nodeInfos.Enqueue(item);
                    }
                }
            }

            public void FindSwitch(bool isOn)
            {
                Queue<NodeInfo> nodeInfos = new Queue<NodeInfo>();
                nodeInfos.Enqueue(rootNode);

                while (nodeInfos.Count > 0)
                {
                    NodeInfo nodeInfo = nodeInfos.Dequeue();
                    nodeInfo.useFind = isOn;
                    nodeInfo.useFind = isOn;

                    foreach (var item in nodeInfo.childs)
                    {
                        nodeInfos.Enqueue(item);
                    }
                }
            }

            public class NodeInfo
            {
                public string name;

                public bool isOpen;

                public bool useVariable;

                public bool useAttributes;

                private bool canUseEvent;

                public bool useEvent;

                public bool useFind;

                public string[] components;

                public int choiceComponent;

                public string path;

                public List<NodeInfo> childs;

                public NodeInfo()
                {

                }

                public NodeInfo(Transform t, string path)
                {
                    isOpen = true;
                    name = t.name;
                    this.path = path;
                    components = TBTManager.GetComponentsName(t);
                    CheckAbbreviation();
                    ChangeType();


                    childs = new List<NodeInfo>();

                    for (int i = 0; i < t.childCount; i++)
                    {
                        childs.Add(new NodeInfo(t.GetChild(i), path + t.GetChild(i).name + "/"));
                    }
                }

                public void DrawLine(int depth)
                {
                    var rect = EditorGUILayout.BeginHorizontal();
                    {
                        if (useVariable) EditorGUI.DrawRect(rect, new Color(0, 0.5f, 0, 0.3f));

                        useVariable = EditorGUILayout.ToggleLeft("变量", useVariable, GUILayout.Width(50));

                        if (useVariable == false)
                        {
                            useAttributes = false;
                            useEvent = false;
                            useFind = false;
                        }

                        if (canUseEvent == false)
                        {
                            useEvent = false;
                        }

                        {
                            GUI.enabled = useVariable;
                            useAttributes = EditorGUILayout.ToggleLeft("属性", useAttributes, GUILayout.Width(50));

                            GUI.enabled = useVariable ? canUseEvent : false;
                            useEvent = EditorGUILayout.ToggleLeft("事件", useEvent, GUILayout.Width(50));

                            GUI.enabled = useVariable;

                            useFind = EditorGUILayout.ToggleLeft("查找", useFind, GUILayout.Width(50));

                            GUI.enabled = true;
                        }

                        int oldIndex = choiceComponent;
                        choiceComponent = EditorGUILayout.Popup(choiceComponent, components, GUILayout.Width(100));
                        if (oldIndex != choiceComponent)
                        {
                            ChangeType();
                        }

                        GUILayout.Space(depth * 20);
                        if (childs.Count > 0)
                        {
                            isOpen = EditorGUILayout.Foldout(isOpen, name, true);
                        }
                        else
                        {
                            EditorGUILayout.LabelField(name);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                }

                private void ChangeType()
                {
                    string componentName = components[choiceComponent];
                    if (componentName == "Button" || componentName == "InputField" ||
                        componentName == "ScrollRect" || componentName == "Dropdown" ||
                        componentName == "Scrollbar" || componentName == "Slider" ||
                        componentName == "Toggle")
                    {
                        canUseEvent = true;
                    }
                    else
                    {
                        canUseEvent = false;
                    }
                }

                private static readonly string[,] abbreviations = new string[,]
                {
                    {"img","Image"},
                    {"btn","Button"},
                    {"aud","AudioClip"},
                    {"ipf","InputField"},
                    {"txt","Text"}
                };

                /// <summary>
                /// 检查缩写
                /// </summary>
                public void CheckAbbreviation()
                {
                    string[] _rawNameTemp = name.Split(new char[] { '_' });

                    for (int i = 0; i < _rawNameTemp.Length - 1; i++)
                    {
                        string crtPart = _rawNameTemp[i];

                        for (int j = 0; j < abbreviations.GetLength(0); j++)
                        {
                            if (crtPart.Equals(abbreviations[j, 0]))
                            {
                                for (int k = 0; k < components.Length; k++)
                                {
                                    if (components[k] == abbreviations[j, 1])
                                    {
                                        choiceComponent = k;
                                    }
                                }
                            }
                        }
                    }
                }

                public string GetVariableString()
                {
                    if (useVariable == true)
                    {
                        StringBuilder sb = new StringBuilder();
                        if (useFind == false)
                        {
                            sb.Append("    [SerializeField]\n");
                        }
                        sb.Append("    private ");
                        sb.Append(components[choiceComponent]);
                        sb.Append(" ");
                        sb.Append(name);
                        sb.Append(";\n");
                        return sb.ToString();
                    }

                    return string.Empty;
                }

                public string GetAttributesString()
                {
                    if (useAttributes == true)
                    {

                    }

                    return string.Empty;
                }
                
                public string GetAwakeString()
                {
                    if (useFind == true)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("        ");
                        sb.Append(name);
                        sb.Append(" = ");
                        if (path==string.Empty)
                        {
                            sb.Append("transform");
                            sb.Append(TBTManager.GetComponentString(components[choiceComponent]));
                            sb.Append(";\n");
                        }
                        else
                        {
                            sb.Append(classBuilder.rootNode.name);
                            sb.Append(".transform.Find(\"");
                            sb.Append(path);
                            sb.Append("\")");
                            sb.Append(TBTManager.GetComponentString(components[choiceComponent]));
                            sb.Append(";\n");
                        }
                        return sb.ToString();
                    }
                    return string.Empty;
                }
            }
        }


        public class FieldInfo
        {
            public string name;
            public string type;
            public string path;

            public FieldInfo(string name, string type, string path)
            {
                this.name = name;
                this.type = type;
                this.path = path;
            }
        }
    }
}
