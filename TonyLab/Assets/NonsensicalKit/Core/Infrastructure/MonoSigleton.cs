using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NonsensicalKit
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T _instance{ get;private set;}

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                return;
            }

            _instance = this as T;


            DontDestroyOnLoad(gameObject);
        }
    }
}
