using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NonsensicalKit.UI;
using NonsensicalKit;
using UnityEngine.UI;
using NonsensicalKit.Utility;
using LitJson;
using System;
using System.Text;

public enum FormulaCalculatorAction
{
    AddItem = 5423,
    RemoveItem,
    AddFormula,
    RemoveFormula,
    RunCalculator,
    ShowCalculatorResult
}



public class FormulaCalculator : NonsensicalMono
{
    [SerializeField] private Transform itemsParent;
    [SerializeField] private Transform formulasParent;

    public static Platform platform;

    protected override void Awake()
    {
        base.Awake();
        Subscribe<Item>((uint)FormulaCalculatorAction.AddItem, AddItem);
        Subscribe<Item>((uint)FormulaCalculatorAction.RemoveItem, RemoveItem);
        Subscribe<Formula>((uint)FormulaCalculatorAction.AddFormula, AddFormula);
        Subscribe<Formula>((uint)FormulaCalculatorAction.RemoveFormula, RemoveFormula);
        Subscribe<Dictionary<string, float>>((uint)FormulaCalculatorAction.RunCalculator, RunCalculator);

        platform = JsonHelper.LoadFile<Platform>("Platform");
        if (platform == null)
        {
            platform = new Platform();
        }
        else
        {
            foreach (var item in platform.Items)
            {
                GameObject newGo = Instantiate(itemsParent.GetChild(0).gameObject, itemsParent);
                newGo.GetComponent<ItemUI>().item = item;
                newGo.SetActive(true);
            }

            foreach (var formula in platform.Formulas)
            {
                GameObject newGo = Instantiate(formulasParent.GetChild(0).gameObject, formulasParent);
                newGo.GetComponent<FormulaUI>().formula = formula;
                newGo.SetActive(true);
            }
        }
    }

    private void AddItem(Item newItem)
    {
        if (platform.ContainItem(newItem.ID))
        {
            NonsensicalDebugger.LogOnGUI("ID重复");
            return;
        }

        GameObject newGo = Instantiate(itemsParent.GetChild(0).gameObject, itemsParent);
        newGo.GetComponent<ItemUI>().item = newItem;
        newGo.SetActive(true);

        platform.Items.Add(newItem);
    }

    private void RemoveItem(Item item)
    {
        if (platform.Items.Contains(item))
        {
            platform.Items.Remove(item);
        }
    }

    private void AddFormula(Formula formula)
    {
        if (platform.ContainFormula(formula.Name))
        {
            NonsensicalDebugger.LogOnGUI("名字重复");
            return;
        }
        GameObject newGo = Instantiate(formulasParent.GetChild(0).gameObject, formulasParent);
        newGo.GetComponent<FormulaUI>().formula = formula;
        newGo.SetActive(true);
        platform.Formulas.Add(formula);
    }

    private void RemoveFormula(Formula item)
    {
        if (platform.Formulas.Contains(item))
        {
            platform.Formulas.Remove(item);
        }
    }

    private void RunCalculator(Dictionary<string, float> keyValuePairs)
    {
        Dictionary<Formula, float> keyValuePairs2 = new Dictionary<Formula, float>();
        foreach (var item in keyValuePairs)
        {
            Formula formula = platform.GetFormula(item.Key);
            if (formula == null)
            {
                NonsensicalDebugger.LogOnGUI("没有配方" + item.Key);
                return;
            }
            keyValuePairs2.Add(formula, item.Value);
        }
        Publish((uint)FormulaCalculatorAction.ShowCalculatorResult, GetInputOutput(keyValuePairs2));
    }

