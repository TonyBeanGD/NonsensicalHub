using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [HideInInspector]
    public static CanvasManager _Instance;

    public Transform _Canvas;

    [HideInInspector]
    public Camera _MainCamera;

    [HideInInspector]
    public int ScreenWidth;

    [HideInInspector]
    public int ScreenHeight;


    private GameObject _BlackMask;

    private void Awake()
    {
        Application.targetFrameRate = 144;
        _Instance = this;
        _Canvas = transform;
        _MainCamera = Camera.main;
        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;

        for (int i = 0; i < _Canvas.Find("Main").childCount; i++)
        {
            _Canvas.Find("Main").GetChild(i).gameObject.SetActive(true);
        }

        _Canvas.Find("LoadingPanel").gameObject.SetActive(true);

        _BlackMask = _Canvas.Find("BlackMask").gameObject;

        Mask(false);

        //#if UNITY_ANDROID
        //        int scWidth = Screen.width;
        //        int scHeight = Screen.height;
        //        int designWidth = 1334; //这个是设计分辨率
        //        int designHeight = 750;
        //        if (scWidth <= designWidth || scHeight <= designHeight)
        //            return;
        //        Screen.SetResolution(designWidth, designHeight, true);
        //#endif

    }

    public void Mask(bool _switch)
    {
        _BlackMask.SetActive(_switch);

        _Canvas.Find("Main").gameObject.SetActive(!_switch);

    }
}
