using UnityEngine;

namespace NonsensicalKit
{
    public class NonsensicalManager<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance!=null)
                {
                    return instance;
                }
                else
                {
                    throw new System.Exception("未实例化单例脚本");
                }
            }
            set
            {
                instance = value;
            }
        }

        protected virtual void Awake()
        {
            Instance = this as T;
        }
    }
}
