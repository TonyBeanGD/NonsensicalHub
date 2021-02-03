using NonsensicalKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NonsensicalKit
{
    public abstract class NonsensicalMono : MonoBehaviour
    {
        protected NonsensicalMono()
        {
            var v = this.GetType().GetInterfaces();
            foreach (var item in v)
            {
                if (item.GetInterface("ICustomEventHandler") != null)
                {
                    MethodInfo mi = item.GetMethods()[0];
                    mi = this.GetType().GetMethod(mi.Name);
                    ParameterInfo[] pi = mi.GetParameters();

                    if (pi.Length == 0)
                    {
                        Subscribe((uint)(int)Enum.Parse(typeof(CustomEventEnum), item.ToString()), () => mi.Invoke(this, null));
                    }
                    else
                    {
                        Type pt = pi[0].ParameterType;
                        Type ma = typeof(MessageAggregator<>).MakeGenericType(pt);
                        Type mh = typeof(MessageHandler<>).MakeGenericType(pt);

                        MethodInfo subMethod = ma.GetMethod("Subscribe", new Type[] { typeof(uint), mh });
                        object instance = ma.GetField("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                        var d = Delegate.CreateDelegate(mh, this, mi);

                        subMethod.Invoke(instance, new object[] { (uint)(int)Enum.Parse(typeof(CustomEventEnum), item.ToString()), d });
                    }
                }
            }
        }

        private List<ListenerInfo> listenerInfos = new List<ListenerInfo>();

        protected void Subscribe<T>(uint index, MessageHandler<T> func)
        {
            MessageAggregator<T>.Instance.Subscribe(index, func);

            ListenerInfo temp = new ListenerInfo(typeof(T), index, func);
            listenerInfos.Add(temp);
        }

        protected void Subscribe(uint index, MessageHandler func)
        {
            MessageAggregator.Instance.Subscribe(index, func);

            ListenerInfo temp = new ListenerInfo(null, index, func);
            listenerInfos.Add(temp);
        }
        protected void Publish<T>(uint index, T data)
        {
            MessageAggregator<T>.Instance.Publish(index, data);
        }

        protected void Publish(uint index)
        {
            MessageAggregator.Instance.Publish(index);
        }

        protected virtual void Awake()
        {

        }

        protected virtual void OnDestroy()
        {
            foreach (var listener in listenerInfos)
            {
                Type type = listener.Type;
                if (type == null)
                {
                    MessageAggregator.Instance.Subscribe(listener.Index, (MessageHandler)listener.Func);
                }
                else
                {
                    Type ma = typeof(MessageAggregator<>).MakeGenericType(type);
                    object instance = ma.GetField("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                    Type mh = typeof(MessageHandler<>).MakeGenericType(type);
                    MethodInfo unsubMethod = ma.GetMethod("Unsubscribe", new Type[] { typeof(uint), mh });
                    unsubMethod.Invoke(instance, new object[] { listener.Index, listener.Func });
                }
            }
        }

        private struct ListenerInfo
        {
            public Type Type;
            public uint Index;
            public object Func;

            public ListenerInfo(Type type, uint index, object func)
            {
                Type = type;
                Index = index;
                Func = func;
            }
        }
    }


    public interface ICustomEventHandler
    {
        /*
         事件接口应只有一个无返回值且有零个或一个传参的方法
         */
    }
}
