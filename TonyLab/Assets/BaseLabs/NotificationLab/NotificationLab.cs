using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace NotificationLab
{
    /// <summary>
    /// TODO:避免object的拆箱
    /// </summary>
    public class NotificationCenter
    {
        static NotificationCenter()
        {
            NotificationList = new Dictionary<Type, object>();

            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var item in types)
            {
                var v = typeof(Signal<>).MakeGenericType(item);

                var notification = Activator.CreateInstance(v);

                NotificationContentAttribute temp = (NotificationContentAttribute)item.GetCustomAttribute(typeof(NotificationContentAttribute));

                if (temp != null)
                {
                    NotificationList.Add(item, notification);
                }

            }
        }

        public static Dictionary<Type, object> NotificationList;

        /// <summary>
        /// 注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        public static void AttachObsever<T>(Action<T> action)
        {
            Type type = action.GetType().GenericTypeArguments[0];

            if (NotificationList.ContainsKey(type) == false)
            {
                return;
            }

            var v = (Signal<T>)NotificationList[type];
            v.AttachObsever(action);
        }

        /// <summary>
        /// 注销监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        public static void DetachObsever<T>(Action<T> action)
        {
            Type type = action.GetType().GenericTypeArguments[0];

            if (NotificationList.ContainsKey(type) == false)
            {
                return;
            }

            var v = (Signal<T>)NotificationList[type];
            v.DetachObsever(action);

        }

        /// <summary>
        /// 激活监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public static void PostDispatch<T>(T data)
        {
            Type type = data.GetType();

            if (NotificationList.ContainsKey(type) == false)
            {
                return;
            }

            var v = (Signal<T>)NotificationList[type];
            v.PostDispatch(data);
        }
    }
    
    public class Signal<T>
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

    [NotificationContent]
    public class Template
    {
        public string TemplateString;
    }

    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    sealed class NotificationContentAttribute : Attribute
    {
        public NotificationContentAttribute()
        {

        }

        public int NamedInt { get; set; }
    }
}
