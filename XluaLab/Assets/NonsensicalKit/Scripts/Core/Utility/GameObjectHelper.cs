using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NonsensicalKit.Utility
{
    public static class GameObjectHelper
    {
        public static string[] GetComponentsName(Transform t)
        {
            string[] defaultComponent = new string[] { "GameObject", "Transform" };
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

        /// <summary>
        /// 销毁未激活部分
        /// </summary>
        /// <param name="tsf"></param>
        public static void DestroyUnactivePart(Transform tsf)
        {
            Queue<Transform> nodes = new Queue<Transform>();
            nodes.Enqueue(tsf);

            while (nodes.Count > 0)
            {
                Transform crtNode = nodes.Dequeue();
                if (crtNode.gameObject.activeSelf == false)
                {
                    GameObject.Destroy(crtNode.gameObject);
                }
                else
                {
                    foreach (Transform item in crtNode)
                    {
                        nodes.Enqueue(item);
                    }
                }
            }
        }
    }
}

