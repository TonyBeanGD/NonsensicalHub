using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NonsensicalKit
{
    public abstract class ViewModelBase
    {
        private bool _isInitialized;

        public virtual void OnStartReveal()
        {
            //在开始显示的时候进行初始化操作
            if (!_isInitialized)
            {
                OnInitialize();
                _isInitialized = true;
            }
        }


        protected virtual void OnInitialize()
        {
            
        }


        public virtual void OnDestory()
        {
            
        }

    }
}
