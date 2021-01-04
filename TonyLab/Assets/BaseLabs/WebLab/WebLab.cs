using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebLab : MonoBehaviour
{
    
    void Start()
    {
        Debug.Log("提交报告");
        SendReportToWeb("报告内容");
    }
    /// <summary>
    /// webgl提交实验报告
    /// </summary>
    /// <param name="jsonReslut">json格式报告字符串</param>
    public void SendReportToWeb(string jsonReslut)
    {
        Application.ExternalCall("ReportEdit", jsonReslut);
    }
}
