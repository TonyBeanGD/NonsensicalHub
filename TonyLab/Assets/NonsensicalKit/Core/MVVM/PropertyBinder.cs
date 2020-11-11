using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using NonsensicalKit;

namespace NonsensicalKit
{
    public class PropertyBinder<T> where T : ViewModelBase
    {
        private delegate void BindHandler(T viewmodel);
        private delegate void UnBindHandler(T viewmodel);

        private BindHandler _binders;
        private UnBindHandler _unbinders;

        /// <summary>
        /// 使用反射通过字段名添加对应监听
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="name"></param>
        /// <param name="valueChangedHandler"></param>
        public void Add<TProperty>(string name, BindableProperty<TProperty>.ValueChangedHandler valueChangedHandler)
        {
            var fieldInfo = typeof(T).GetField(name, BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo == null)
            {
                throw new Exception(string.Format("Unable to find bindableproperty field '{0}.{1}'", typeof(T).Name, name));
            }

            _binders += (viewmodel =>
            {
                GetPropertyValue<TProperty>(name, viewmodel, fieldInfo).OnValueChanged += valueChangedHandler;
            });

            _unbinders += (viewModel =>
            {
                GetPropertyValue<TProperty>(name, viewModel, fieldInfo).OnValueChanged -= valueChangedHandler;
            });

        }

        private BindableProperty<TProperty> GetPropertyValue<TProperty>(string name, T viewModel, FieldInfo fieldInfo)
        {
            var value = fieldInfo.GetValue(viewModel);
            BindableProperty<TProperty> bindableProperty = value as BindableProperty<TProperty>;
            if (bindableProperty == null)
            {
                throw new Exception(string.Format("Illegal bindableproperty field '{0}.{1}' ", typeof(T).Name, name));
            }

            return bindableProperty;
        }

        public void Bind(T viewmodel)
        {
            if (viewmodel != null)
            {
                _binders?.Invoke(viewmodel);
            }
        }

        public void Unbind(T viewmodel)
        {
            if (viewmodel != null)
            {
                _unbinders?.Invoke(viewmodel);
            }
        }

    }
}
