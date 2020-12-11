using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonsensicalKit.Custom
{
    public abstract class Singleton<T> where T : class
    {
        public static T Instance { get; private set; }

        protected Singleton()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
        }
    }
}
