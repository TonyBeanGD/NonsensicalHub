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

        private readonly Dictionary<string, MessageHandler<T>> _messages = new Dictionary<string, MessageHandler<T>>();

        private MessageAggregator()
        {

        }

        public void Subscribe(string name, MessageHandler<T> handler)
        {
            if (!_messages.ContainsKey(name))
            {
                _messages.Add(name, handler);
            }
            else
            {
                _messages[name] += handler;
            }
        }

        public void Unsubscribe(string name, MessageHandler<T> handler)
        {
            if (!_messages.ContainsKey(name))
            {
                return;
            }
            else
            {
                _messages[name] -= handler;

                if (_messages[name] == null)
                {
                    _messages.Remove(name);
                }
            }
        }
        
        public void Publish(string name, MessageArgs<T> args)
        {
            if (_messages.ContainsKey(name) && _messages[name] != null)
            {
                //转发
                _messages[name](args);
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
}
