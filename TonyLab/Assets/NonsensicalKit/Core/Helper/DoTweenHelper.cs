using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonsensicalKit
{
    public static class DoTweenHelper
    {
        public static Tweenner DoFade(this CanvasGroup canvasGroup, float endValue, float duration)
        {
            CanvasGroupTweener newTweener = new CanvasGroupTweener(canvasGroup, endValue, duration);

            NonsensicalUnityInstance.Instance.tweenners.Add(newTweener);

            return newTweener;
        }
    }

    public abstract class Tweenner
    {
        private readonly float duration;
        private float delay;

        protected float scheduleTime;

        public bool NeedAbort;

        public delegate void OnCompleteHander();
        public OnCompleteHander OnCompleteEvent;


        protected Tweenner(float _duration)
        {
            duration = _duration;
            scheduleTime = 0;
            delay = 0;
            NeedAbort = false;
        }

        public bool DoIt(float _deltaTime)
        {
            scheduleTime += _deltaTime;

            float schedule = (scheduleTime - delay) / duration;

            if (schedule > 1)
            {
                NeedAbort=DoSpecific(1);
                OnCompleteEvent?.Invoke();
                return true;
            }
            else
            {
                NeedAbort = DoSpecific(schedule);
                return false;
            }
        }

        public void Abort()
        {
            NeedAbort = true;
        }

        public abstract bool DoSpecific(float _schedule);

        public Tweenner SetDelay(float _time)
        {
            delay += _time;
            return this;
        }

        public Tweenner OnComplete(OnCompleteHander _func)
        {
            OnCompleteEvent += _func;
            return this;
        }

    }

    public class CanvasGroupTweener : Tweenner
    {
        private CanvasGroup canvasGroup;
        private float startValue;
        private float endValue;

        public CanvasGroupTweener(CanvasGroup _canvasGroup, float _endValue, float _duration) : base(_duration)
        {
            canvasGroup = _canvasGroup;
            startValue = _canvasGroup.alpha;
            endValue = _endValue;
        }

        public override bool DoSpecific(float schedule)
        {
            if (canvasGroup == null)
            {
                return false;
            }
            canvasGroup.alpha = startValue + (endValue - startValue) * schedule;

            return true;
        }
    }
}

