using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using NonsensicalKit;

public class BackpackItem : MonoBehaviour
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private Button button;
    [SerializeField]
    private Text txt_Num;
    [SerializeField]
    private Text txt_Name;

    private int num;

    private BackpackItemInfo buffer;

    public void SetOne()
    {
        num++;
        txt_Num.text = num.ToString();
    }

    public void GetOne()
    {
      GameObject go=  ResourcesHelper.Load<GameObject>(buffer.Name);

        DraggingModel2D m2i = go.AddComponent<DraggingModel2D>();

        m2i.Init(buffer.Name, buffer);
      
        num--;
        txt_Num.text = num.ToString();

        if (num <= 0)
        {
            MessageAggregator<string>.Instance.Publish((uint)UIEnum.RemoveItemInBackpack, new MessageArgs<string>(this, buffer.Name));

            Destroy(gameObject);
        }
    }

    public void Init(BackpackItemInfo itemInfo)
    {
        buffer = itemInfo;

        image.sprite = Resources.Load<Sprite>(itemInfo.ImgPath);
        image.preserveAspect = true;

        button.onClick.AddListener(OnClick);

        num = 1;
        txt_Num.text = num.ToString();

        txt_Name.text = itemInfo.Name;
    }

    private void OnClick()
    {
        MessageAggregator<BackpackItemInfo>.Instance.Publish((uint)UIEnum.BackpackGridClick,new MessageArgs<BackpackItemInfo>(this,buffer));
    }
}