    private Tuple<string, string> GetInputOutput(Dictionary<Formula, float> formulas)
    {
        Dictionary<int, double> product = new Dictionary<int, double>();
        Dictionary<int, double> demand = new Dictionary<int, double>();
        foreach (var formula in formulas)
        {
            foreach (var input in formula.Key.Input)
            {
                double needCount = input.Value / formula.Key.Time * formula.Value;
                if (product.ContainsKey(input.Key) == false)
                {
                    product.Add(input.Key, 0);
                }

                if (product[input.Key] > needCount)
                {
                    product[input.Key] -= needCount;
                }
                else
                {
                    needCount -= product[input.Key];
                    product[input.Key] = 0;
                    if (demand.ContainsKey(input.Key) == false)
                    {
                        demand.Add(input.Key, 0);
                    }

                    demand[input.Key] += needCount;
                }
            }

            foreach (var output in formula.Key.Output)
            {
                double createCount = output.Value / formula.Key.Time * formula.Value;
                if (demand.ContainsKey(output.Key) == false)
                {
                    demand.Add(output.Key, 0);
                }
                if (demand[output.Key] > createCount)
                {
                    demand[output.Key] -= createCount;
                }
                else
                {
                    createCount -= demand[output.Key];
                    demand[output.Key] = 0;
                    if (product.ContainsKey(output.Key) == false)
                    {
                        product.Add(output.Key, 0);
                    }
                    product[output.Key] += createCount;
                }
            }
        }

        StringBuilder sb1 = new StringBuilder();
        sb1.Append("每秒输入\n");
        foreach (var item in demand)
        {
            if (item.Value > 0)
            {
                sb1.Append(platform.GetName(item.Key));
                sb1.Append(":");
                sb1.Append(item.Value);
                sb1.Append(";\n");
            }

        }

        StringBuilder sb2 = new StringBuilder();
        sb2.Append("每秒输出\n");
        foreach (var item in product)
        {
            if (item.Value > 0)
            {
                sb2.Append(platform.GetName(item.Key));
                sb2.Append(":");
                sb2.Append(item.Value);
                sb2.Append(";\n");
            }
        }

        return new Tuple<string, string>(sb1.ToString(), sb2.ToString());
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (platform != null)
        {
            JsonHelper.SaveFile<Platform>("Platform", platform);
        }
    }
}

/// <summary>
/// 平台
/// </summary>
public class Platform
{
    public List<Item> Items = new List<Item>();
    public List<Formula> Formulas = new List<Formula>();

    public string GetName(int id)
    {
        foreach (var item in Items)
        {
            if (id == item.ID)
            {
                return item.Name;
            }
        }
        return null;
    }

    public bool ContainItem(int id)
    {
        foreach (var item in Items)
        {
            if (id == item.ID)
            {
                return true;
            }
        }
        return false;
    }

    public bool ContainFormula(string name)
    {
        foreach (var item in Formulas)
        {
            if (name == item.Name)
            {
                return true;
            }
        }
        return false;
    }

    public Formula GetFormula(string name)
    {
        foreach (var item in Formulas)
        {
            if (item.Name == name)
            {
                return item;
            }
        }
        return null;
    }

}

/// <summary>
/// 物料
/// </summary>
public class Item
{
    /// <summary>
    /// 唯一ID
    /// </summary>
    public int ID;
    /// <summary>
    /// 名字
    /// </summary>
    public string Name;

    public Item(int iD, string name)
    {
        ID = iD;
        Name = name;
    }

    public override string ToString()
    {
        return $"{ID}({Name})";
    }
}

/// <summary>
/// 配方
/// </summary>
public class Formula
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name;
    /// <summary>
    /// 时间（s）
    /// </summary>
    public double Time;
    /// <summary>
    /// 输入(ID:数量)
    /// </summary>
    public Dictionary<int, int> Input = new Dictionary<int, int>();
    /// <summary>
    /// 输出(ID:数量)
    /// </summary>
    public Dictionary<int, int> Output = new Dictionary<int, int>();

    public Formula(string name, double time, Dictionary<int, int> input, Dictionary<int, int> output)
    {
        Name = name;
        Time = time;
        Input = input;
        Output = output;
    }
}