using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NonsensicalFrame
{
    public enum EventName
    {

    }

    public class NotificationCenter
    {
        private Dictionary<EventName, List<Action<object>>> allListeners = new Dictionary<EventName, List<Action<object>>>();

        private static NotificationCenter _instance;

        public static NotificationCenter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NotificationCenter();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="action"></param>
        public void AttachObsever(EventName eventName, Action<object> action)
        {
            if (!allListeners.Keys.Contains(eventName))
            {
                allListeners.Add(eventName, new List<Action<object>>());
            }
            allListeners[eventName].Add(action);
        }

        /// <summary>
        /// 注销事件的监视
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="action"></param>
        public void DetachObsever(EventName eventName, Action<object> action)
        {
            if (!allListeners.Keys.Contains(eventName))
            {
                Debug.Log("注销监视失败，因为没有正在激活的监视");
                return;
            }

            allListeners[eventName].Remove(action);

            if (allListeners[eventName] == null)
            {
                allListeners.Remove(eventName);
            }
        }

        /// <summary>
        /// 激活事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="objs"></param>
        public void PostDispatch(EventName eventName, object objs)
        {
            if (!allListeners.ContainsKey(eventName))
            {
                Debug.Log("激活的事件不存在监视：" + eventName.ToString());
                return;
            }

            foreach (var item in allListeners[eventName])
            {
                item(objs);
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            allListeners.Clear();
        }

        /// <summary>
        /// 获取事件是否已经注册
        /// </summary>
        public bool GetEventState(EventName eventName)
        {
            return allListeners.ContainsKey(eventName);
        }
    }
}
