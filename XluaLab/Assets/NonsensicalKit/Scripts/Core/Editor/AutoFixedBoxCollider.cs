using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NonsensicalKit.Editor
{

    public class AutoFixedBoxCollider : EditorWindow
    {
        private static string showText; //显示给用户的文本

        [MenuItem("TBTools/自动添加自适应大小盒子碰撞器")]
        static void AddComponentToCrtTarget()
        {
            showText = "这是一条信息";
            if (Selection.gameObjects.Length == 0)
            {
                showText = "未选中任何对象";
            }
            else
            {
                for (int i = 0; i < Selection.gameObjects.Length; i++)
                {
                    AutoFixed(Selection.gameObjects[i].transform);
                }

                showText = "盒子碰撞器自适应完成";
            }

            EditorWindow.GetWindow(typeof(AutoFixedBoxCollider));
        }

        [MenuItem("TBTools/自动删除盒子碰撞器")]
        static void DeleteComponentToCrtTarget()
        {
            showText = "这是一条信息";
            if (Selection.gameObjects.Length == 0)
            {
                showText = "未选中任何对象";
            }
            else
            {
                for (int i = 0; i < Selection.gameObjects.Length; i++)
                {
                    AutoDelete(Selection.gameObjects[i].transform);
                }

                showText = "盒子碰撞器删除完成";
            }

            EditorWindow.GetWindow(typeof(AutoFixedBoxCollider));
        }

        void OnGUI()
        {
            EditorGUILayout.HelpBox(showText, MessageType.Info);
        }

        private static void AutoFixed(Transform tran)
        {
            Stack<Transform> crtTargets = new Stack<Transform>();
            crtTargets.Push(tran);

            while (crtTargets.Count > 0)
            {
                Transform crt = crtTargets.Pop();

                Renderer Renderer = crt.gameObject.transform.GetComponent<Renderer>();

                if (Renderer == null)
                {
                    FitToChildren(crt.gameObject);
                }
                else
                {
                    FitCollider(crt.gameObject);
                }

                foreach (Transform item in crt)
                {
                    crtTargets.Push(item);
                }
            }
        }

        private static void AutoDelete(Transform tran)
        {
            Stack<Transform> crtTargets = new Stack<Transform>();
            crtTargets.Push(tran);

            while (crtTargets.Count > 0)
            {
                Transform crt = crtTargets.Pop();

                if (crt.GetComponent<BoxCollider>() != null)
                {
                    DestroyImmediate(crt.GetComponent<BoxCollider>());
                }

                foreach (Transform item in crt)
                {
                    crtTargets.Push(item);
                }
            }
        }

        /// <summary>
        /// 添加盒子碰撞器并且自适应其大小
        /// </summary>
        /// <param name="go"></param>
        private static void FitCollider(GameObject go)
        {
            Renderer Renderer = go.transform.GetComponent<Renderer>();
            if (Renderer == null)
            {
                return;
            }
            BoxCollider bc;
            if ((bc = go.GetComponent<BoxCollider>()) == null)
            {
                bc = go.AddComponent<BoxCollider>();
            }

            bc.isTrigger = true;

            Quaternion qn = go.transform.rotation;
            go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            Bounds bounds = Renderer.bounds;
            go.transform.rotation = qn;

            bc.size = new Vector3(bounds.size.x / go.transform.lossyScale.x, bounds.size.y / go.transform.lossyScale.y, bounds.size.z / go.transform.lossyScale.z);
        }

        /// <summary>
        /// 创建一个刚好包住所有子物体的盒子碰撞器
        /// </summary>
        /// <param name="go"></param>
        private static void FitToChildren(GameObject go)
        {
            Quaternion qn = go.transform.rotation;
            go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

            bool hasBounds = false;
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

            Renderer[] childRenderers = go.transform.GetComponentsInChildren<Renderer>();

            foreach (var item in childRenderers)
            {
                if (hasBounds)
                {
                    bounds.Encapsulate(item.bounds);
                }
                else
                {
                    bounds = item.bounds;
                    hasBounds = true;
                }

            }

            BoxCollider collider;
            if ((collider = go.GetComponent<BoxCollider>()) == null)
            {
                collider = go.AddComponent<BoxCollider>();
            }

            collider.isTrigger = true;
            collider.center = bounds.center - go.transform.position;

            collider.size = new Vector3(bounds.size.x / go.transform.lossyScale.x, bounds.size.y / go.transform.lossyScale.y, bounds.size.z / go.transform.lossyScale.z);

            go.transform.rotation = qn;
        }
    }


}
