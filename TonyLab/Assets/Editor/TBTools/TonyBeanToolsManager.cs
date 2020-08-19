using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

/*  TODO:自己写一个Resources.FindObjectsOfTypeAll(typeof(Button));来防止获取到非场景内的对象
 */
namespace TonyBeanTools
{
    public static class TBTManager
    {
        public static Transform selectTransform;
        public static Action selectChanged;

        [InitializeOnLoadMethod]
        private static void App()
        {
            Selection.selectionChanged += () =>
            {
                if (Selection.gameObjects.Length < 1)
                {
                    selectTransform = null;
                    return;
                }
                else
                {
                    selectTransform = Selection.gameObjects[0].transform;
                }

                selectChanged?.Invoke();
            };
        }

        public static string[] GetStringList(string target, int max = 10000)
        {
            if (target.Length < max) return new string[] { target };
            int count = target.Length / max;
            var array = new string[count + 1];
            for (int i = 0; i < count; i++)
            {
                array[i] = target.Substring(i * max, max) + "\n";
            }
            array[count] = target.Substring(count * max, target.Length % max);
            return array;
        }


        public static Assembly GetAssembly()
        {
            Assembly[] AssbyCustmList = System.AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < AssbyCustmList.Length; i++)
            {
                string assbyName = AssbyCustmList[i].GetName().Name;
                if (assbyName == "Assembly-CSharp")
                {
                    return AssbyCustmList[i];
                }
            }
            return null;
        }

        public static string GetFileName(string fullName, bool withSuffix = false)
        {
            if (fullName==null)
            {
                return string.Empty;
            }

            string[] fullNameTemp = fullName.Split(new char[] { '/', '\\' });

            string nameWithSuffix = fullNameTemp[fullNameTemp.Length - 1];

            string filename = nameWithSuffix;

            if (withSuffix == false)
            {
                string[] nameWithSuffixTemp = nameWithSuffix.Split(new char[] { '.' });

                filename = nameWithSuffixTemp[0];
            }

            return filename;
        }

        public static string[] GetComponentsName(Transform t)
        {
            string[] defaultComponent = new string[] {"GameObject","Transform"}; 
            var coms = t.GetComponents<Component>();

            List<string> types = new List<string>();

            for (int i = 0; i < coms.Length; i++)
            {
                if (coms[i] == null) continue;
                types.Add(coms[i].GetType().Name);
            }

            string[] otherComponent = types.ToArray();

            return defaultComponent.Concat(otherComponent).ToArray();
        }

        public static string GetComponentString(string name)
        {
            StringBuilder sb = new StringBuilder();
            if (name == "GameObject")
            {
                sb.Append(".gameObject");
            }
            else if (name == "Transform")
            {
                sb.Append(".transform");
            }
            else
            {
                sb.Append(".GetComponent<");
                sb.Append(name);
                sb.Append(">()");
            }
            return sb.ToString();
        }
    }

    public class Lab
    {
        [MenuItem("TBTools/Items/清空图片")]
        static void DeleteAllImages()
        {
            DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath + @"/images");

            foreach (var item in di.GetFiles())
            {
                File.Delete(item.FullName);
            }
        }

        [MenuItem("TBTools/Items/预制体应用")]
        static void SaveAssets()
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
        public static void ApplyPrefab(GameObject obj)
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
        public static bool CheckResoureDuplicateName()
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
        public static void RefeshAsset()
        {
            AssetDatabase.Refresh();
            Debug.Log("刷新完成");
        }
    }
}