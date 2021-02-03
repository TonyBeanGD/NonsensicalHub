using NonsensicalKit.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NonsensicalKit.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIBase : NonsensicalMono
    {
        public bool IsShow { get; private set; }

        protected CanvasGroup _canvasGroup;

        protected override void Awake()
        {
            base.Awake();
            _canvasGroup = transform.GetComponent<CanvasGroup>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void Appear(bool immediately = false)
        {
            OpenSelf(immediately);
        }

        public void Disappear(bool immediately = false)
        {
            CloseSelf(immediately);
        }

        protected virtual void OpenSelf(bool immediately = false)
        {
            _canvasGroup.blocksRaycasts = true;
            if (immediately)
            {
                _canvasGroup.alpha = 1;
            }
            else
            {
                _canvasGroup.DoFade(1, 0.2f);
            }
            IsShow = true;
        }

        protected virtual void CloseSelf(bool immediately = false)
        {
            _canvasGroup.blocksRaycasts = false;
            if (immediately)
            {
                _canvasGroup.alpha = 0;
            }
            else
            {
                _canvasGroup.DoFade(0, 0.2f);
            }
            IsShow = false;
        }

        protected void SwitchSelf(bool immediately = false)
        {
            if (_canvasGroup.blocksRaycasts == true)
            {
                CloseSelf(immediately);
            }
            else
            {
                OpenSelf(immediately);
            }
        }
    }
}

