using NonsensicalKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddItemPanel : NonsensicalMono
{

    [SerializeField] private InputField ipf_ItemID;
    [SerializeField] private InputField ipf_ItemName;
    [SerializeField] private Button btn_AddItem;

    protected override void Awake()
    {
        base.Awake();

        btn_AddItem.onClick.AddListener(OnAddItemClick);
    }

    private void OnAddItemClick()
    {
        int id;
        if (int.TryParse(ipf_ItemID.text, out id))
        {
            Item newItem = new Item(id, ipf_ItemName.text);
            if (newItem != null)
            {
                MessageAggregator<Item>.Instance.Publish((uint)FormulaCalculatorAction.AddItem, newItem);
            }
        }
    }
}
