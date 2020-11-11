using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NonsensicalKit;

namespace NonsensicalKit
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UGUIViewBase<T> : ViewBase<T> where T : UGUIViewModelBase
    {
        /// <summary>
        /// 是否在隐藏时销毁
        /// </summary>
        public bool DestroyOnHide;

        /// <summary>
        /// 显示之后的回掉函数
        /// </summary>
        public Action RevealedAction { get; set; }

        /// <summary>
        /// 隐藏之后的回掉函数
        /// </summary>
        public Action HiddenAction { get; set; }

        public override void OnDestroy()
        {
            if (BindingContext != null)
            {
                if (BindingContext.IsRevealed)
                {
                    Hide(true);
                }
            }
            base.OnDestroy();
        }

        public void Reveal(bool immediate = false, Action action = null)
        {
            if (action != null)
            {
                RevealedAction += action;
            }
            OnAppear();
            OnReveal(immediate);
            OnRevealed();
        }

        public void Hide(bool immediate = false, Action action = null)
        {
            if (action != null)
            {
                HiddenAction += action;
            }
            Hide(immediate);
            OnHidden();
            OnDisappear();
        }

        /// <summary>
        /// 开始显示
        /// </summary>
        /// <param name="immediate"></param>
        private void OnReveal(bool immediate)
        {
            if (immediate)
            {
                //立即显示
                transform.localScale = Vector3.one;
                GetComponent<CanvasGroup>().alpha = 1;
            }
            else
            {
                StartAnimatedReveal();
            }
        }

        /// <summary>
        /// alpha 0->1 之后执行
        /// </summary>
        public virtual void OnRevealed()
        {
            BindingContext.OnFinishReveal();
            //回掉函数
            RevealedAction?.Invoke();
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <param name="immediate">是否立即执行，为否时渐隐</param>
        private void Hide(bool immediate)
        {
            BindingContext.OnStartHide();
            if (immediate)
            {
                //立即隐藏
                transform.localScale = Vector3.zero;
                GetComponent<CanvasGroup>().alpha = 0;
            }
            else
            {
                StartAnimatedHide();
            }
        }

        /// <summary>
        /// alpha 1->0时
        /// </summary>
        public virtual void OnHidden()
        {
            //回掉函数
            HiddenAction?.Invoke();
        }

        /// <summary>
        /// 消失 Enable->Disable
        /// </summary>
        public virtual void OnDisappear()
        {
            gameObject.SetActive(false);
            BindingContext.OnFinishHide();
            if (DestroyOnHide)
            {
                //销毁
                Destroy(this.gameObject);
            }

        }

        /// <summary>
        /// scale:1,alpha:1
        /// </summary>
        protected virtual void StartAnimatedReveal()
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            transform.localScale = Vector3.one;

            canvasGroup.DoFade(1, 0.2f).SetDelay(0.2f).OnComplete(() =>
            {
                canvasGroup.interactable = true;
            });
        }

        /// <summary>
        /// alpha:0,scale:0
        /// </summary>
        protected virtual void StartAnimatedHide()
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.DoFade(0, 0.2f).SetDelay(0.2f).OnComplete(() =>
            {
                transform.localScale = Vector3.zero;
                canvasGroup.interactable = true;
            });
        }
    }
}