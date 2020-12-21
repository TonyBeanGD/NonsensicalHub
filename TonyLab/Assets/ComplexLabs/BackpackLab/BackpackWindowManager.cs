using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using NonsensicalKit;
using NonsensicalKit.UI;

public class BackpackWindowManager : UIBase
{
    [SerializeField]
    private Transform buttonsGroup;
    [SerializeField]
    private Scrollbar scrollbar;
    [SerializeField]
    private Button btn_Topping;

    private Dictionary<string, BackpackItem> items;

    protected override void Awake()
    {
        base.Awake();

        items = new Dictionary<string, BackpackItem>();

        MessageAggregator<int>.Instance.Subscribe((uint)UIEnum.SwitchBackpackWindow, SwitchBackpackWindow);
        MessageAggregator<string>.Instance.Subscribe((uint)UIEnum.RemoveItemInBackpack, RemoveItemInBackpack);
        MessageAggregator<BackpackItemInfo>.Instance.Subscribe((uint)UIEnum.SetItemToBackpack, SetItemToBackpack);
        MessageAggregator<BackpackItemInfo>.Instance.Subscribe((uint)UIEnum.GetItemFormBackpack, GetItemFromBackpack);

        btn_Topping.onClick.AddListener(() => { scrollbar.value = 1; });
    }


    protected override void OnDestroy()
    {
        MessageAggregator<int>.Instance.Unsubscribe((uint)UIEnum.SwitchBackpackWindow, SwitchBackpackWindow);
        MessageAggregator<string>.Instance.Unsubscribe((uint)UIEnum.RemoveItemInBackpack, RemoveItemInBackpack);
        MessageAggregator<BackpackItemInfo>.Instance.Unsubscribe((uint)UIEnum.SetItemToBackpack, SetItemToBackpack);
        MessageAggregator<BackpackItemInfo>.Instance.Unsubscribe((uint)UIEnum.GetItemFormBackpack, GetItemFromBackpack);
    }

    private void SwitchBackpackWindow(MessageArgs<int> value)
    {
        SwitchSelf();
    }


    private void AutoFit()
    {
        StartCoroutine(Fitting());
    }

    private IEnumerator Fitting()
    {
        buttonsGroup.GetComponent<ContentSizeFitter>().enabled = false;

        yield return null;
        buttonsGroup.GetComponent<ContentSizeFitter>().enabled = true;

        yield return null;
        scrollbar.value = 1;
    }

    private void SetItemToBackpack(MessageArgs<BackpackItemInfo> value)
    {
        BackpackItemInfo itemInfo = value.Item;

        if (items.ContainsKey(itemInfo.Name))
        {
            items[itemInfo.Name].SetOne();
        }
        else
        {
            GameObject newButton = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("UIPrefabs/btn_GroupImageElement"), buttonsGroup);

            BackpackItem backpackItem = newButton.GetComponent<BackpackItem>();

            items.Add(itemInfo.Name, backpackItem);

            backpackItem.Init(itemInfo);
        }

        AutoFit();
    }

    private void GetItemFromBackpack(MessageArgs<BackpackItemInfo> value)
    {
        BackpackItemInfo itemInfo = value.Item;

        if (items.ContainsKey(itemInfo.Name))
        {
            items[itemInfo.Name].GetOne();
        }
        else
        {
            Debug.LogWarning("试图获取没有剩余的Item" + itemInfo.Name);
        }
    }

    private void RemoveItemInBackpack(MessageArgs<string> value)
    {
        string name = value.Item;

        if (items.ContainsKey(name))
        {
            items.Remove(name);
        }
        else
        {
            Debug.LogWarning("试图移除已经没有剩余的Item" + name);
        }

        AutoFit();
    }
}

public class BackpackItemInfo
{
    public string Name;
    public string ImgPath;
    public int Num;

    public BackpackItemInfo(string name, string imgPath)
    {
        this.Name = name;
        this.ImgPath = imgPath;
        this.Num = 1;
    }

    public BackpackItemInfo Clone()
    {
        BackpackItemInfo temp = new BackpackItemInfo(this.Name, this.ImgPath);
        temp.Num = Num;
        return temp;
    }

}
