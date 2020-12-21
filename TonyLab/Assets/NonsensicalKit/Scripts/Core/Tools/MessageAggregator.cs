using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NonsensicalKit
{
    public delegate void MessageHandler<T>(MessageArgs<T> args);

    public class MessageAggregator<T>
    {
        /// <summary>
        /// 不能使用单例基类，会使构造函数暴露
        /// </summary>
        public static MessageAggregator<T> Instance = new MessageAggregator<T>();
        
        private readonly Dictionary<uint, MessageHandler<T>> _messages_Uint = new Dictionary<uint, MessageHandler<T>>();

        private MessageAggregator()
        {

        }
        public void Subscribe(uint name, MessageHandler<T> handler)
        {
            if (!_messages_Uint.ContainsKey(name))
            {
                _messages_Uint.Add(name, handler);
            }
            else
            {
                _messages_Uint[name] += handler;
            }
        }

        public void Unsubscribe(uint name, MessageHandler<T> handler)
        {
            if (!_messages_Uint.ContainsKey(name))
            {
                return;
            }
            else
            {
                _messages_Uint[name] -= handler;

                if (_messages_Uint[name] == null)
                {
                    _messages_Uint.Remove(name);
                }
            }
        }

        public void Publish(uint name, MessageArgs<T> args)
        {
            if (_messages_Uint.ContainsKey(name) && _messages_Uint[name] != null)
            {
                _messages_Uint[name](args);
            }
        }
    }

    public class MessageArgs<T>
    {
        public object sender;
        public T Item { get; set; }
        public MessageArgs(object sender, T item)
        {
            Item = item;
        }
    }

    public enum TypeEnum
    {
        Test1=15000,
        Test2,
    }

}
