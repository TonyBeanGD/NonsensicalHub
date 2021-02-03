using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonsensicalKit.Utility
{
    public static class DoTweenHelper
    {
        public static Tweenner DoFade(this CanvasGroup canvasGroup, float endValue, float duration)
        {
            CanvasGroupTweener newTweener = new CanvasGroupTweener(canvasGroup, endValue, duration);

            NonsensicalUnityInstance.Instance.tweenners.Add(newTweener);

            return newTweener;
        }

        public static Tweenner DoMove(this Transform _transform, Vector3 endValue, float duration)
        {
            TransformMoveTweener newTweener = new TransformMoveTweener(_transform, endValue, duration);

            NonsensicalUnityInstance.Instance.tweenners.Add(newTweener);

            return newTweener;
        }

        public static Tweenner DoRotate(this Transform _transform, Vector3 endValue, float duration)
        {
            TransformRotateTweener newTweener = new TransformRotateTweener(_transform, endValue, duration);

            NonsensicalUnityInstance.Instance.tweenners.Add(newTweener);

            return newTweener;
        }

        public static Tweenner DoQuaternionRotate(this Transform _transform, Quaternion endValue, float duration)
        {
            TransformQuaternionRotateTweener newTweener = new TransformQuaternionRotateTweener(_transform, endValue, duration);

            NonsensicalUnityInstance.Instance.tweenners.Add(newTweener);

            return newTweener;
        }

        public static Tweenner DoLocalMove(this Transform _transform, Vector3 endValue, float duration)
        {
            TransformLocalMoveTweener newTweener = new TransformLocalMoveTweener(_transform, endValue, duration);

            NonsensicalUnityInstance.Instance.tweenners.Add(newTweener);

            return newTweener;
        }


        public static Tweenner DoLocalMoveX(this Transform _transform, float endValue, float duration)
        {
            TransformLocalMoveXTweener newTweener = new TransformLocalMoveXTweener(_transform, endValue, duration);

            NonsensicalUnityInstance.Instance.tweenners.Add(newTweener);

            return newTweener;
        }

        public static Tweenner DoLocalRotate(this Transform _transform, Vector3 endValue, float duration)
        {
            TransformLocalRotateTweener newTweener = new TransformLocalRotateTweener(_transform, endValue, duration);

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
                NeedAbort = DoSpecific(1);
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
        private readonly CanvasGroup canvasGroup;
        private readonly float startValue;
        private readonly float endValue;

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
                return true;
            }
            canvasGroup.alpha = startValue + (endValue - startValue) * schedule;

            return false;
        }
    }

    public class TransformRotateTweener : Tweenner
    {
        private readonly Transform transform;
        private readonly Vector3 startValue;
        private readonly Vector3 endValue;
        public TransformRotateTweener(Transform _transform, Vector3 _endValue, float _duration) : base(_duration)
        {
            transform = _transform;
            startValue = _transform.eulerAngles;
            endValue = _endValue;
        }

        public override bool DoSpecific(float schedule)
        {
            if (transform == null)
            {
                return true;
            }
            transform.eulerAngles = Vector3.Lerp(startValue, endValue, schedule);

            return false;
        }
    }

    public class TransformQuaternionRotateTweener : Tweenner
    {
        private readonly Transform transform;
        private readonly Quaternion startValue;
        private readonly Quaternion endValue;
        public TransformQuaternionRotateTweener(Transform _transform, Quaternion _endValue, float _duration) : base(_duration)
        {
            transform = _transform;
            startValue = _transform.rotation;
            endValue = _endValue;
        }

        public override bool DoSpecific(float schedule)
        {
            if (transform == null)
            {
                return true;
            }
            transform.rotation = Quaternion.Lerp(startValue, endValue, schedule);

            return false;
        }
    }

    public class TransformMoveTweener : Tweenner
    {
        private readonly Transform transform;
        private readonly Vector3 startValue;
        private readonly Vector3 endValue;
        public TransformMoveTweener(Transform _transform, Vector3 _endValue, float _duration) : base(_duration)
        {
            transform = _transform;
            startValue = _transform.position;
            endValue = _endValue;
        }

        public override bool DoSpecific(float schedule)
        {
            if (transform == null)
            {
                return true;
            }
            transform.position = Vector3.Lerp(startValue, endValue, schedule);

            return false;
        }
    }

    public class TransformLocalMoveTweener : Tweenner
    {
        private readonly Transform transform;
        private readonly Vector3 startValue;
        private readonly Vector3 endValue;
        public TransformLocalMoveTweener(Transform _transform, Vector3 _endValue, float _duration) : base(_duration)
        {
            transform = _transform;
            startValue = _transform.localPosition;
            endValue = _endValue;
        }

        public override bool DoSpecific(float schedule)
        {
            if (transform == null)
            {
                return true;
            }
            transform.localPosition = Vector3.Lerp(startValue, endValue, schedule);

            return false;
        }
    }

    public class TransformLocalRotateTweener : Tweenner
    {
        private readonly Transform transform;
        private readonly Vector3 startValue;
        private readonly Vector3 endValue;
        public TransformLocalRotateTweener(Transform _transform, Vector3 _endValue, float _duration) : base(_duration)
        {
            transform = _transform;
            startValue = _transform.localEulerAngles;
            endValue = _endValue;
        }

        public override bool DoSpecific(float schedule)
        {
            if (transform == null)
            {
                return true;
            }
            transform.localEulerAngles = Vector3.Lerp(startValue, endValue, schedule);
            return false;
        }
    }

    public class TransformLocalMoveXTweener : Tweenner
    {
        private readonly Transform transform;
        private readonly float startValue;
        private readonly float endValue;
        public TransformLocalMoveXTweener(Transform _transform, float _endValue, float _duration) : base(_duration)
        {
            transform = _transform;
            startValue = _transform.localPosition.x;
            endValue = _endValue;
        }

        public override bool DoSpecific(float schedule)
        {
            if (transform == null)
            {
                return true;
            }
            Vector3 temp = transform.localPosition;
            temp.x = startValue + (endValue - startValue) * schedule;
            transform.localPosition = temp;
            return false;
        }
    }

}

