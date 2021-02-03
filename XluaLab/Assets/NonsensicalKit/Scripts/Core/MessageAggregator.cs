using NonsensicalKit.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NonsensicalKit
{
    public delegate void MessageHandler<T>(T arg);
    public delegate void MessageHandler();
    
    public class MessageAggregator<T>
    {
        public static MessageAggregator<T> Instance = new MessageAggregator<T>();

        private readonly Dictionary<uint, MessageHandler<T>> _messages = new Dictionary<uint, MessageHandler<T>>();

        private MessageAggregator()
        {

        }

        public void Subscribe(uint name, MessageHandler<T> handler)
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

        public void Unsubscribe(uint name, MessageHandler<T> handler)
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

        public void Publish(uint name, T args)
        {
            if (_messages.ContainsKey(name) && _messages[name] != null)
            {
                _messages[name](args);
            }
        }
    }

    public class MessageAggregator
    {
        /// <summary>
        /// 不能使用单例基类，会使构造函数暴露
        /// </summary>
        public static MessageAggregator Instance = new MessageAggregator();

        private readonly Dictionary<uint, MessageHandler> _messages = new Dictionary<uint, MessageHandler>();

        private MessageAggregator()
        {

        }

        public void Subscribe(uint name, MessageHandler handler)
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

        public void Unsubscribe(uint name, MessageHandler handler)
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

        public void Publish(uint name)
        {
            if (_messages.ContainsKey(name) && _messages[name] != null)
            {
                _messages[name]();
            }
        }
    }
}
