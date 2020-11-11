using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonsensicalKit
{
    public abstract class UGUIViewModelBase : ViewModelBase
    {
        public bool IsRevealed { get; private set; }
        public bool IsRevealInProgress { get; private set; }
        public bool IsHideInProgress { get; private set; }

        public override void OnStartReveal()
        {
            IsRevealInProgress = true;
            base.OnStartReveal();
        }

        public virtual void OnFinishReveal()
        {
            IsRevealInProgress = false;
            IsRevealed = true;
        }

        public virtual void OnStartHide()
        {
            IsHideInProgress = true;

        }

        public virtual void OnFinishHide()
        {
            IsHideInProgress = false;
            IsRevealed = false;
        }
    }

}
