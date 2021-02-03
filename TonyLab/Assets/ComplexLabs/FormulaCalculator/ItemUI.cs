using NonsensicalKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI :NonsensicalMono {

    [SerializeField] private Text txt_ID_Name;
    [SerializeField] private Button btn_Delete;

    public Item item;

 
    protected override void Awake()
    {
        base.Awake();

        btn_Delete.onClick.AddListener(OnDeleteClick);
    }

    private void Start()
    {
        txt_ID_Name.text=item.ID+":"+item.Name;
    }

    private void OnDeleteClick()
    {
        MessageAggregator<Item>.Instance.Publish((uint)FormulaCalculatorAction.RemoveItem, item);
        Destroy(gameObject);
    }

}
