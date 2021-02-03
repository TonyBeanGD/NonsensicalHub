using NonsensicalKit;
using NonsensicalKit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NonsensicalKit.UI
{

    public class ListManagerBase<ListData, ListElement, ElementData> : UIBase, ICanEdit
        where ListData : DataObject<ElementData>
        where ListElement : ListElementBase<ElementData>
        where ElementData : class, IData, new()
    {
        [SerializeField] private Transform _group;

        [SerializeField] private Button btn_Add;

        [SerializeField] private Button btn_Remove;

        protected ListData listData;

        protected override void Awake()
        {
            base.Awake();

            MessageAggregator<ListData>.Instance.Subscribe((uint)UIEnum.Load, OnLoadFunc);
            MessageAggregator<ElementData>.Instance.Subscribe((uint)UIEnum.TopSort, OnTopSortFunc);
            MessageAggregator<bool>.Instance.Subscribe((uint)UIEnum.CanEditSwitch, OnCanEditSwitch);


            btn_Add.onClick.AddListener(OnAddButtonClick);
            btn_Remove.onClick.AddListener(OnRemoveButtonClick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            MessageAggregator<ListData>.Instance.Unsubscribe((uint)UIEnum.Load, OnLoadFunc);
            MessageAggregator<ElementData>.Instance.Unsubscribe((uint)UIEnum.TopSort, OnTopSortFunc);
            MessageAggregator<bool>.Instance.Unsubscribe((uint)UIEnum.CanEditSwitch, OnCanEditSwitch);
        }


        private void OnCanEditSwitch(bool value)
        {
            CanEditSwitch(value);
        }

        protected virtual void OnLoadFunc(ListData value)
        {
            listData = value;
            UpdateUI();
        }

        protected virtual void OnTopSortFunc(ElementData value)
        {
            listData.Top(value);

            UpdateUI();
        }

        public virtual void CanEditSwitch(bool canEdit)
        {
            btn_Add.gameObject.SetActive(canEdit);
            btn_Remove.gameObject.SetActive(canEdit);
        }

        protected virtual void UpdateUI()
        {
            _group.gameObject.SetActive(listData.Count != 0);

            int max = Mathf.Max(_group.childCount - 1, listData.Count);

            GameObject prefab = _group.GetChild(0).gameObject;
            prefab.gameObject.SetActive(false);
            for (int i = 0; i < max; i++)
            {
                if (i < listData.Count)
                {
                    ListElement crtView;
                    if (i + 1 < _group.childCount)
                    {
                        crtView = _group.GetChild(i + 1).GetComponent<ListElement>();
                    }
                    else
                    {
                        crtView = Instantiate(prefab, _group).GetComponent<ListElement>();
                    }

                    crtView.Init((ElementData)listData[i]);
                    crtView.gameObject.SetActive(true);
                }
                else
                {
                    _group.GetChild(i + 1).gameObject.SetActive(false);
                }
            }
        }

        private void OnAddButtonClick()
        {
            listData.AddNew();
            UpdateUI();
        }

        private void OnRemoveButtonClick()
        {
            listData.RemoveLast();
            UpdateUI();
        }
    }

}
