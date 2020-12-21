using NonsensicalKit.UI;
using NonsensicalKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PopupManager : UIBase
{
    [SerializeField] private GameObject mask;

    [SerializeField] private Text txt_Title;
    [SerializeField] private Text txt_Message;
    [SerializeField] private Image img_Icon;
    [SerializeField] private Text txt_LeftButton;
    [SerializeField] private Text txt_RightButton;
    [SerializeField] private Button btn_Left;
    [SerializeField] private Button btn_Right;

    protected override void Awake()
    {
        base.Awake();
        MessageAggregator<PopupArgs>.Instance.Subscribe((uint) UIEnum.Popup, Popup);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        MessageAggregator<PopupArgs>.Instance.Unsubscribe((uint)UIEnum.Popup, Popup);
    }

    private void Popup(MessageArgs<PopupArgs> value)
    {
        PopupArgs popupArgs = value.Item;

        txt_Title.text = popupArgs.Title;
        txt_Message.text = popupArgs.Message;
        img_Icon.sprite =Resources.Load<Sprite>(popupArgs.IconPath);
        txt_LeftButton.text = popupArgs.LeftButtonText;
        txt_RightButton.text = popupArgs.RightButtonText;
        btn_Left.onClick.RemoveAllListeners();
        btn_Left.onClick.AddListener(()=> { popupArgs.LeftButtonAction?.Invoke(); });
        btn_Right.onClick.RemoveAllListeners();
        btn_Right.onClick.AddListener(()=> { popupArgs.RightButtonAction?.Invoke(); });
    }
}

public class PopupArgs
{
    public string Title;
    public string Message;
    public string IconPath;
    public string LeftButtonText;
    public string RightButtonText;
    public Action LeftButtonAction;
    public Action RightButtonAction;
}