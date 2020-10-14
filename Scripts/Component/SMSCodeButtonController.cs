using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SMSCodeButtonController : MonoBehaviour
{
    public int MaxTimer;

    private bool isRunning;
    private Button button;
    private Text text;
    private float Timer;


    void Update()
    {
        if (isRunning==true)
        {
            Timer -= Time.deltaTime;

            if (Timer < 0)
            {
                button.interactable = true;
                text.text = "获取验证码";
                isRunning = false;
            }
            else
            {
               text.text= "重新获取" + Timer.ToString("f2");
            }
        }
    }

    public void Init(Action action)
    {  
        button = transform.GetComponent<Button>();
        text = transform.GetChild(0).GetComponent<Text>();
        button.onClick.AddListener(()=>action?.Invoke());
    }

    public void Reset()
    {
        Timer = MaxTimer;
        isRunning = false;
        button.interactable = true;
        text.text ="获取验证码";
    }

    public void Run()
    {
        button.interactable = false;
        isRunning = true;
    }
}