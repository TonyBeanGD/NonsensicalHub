using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NonsensicalKit
{
    public abstract class ViewBase<T> : MonoBehaviour where T : ViewModelBase
    {
        private bool _isInitialized;
        protected readonly PropertyBinder<T> Binder = new PropertyBinder<T>();
        public readonly BindableProperty<T> ViewModelProperty = new BindableProperty<T>();
        
        public virtual void OnDestroy()
        {
            if (BindingContext != null)
            {
                BindingContext.OnDestory();
                BindingContext = null;
                ViewModelProperty.OnValueChanged = null;
            }
        }

        public T BindingContext
        {
            get
            {
                return ViewModelProperty.Value;
            }
            set
            {
                if (!_isInitialized)
                {
                    OnInitialize();
                    _isInitialized = true;
                }
                //触发OnValueChanged事件
                ViewModelProperty.Value = value;
            }
        }

       
        /// <summary>
        /// 初始化View，当BindingContext改变时执行
        /// </summary>
        protected virtual void OnInitialize()
        {
            //无所ViewModel的Value怎样变化，只对OnValueChanged事件监听(绑定)一次
            ViewModelProperty.OnValueChanged += OnBindingContextChanged;
        }

        /// <summary>
        /// 激活gameObject,Disable->Enable
        /// </summary>
        public virtual void OnAppear()
        {
            gameObject.SetActive(true);
            BindingContext.OnStartReveal();
        }
    
        /// <summary>
        /// 绑定的上下文发生改变时的响应方法
        /// 利用反射+=/-=OnValuePropertyChanged
        /// </summary>
        public virtual void OnBindingContextChanged(T oldValue, T newValue)
        {
            Binder.Unbind(oldValue);
            Binder.Bind(newValue);
        }
    }
}
