using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NonsensicalKit;

namespace NonsensicalKit.UI
{
    public abstract class ListElementBase<ElementData> : MonoBehaviour where ElementData : class, IData
    {
        protected bool _isSelect;

        protected ElementData _elementData;

        protected virtual void Awake()
        {
            MessageAggregator<ElementData>.Instance.Subscribe((uint)UIEnum.Select, OnSelectFunc);
        }

        protected virtual void OnDestroy()
        {
            MessageAggregator<ElementData>.Instance.Unsubscribe((uint)UIEnum.Select, OnSelectFunc);
        }

        protected virtual void OnSelectFunc(ElementData value)
        {
            _isSelect = (value == _elementData);
            SelectSwitch(_isSelect);
            UpdateUI();
        }

        public virtual void Init(ElementData data)
        {
            _elementData = data;
        }

        protected virtual void OnTopSort()
        {
            MessageAggregator<ElementData>.Instance.Publish((uint)UIEnum.TopSort, _elementData);
        }

        protected abstract void UpdateUI();

        protected abstract void SelectSwitch(bool isSelect);
    }

}

