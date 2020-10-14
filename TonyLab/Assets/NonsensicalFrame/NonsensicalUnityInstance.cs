using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonsensicalFrame
{
    /// <summary>
    /// 场景中的实例对象，用于执行协程
    /// </summary>
    public class NonsensicalUnityInstance : MonoBehaviour
    {
        private static NonsensicalUnityInstance instance;
        public static NonsensicalUnityInstance Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject instanceGameobject = new GameObject("Nonsensical Unity Instance");
                    DontDestroyOnLoad(instanceGameobject);
                    instance = instanceGameobject.AddComponent<NonsensicalUnityInstance>();
                }
                return instance;
            }
        }
    }

}
