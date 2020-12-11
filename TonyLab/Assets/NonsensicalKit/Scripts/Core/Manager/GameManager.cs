using NonsensicalKit.Custom;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace NonsensicalKit
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private List<IManager> managers;

        private void Start()
        {

            string logLock = Path.Combine(Application.streamingAssetsPath, "Nonsensical");

            if (File.Exists(logLock))
            {
                File.Delete(logLock);
                gameObject.AddComponent<DebugConsole>();
            }

            managers = new List<IManager>();

            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var item in types)
            {
                var type = item.GetInterface("IManager");

                if (type != null)
                {
                    var instance = Activator.CreateInstance(item);

                    IManager im = (IManager)instance;
                    managers.Add(im);
                }
            }

            StartCoroutine(Init());

        }

        private IEnumerator Init()
        {
            foreach (var item in managers)
            {
                item.OnInit();
            }

            yield return AllInitComplete();

            foreach (var item in managers)
            {
                item.OnLateInit();
            }

            yield return AllLateInitComplete();


            MessageAggregator<int>.Instance.Publish("InitEnd", new MessageArgs<int>(this, 1));
        }

        private IEnumerator AllInitComplete()
        {
            while (true)
            {
                bool noComleteFlag = false;

                foreach (var item in managers)
                {
                    if (item.InitComplete == false)
                    {
                        noComleteFlag = true;
                        break;
                    }
                }

                if (noComleteFlag == true)
                {
                    yield return null;
                    continue;
                }
                else
                {
                    break;
                }
            }
        }


        private IEnumerator AllLateInitComplete()
        {
            while (true)
            {
                bool noComleteFlag = false;

                foreach (var item in managers)
                {
                    if (item.LateInitComplete == false)
                    {
                        noComleteFlag = true;
                        break;
                    }
                }

                if (noComleteFlag == true)
                {
                    yield return null;
                    continue;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
