using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonsensicalKit
{

    public class ActionQueue
    {
        private Queue<Action> actions;

        public ActionQueue()
        {
            actions = new Queue<Action>();
        }
        public void AddAction(Action action)
        {
            actions.Enqueue(action);
        }
        public void DoNext()
        {
            Action action = actions.Dequeue();
            action?.Invoke();
        }
    }

}