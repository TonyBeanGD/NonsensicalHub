using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using NonsensicalKit.Utility;
using System.Linq;
using System.Reflection;

/// <summary>
/// 生成模型配置文件
/// </summary>
public class ConfigManager : EditorWindow
{
    static string showText; //显示给用户的文本

    [MenuItem("Tools/UpdateConfigFileTest")]
    static void CreateConfigFile()
    {
        showText = "这是一条文本";

        GameObject[] roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();//Selection.gameObjects;

        if (roots.Length == 0)
        {
            showText = "场景中没有任何对象";
            EditorWindow.GetWindow(typeof(ConfigManager));
            return;
        }

        SuperConfigData configArgs = new SuperConfigData();

        foreach (GameObject item in roots)
        {
            configArgs.ConfigData.Add(item.name, GetTList(item.transform));
        }

        FileHelper.WriteTxt(Path.Combine(Application.streamingAssetsPath, "Json", "SuperConfigs.json"), JsonConvert.SerializeObject(configArgs, Formatting.Indented));

        showText = "生成配置文件完毕";
        EditorWindow.GetWindow(typeof(ConfigManager));

        AssetDatabase.SaveAssets();
    }


    private static Dictionary<string, Dictionary<string, string>> GetTList(Transform topNode)
    {
        Dictionary<string, Dictionary<string, string>> keyValuePairs = new Dictionary<string, Dictionary<string, string>>();

        Queue<Transform> nodes = new Queue<Transform>();
        Queue<string> nodePaths = new Queue<string>();

        nodes.Enqueue(topNode);
        nodePaths.Enqueue("");

        while (nodes.Count > 0)
        {
            Transform crtNode = nodes.Dequeue();
            string crtNodePaths = nodePaths.Dequeue();

            Dictionary<string, string> temp = GetNodeInfo(crtNode);
            if (temp != null)
            {
                keyValuePairs.Add(crtNodePaths, temp);
            }

            for (int i = 0; i < crtNode.childCount; i++)
            {
                nodes.Enqueue(crtNode.GetChild(i));
                if (crtNodePaths == "")
                {
                    nodePaths.Enqueue(i.ToString());
                }
                else
                {
                    nodePaths.Enqueue(crtNodePaths + "|" + i);
                }
            }
        }

        return keyValuePairs;
    }

    private static Dictionary<string, string> GetNodeInfo(Transform crtNode)
    {
        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        MonoBehaviour[] monoBehaviours = crtNode.GetComponents<MonoBehaviour>();

        foreach (var item in monoBehaviours)
        {
            if (!item)
            {
                continue;
            }
            //挂载多个相同脚本时只会记录同一节点上第一个脚本
            if (keyValuePairs.ContainsKey(item.GetType().ToString()) == false)
            {
                var types = item.GetType().GetInterfaces();
                System.Type useType = null;
                foreach (var type in types)
                {
                    if (type.IsGenericType&& type.GetGenericTypeDefinition() == typeof(IUseArgsClass<>))
                    {
                        useType = type;
                        break;
                    }
                }
                if (useType != null)
                {

                    MethodInfo unsubMethod = useType.GetMethod("GetArgs");
                    object args = unsubMethod.Invoke(item, null);

                   string str= JsonConvert.SerializeObject(args);
                    keyValuePairs.Add(item.GetType().ToString(), str);
                }
            }
        }

        if (keyValuePairs.Count == 0)
        {
            return null;
        }
        else
        {
            return keyValuePairs;
        }
    }

    void OnGUI()
    {
        EditorGUILayout.HelpBox(showText, MessageType.Info);
    }
}
