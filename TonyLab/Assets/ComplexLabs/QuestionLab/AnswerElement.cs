using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NonsensicalKit;
using NonsensicalKit.UI;

public class AnswerElement : ListElementBase<AnswerArgs>, ICanEdit
{
    [SerializeField] private Text txt_Answer;

    [SerializeField] private InputField ipf_Answer;

    [SerializeField] private Toggle tog_Answer;

    [SerializeField] private Transform img_Selected;

    [SerializeField] private Button btn_Top;
    
    protected override void Awake()
    {
        MessageAggregator<bool>.Instance.Subscribe((uint)UIEnum.CanEditSwitch, OnCanEditSwitchFunc);

        tog_Answer.onValueChanged.AddListener(OnToggleChoice);
        btn_Top.onClick.AddListener(OnTopSort);
        ipf_Answer.onEndEdit.AddListener(OnEditEnd);
    }

    protected override void OnDestroy()
    {
        MessageAggregator<bool>.Instance.Unsubscribe((uint)UIEnum.CanEditSwitch, OnCanEditSwitchFunc);
    }

    private void OnToggleChoice(bool isSelect)
    {
        if (isSelect != this._isSelect)
        {
            _isSelect = isSelect;
            UpdateUI();
        }
    }

    private void OnEditEnd(string text)
    {
        if (text!=_elementData.Answer)
        {
            _elementData.Answer = text;
            UpdateUI();
        }
    }

    private void OnCanEditSwitchFunc(bool value)
    {
        CanEditSwitch(value);
        UpdateUI();
    }

    public override void Init(AnswerArgs data)
    {
        _elementData = data;
        UpdateUI();
    }

    public void CanEditSwitch(bool canEdit)
    {
        ipf_Answer.gameObject.SetActive(canEdit);
    }
    
    protected override void UpdateUI()
    {
        txt_Answer.text = _elementData.Answer;
        ipf_Answer.text = _elementData.Answer;

        SelectSwitch(_isSelect);
    }

    protected override void SelectSwitch(bool isSelect)
    {
        img_Selected.gameObject.SetActive(isSelect);
    }
}
