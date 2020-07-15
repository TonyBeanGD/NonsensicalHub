using UnityEngine;

namespace NonsensicalFrame
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T instance;

        protected virtual void Awake()
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }
}
