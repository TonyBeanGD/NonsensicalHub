using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NonsensicalKit
{
    public class NotificationCenter
    {
        private static NotificationCenter instance;
        public static NotificationCenter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GetNewNotificationInstance();


                }
                return instance;
            }
        }

        private static NotificationCenter GetNewNotificationInstance()
        {
            NotificationCenter newInstance = new NotificationCenter();
            newInstance.notificationList = new Dictionary<Type, object>();

            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var item in types)
            {
                NotificationContentAttribute temp = (NotificationContentAttribute)item.GetCustomAttribute(typeof(NotificationContentAttribute));

                if (temp != null)
                {
                    var v = typeof(Signal<>).MakeGenericType(item);

                    var notification = Activator.CreateInstance(v);

                    newInstance.notificationList.Add(item, notification);
                }

            }
            return newInstance;
        }

        private Dictionary<Type, object> notificationList;

        /// <summary>
        /// 注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        public void AttachObsever<T>(Action<T> action) where T : INotificationContent
        {
            Type type = action.GetType().GenericTypeArguments[0];

            if (notificationList.ContainsKey(type) == false)
            {
                Debugger.Log();
                return;
            }

            var v = (Signal<T>)notificationList[type];
            v.AttachObsever(action);
        }

        /// <summary>
        /// 注销监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        public void DetachObsever<T>(Action<T> action) where T : INotificationContent
        {
            Type type = action.GetType().GenericTypeArguments[0];

            if (notificationList.ContainsKey(type) == false)
            {
                return;
            }

            var v = (Signal<T>)notificationList[type];
            v.DetachObsever(action);

        }

        /// <summary>
        /// 激活监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void PostDispatch<T>(T data) where T : INotificationContent
        {
            Type type = data.GetType();

            if (notificationList.ContainsKey(type) == false)
            {
                return;
            }

            var v = (Signal<T>)notificationList[type];
            v.PostDispatch(data);
        }

        private class Signal<T>
        {
            private Action<T> action;

            /// <summary>
            /// 注册监听
            /// </summary>
            /// <param name="_action"></param>
            public void AttachObsever(Action<T> _action)
            {
                action += _action;
            }

            /// <summary>
            /// 注销监听
            /// </summary>
            /// <param name="_action"></param>
            public void DetachObsever(Action<T> _action)
            {
                action -= _action;
            }

            /// <summary>
            /// 激活监听
            /// </summary>
            /// <param name="_data"></param>
            public void PostDispatch(T _data)
            {
                action?.Invoke(_data);
            }
        }
        
    }

    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    public class NotificationContentAttribute : Attribute
    {
        public NotificationContentAttribute()
        {

        }

        public int NamedInt { get; set; }
    }

    public interface INotificationContent
    {

    }

    /* 通知内容类或者结构体需要使用 [NotificationContent] 特性并且使用 INotificationContent 接口
     * 下面是类的模板和结构体的模板
     */

    [NotificationContent]
    public class TemplateClass : INotificationContent
    {
        public string TemplateString;
    }

    [NotificationContent]
    public struct TemplateStruct : INotificationContent
    {
        public float TemplateFloat;
    }

}