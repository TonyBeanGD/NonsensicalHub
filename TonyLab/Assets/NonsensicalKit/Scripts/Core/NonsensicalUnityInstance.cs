using NonsensicalKit.Utility;
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

        private string GUIText;
        private float GUITimer;

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

        public void LogOnGUI(string text,float time=3)
        {
            GUIText = text;
            GUITimer = time;
        }

        private void OnGUI()
        {
            GUITimer -= Time.deltaTime;
            if (GUITimer>0)
            {
                GUI.TextArea(new Rect(Screen.width * 0.5f - 75, Screen.height * 0.5f - 50, 150, 100), GUIText);
            }
        }

        public void DelayDoIt(float _delayTime, Action _action)
        {
            StartCoroutine(DelayDoItCoroutine(_delayTime, _action));
        }

        private IEnumerator DelayDoItCoroutine(float _delayTime, Action _action)
        {
            yield return new WaitForSeconds(_delayTime);

            _action?.Invoke();
        }
    }
}
