using NonsensicalKit;
using NonsensicalKit.UI;
using NonsensicalKit.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddFormulaPanel : UIBase
{
    [SerializeField] private InputField ipf_FormulaName;
    [SerializeField] private InputField ipf_FormulaTime;
    [SerializeField] private Transform inputParent;
    [SerializeField] private Transform outputParent;
    [SerializeField] private Button btn_AddInput;
    [SerializeField] private Button btn_RemoveInput;
    [SerializeField] private Button btn_AddOutput;
    [SerializeField] private Button btn_RemoveOutput;

    [SerializeField] private Button btn_AddFormula;

    protected override void Awake()
    {
        base.Awake();
        btn_AddInput.onClick.AddListener(OnAddInputClick);
        btn_RemoveInput.onClick.AddListener(OnRemoveInputClick);
        btn_AddOutput.onClick.AddListener(OnAddOutputClick);
        btn_RemoveOutput.onClick.AddListener(OnRemoveOutputClick);
        btn_AddFormula.onClick.AddListener(OnAddFormulaClick);
    }

    private void OnAddInputClick()
    {
        GameObject newGo = Instantiate(inputParent.GetChild(0).gameObject, inputParent);
        newGo.SetActive(true);
    }

    private void OnRemoveInputClick()
    {
        if (inputParent.childCount > 1)
        {
            Destroy(inputParent.GetChild(inputParent.childCount - 1).gameObject);
        }
    }

    private void OnAddOutputClick()
    {
        GameObject newGo = Instantiate(outputParent.GetChild(0).gameObject, outputParent);
        newGo.SetActive(true);
    }
    private void OnRemoveOutputClick()
    {
        if (outputParent.childCount > 1)
        {
            Destroy(outputParent.GetChild(outputParent.childCount - 1).gameObject);
        }
    }
    private void OnAddFormulaClick()
    {
        string name = ipf_FormulaName.text;
        double time = double.Parse(ipf_FormulaTime.text);

        Dictionary<int, int> inputs = new Dictionary<int, int>();

        for (int i = 1; i < inputParent.childCount; i++)
        {

            int id = int.Parse(inputParent.GetChild(i).GetChild(0).GetComponent<InputField>().text);
            int count = int.Parse(inputParent.GetChild(i).GetChild(1).GetComponent<InputField>().text);


            if (inputs.ContainsKey(id))
            {
                NonsensicalDebugger.LogOnGUI("输入ID重复");
                return;
            }
            inputs.Add(id, count);
        }

        if (inputs.Count == 0)
        {
            NonsensicalDebugger.LogOnGUI("无输入");
            return;
        }
        Dictionary<int, int> outputs = new Dictionary<int, int>();

        for (int i = 1; i < outputParent.childCount; i++)
        {

            int id = int.Parse(outputParent.GetChild(i).GetChild(0).GetComponent<InputField>().text);
            int count = int.Parse(outputParent.GetChild(i).GetChild(1).GetComponent<InputField>().text);


            if (outputs.ContainsKey(id))
            {
                NonsensicalDebugger.LogOnGUI("输出ID重复");
                return;
            }
            outputs.Add(id, count);
        }
        if (outputs.Count == 0)
        {
            NonsensicalDebugger.LogOnGUI("无输出");
            return;
        }
        Formula newFormula = new Formula(name, time, inputs, outputs);

        Publish<Formula>((uint)FormulaCalculatorAction.AddFormula, newFormula);
    }
}
