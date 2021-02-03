using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NonsensicalKit.Editor
{
    public static class TBTManager
    {
        public static GameObject[] selectGameObjects;
        public static Transform selectTransform;
        public static Action selectChanged;
        public static Action EditorUpdate;

        [InitializeOnLoadMethod]
        private static void App()
        {
            EditorApplication.update += () =>
            {
                EditorUpdate?.Invoke();
            };

            Selection.selectionChanged += () =>
            {
                if (Selection.gameObjects.Length < 1)
                {
                    selectGameObjects = new GameObject[0];
                    selectTransform = null;
                }
                else
                {
                    selectGameObjects = Selection.gameObjects;
                    selectTransform = selectGameObjects[0].transform;
                }

                selectChanged?.Invoke();
            };
        }
    }

    public class Lab
    {
        [MenuItem("TBTools/Items/清空图片")]
        private static void DeleteAllImages()
        {
            DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath + @"/images");

            foreach (var item in di.GetFiles())
            {
                File.Delete(item.FullName);
            }
        }

        [MenuItem("TBTools/Items/预制体应用")]
        private static void SaveAssets()
        {
            GameObject[] roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            AssetDatabase.SaveAssets();
            foreach (var item in roots)
            {
                ApplyPrefab(item);
            }

            Debug.Log("所有根节点预制体应用完毕");
        }

        /// <summary>
        /// 应用对象预制体（Apply）
        /// </summary>
        /// <param name="obj"></param>
        private static void ApplyPrefab(GameObject obj)
        {
            PrefabType type = PrefabUtility.GetPrefabType((UnityEngine.Object)obj);

            if (type != PrefabType.PrefabInstance)
            {
                return;
            }

            if (obj != null)
            {
                UnityEngine.Object prefabAsset = PrefabUtility.GetPrefabParent(obj);

                if (prefabAsset != null)
                {
                    PrefabUtility.ReplacePrefab(obj, prefabAsset, ReplacePrefabOptions.ConnectToPrefab);
                }
            }
        }

        [MenuItem("TBTools/Items/检测资源重名")]
        private static bool CheckResoureDuplicateName()
        {
            List<string> duplicateNameInfo = new List<string>();

            HashSet<string> vs = new HashSet<string>();

            Queue<DirectoryInfo> directoryInfos = new Queue<DirectoryInfo>();

            DirectoryInfo di = new DirectoryInfo(Application.dataPath + @"/Resources");

            directoryInfos.Enqueue(di);

            int leftCount = 1;

            while (leftCount > 0)
            {
                DirectoryInfo directoryInfo = directoryInfos.Dequeue();
                leftCount--;

                foreach (FileInfo item in directoryInfo.GetFiles())
                {
                    if (vs.Add(item.Name) == false)
                    {
                        duplicateNameInfo.Add(item.FullName);
                    }
                }

                foreach (DirectoryInfo item in directoryInfo.GetDirectories())
                {
                    directoryInfos.Enqueue(item);
                    leftCount++;
                }
            }

            foreach (var item in duplicateNameInfo)
            {
                Debug.Log($"资源重名：{item}");
            }

            if (duplicateNameInfo.Count == 0)
            {
                Debug.Log("无资源重名");
                return false;
            }

            return true;
        }

        [MenuItem("TBTools/Items/刷新项目文件")]
        private static void RefeshAsset()
        {
            AssetDatabase.Refresh();
            Debug.Log("刷新完成");
        }

        [MenuItem("TBTools/Items/根据名称排序")]
        private static void NameSort()
        {
            GameObject[] roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

            for (int i = 0; i < roots.Length - 1; i++)
            {
                for (int j = i + 1; j < roots.Length; j++)
                {
                    if (string.Compare(roots[i].name, roots[j].name) > 0)
                    {
                        GameObject temp = roots[i];
                        roots[i] = roots[j];
                        roots[j] = temp;
                    }
                }
            }

            foreach (var item in roots)
            {
                item.transform.SetAsLastSibling();
            }

            Debug.Log("排序完成");
        }

    }
}