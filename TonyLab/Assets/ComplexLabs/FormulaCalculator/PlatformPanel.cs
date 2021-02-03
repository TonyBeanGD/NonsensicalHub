using NonsensicalKit;
using NonsensicalKit.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformPanel : NonsensicalMono
{
    [SerializeField] private Button btn_Add;
    [SerializeField] private Button btn_Run;
    [SerializeField] private Button btn_Save;
    [SerializeField] private Button btn_Load;
    [SerializeField] private Transform tableParent;
    [SerializeField] private Text input;
    [SerializeField] private Text output;
    [SerializeField] private InputField ipf_ProgrammeName;
    
    Programme programme;
    
    protected override void Awake()
    {
        base.Awake();
        Subscribe<Tuple<string, string>>((uint)FormulaCalculatorAction.ShowCalculatorResult, ShowResult);
        btn_Add.onClick.AddListener(AddOne);
        btn_Run.onClick.AddListener(Run);
        btn_Load.onClick.AddListener(Load);
        btn_Save.onClick.AddListener(Save);

        programme = JsonHelper.LoadFile<Programme>("Programme");
        if (programme == null)
        {
            programme = new Programme();
        }
    }

    private void AddOne()
    {
        GameObject go = Instantiate(tableParent.GetChild(0).gameObject, tableParent);
        go.SetActive(true);
    }

    private void Run()
    {
        Dictionary<string, float> keyValuePairs = new Dictionary<string, float>();
        for (int i = 1; i < tableParent.childCount; i++)
        {
            keyValuePairs.Add(tableParent.GetChild(i).GetChild(0).GetComponent<InputField>().text, float.Parse(tableParent.GetChild(i).GetChild(1).GetComponent<InputField>().text));
        }

        Publish((uint)FormulaCalculatorAction.RunCalculator, keyValuePairs);
    }

    private void Load()
    {
        string name = ipf_ProgrammeName.text;
        if (programme.programmes.ContainsKey(name))
        {
            Dictionary<string, float> keyValuePairs = programme.programmes[name];
            for (int i = 1; i < tableParent.childCount; i++)
            {
                Destroy(tableParent.GetChild(i).gameObject);
            }
            foreach (var item in keyValuePairs)
            {
                GameObject go = Instantiate(tableParent.GetChild(0).gameObject, tableParent);
                go.transform.GetChild(0).GetComponent<InputField>().text = item.Key;
                go.transform.GetChild(1).GetComponent<InputField>().text =item.Value.ToString();
                go.SetActive(true);
            }
        }
        else
        {
            NonsensicalDebugger.LogOnGUI("不存在目标存档");
        }
    }

    private void Save()
    {
        string name = ipf_ProgrammeName.text;
        Dictionary<string, float> keyValuePairs = new Dictionary<string, float>();
        for (int i = 1; i < tableParent.childCount; i++)
        {
            keyValuePairs.Add(tableParent.GetChild(i).GetChild(0).GetComponent<InputField>().text, float.Parse(tableParent.GetChild(i).GetChild(1).GetComponent<InputField>().text));
        }

        if (programme.programmes.ContainsKey(name))
        {
            programme.programmes[name] = keyValuePairs;
        }
        else
        {
            programme.programmes.Add(name, keyValuePairs);
        }
    }

    private void ShowResult(Tuple<string, string> result)
    {
        input.text = result.Item1;
        output.text = result.Item2;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (programme != null)
        {
            JsonHelper.SaveFile<Programme>("Programme", programme);
        }
    }
}

public class Programme
{
   public Dictionary<string, Dictionary<string, float>> programmes = new Dictionary<string, Dictionary<string, float>>();
}