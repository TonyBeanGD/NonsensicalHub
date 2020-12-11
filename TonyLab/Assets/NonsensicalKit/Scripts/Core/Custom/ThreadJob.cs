using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace NonsensicalKit.Custom
{
    public class ThreadJob
    {
        private bool isRunning;
        private Thread thread;

        private readonly Action onBeginHandler;
        private readonly Action onFinishHandler;

        public ThreadJob(Action _onBeginHandler, Action _onFinishedHandler)
        {
            this.onBeginHandler = _onBeginHandler;
            this.onFinishHandler = _onFinishedHandler;
        }

        public virtual void Start()
        {
            if (isRunning == false)
            {
                thread = new Thread(Run);
                thread.Start();
            }
        }

        public virtual void Abort()
        {
            if (isRunning == true)
            {
                isRunning = false;
                thread.Abort();
                thread = null;
            }
        }

        public virtual bool Update()
        {
            if (!isRunning)
            {
                OnFinished();
                return true;
            }
            return false;
        }

        public IEnumerator WaitFor()
        {
            while (!Update())
            {
                //暂停协同程序，下一帧再继续往下执行
                yield return null;
            }
        }


        private void OnRun()
        {
            onBeginHandler?.Invoke();
        }

        private void OnFinished()
        {
            onFinishHandler?.Invoke();
            thread = null;
        }

        private void Run()
        {
            OnRun();
            isRunning = false;
        }
    }
}
