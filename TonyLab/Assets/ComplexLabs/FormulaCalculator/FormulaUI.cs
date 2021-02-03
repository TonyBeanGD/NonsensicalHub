using NonsensicalKit;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class FormulaUI : NonsensicalMono
{
    [SerializeField] private Text txt_Name;
    [SerializeField] private Text txt_Time;
    [SerializeField] private Text txt_Input;
    [SerializeField] private Text txt_Output;
    [SerializeField] private Button btn_Delete;

    public Formula formula;
    
    protected override void Awake()
    {
        base.Awake();

        btn_Delete.onClick.AddListener(OnDeleteClick);
    }

    private void Start()
    {
        txt_Name.text = formula.Name;
        txt_Time.text = formula.Time.ToString();

        StringBuilder sb1=new StringBuilder();
        foreach (var item in formula.Input)
        {
            sb1.Append(FormulaCalculator.platform.GetName( item.Key));
            sb1.Append(":");
            sb1.Append(item.Value);
            sb1.Append(",");
        }
        txt_Input.text = sb1.ToString();
        StringBuilder sb2 = new StringBuilder();
        foreach (var item in formula.Output)
        {
            sb2.Append(FormulaCalculator.platform.GetName(item.Key));
            sb2.Append(":");
            sb2.Append(item.Value);
            sb2.Append(",");
        }
        txt_Output.text = sb2.ToString();
    }

    private void OnDeleteClick()
    {
        MessageAggregator<Formula>.Instance.Publish((uint)FormulaCalculatorAction.RemoveFormula, formula);
        Destroy(gameObject);
    }
}
