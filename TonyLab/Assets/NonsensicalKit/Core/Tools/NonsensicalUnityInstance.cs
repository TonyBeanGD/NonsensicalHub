using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonsensicalKit
{
    /// <summary>
    /// 场景中的实例对象，用于执行协程、线程Debug、DoTween
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

        public Queue<string> messages;

        public List<Tweenner> tweenners;

        private void Awake()
        {
            messages = new Queue<string>();
            tweenners = new List<Tweenner>();
        }

        private void Update()
        {
            while (messages.Count > 0)
            {
                Debug.Log(messages.Dequeue());
            }
            
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < tweenners.Count; i++)
            {
                if (tweenners[i].NeedAbort)
                {
                    tweenners.RemoveAt(i);
                    i--;
                }
                else if (tweenners[i].DoIt(deltaTime) == true)
                {
                    tweenners.RemoveAt(i);
                    i--;
                }
            }
        }

        public void DelayDoIt(float _delayTime,Action _action)
        {
            StartCoroutine(DelayDoItCoroutine(_delayTime,_action));
        }

        public IEnumerator DelayDoItCoroutine(float _delayTime, Action _action)
        {
            yield return new WaitForSeconds(_delayTime);

            _action?.Invoke();
        }
    }
}
