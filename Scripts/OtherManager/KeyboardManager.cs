using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DataEncryptDecrypt;
using System.IO;

public class KeyboardManager : MonoBehaviour
{
    private GameObject _Canvas;             //主画布

    private EventSystem _EventSystem;       //事件系统

    private bool _IsTiming;                 //是否开始计时
    private float _CountDown;               //倒计时

    private InputField[] _ifs;              //所有的InputField

    private void Start()
    {
        _Canvas = CanvasManager._Instance._Canvas.gameObject;
        _EventSystem = EventSystem.current;
        _ifs = _Canvas.GetComponentsInChildren<InputField>(true);
    }

    void Update()
    {
        TapSwitch();
        EixtDetection(); //调用 退出检测函数
    }

    /// <summary>
    /// tap键切换输入框选择
    /// </summary>
    private void TapSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            List<InputField> temp = new List<InputField>();

            foreach (var item in _ifs)
            {
                if (item.gameObject.activeInHierarchy)
                {
                    temp.Add(item);
                }
            }

            if (temp.Count == 0)
            {
                return;
            }

            int index = -1;

            for (int i = 0; i < temp.Count; i++)
            {
                if (EventSystem.current.currentSelectedGameObject == temp[i].gameObject)
                {
                    index = i;
                    break;
                }
            }

            if (index + 1 == temp.Count)
            {
                index = 0;
            }
            else
            {
                index += 1;
            }

            _EventSystem.SetSelectedGameObject(temp[index].gameObject, new BaseEventData(_EventSystem));
        }
    }

    /// <summary>
    /// 退出检测
    /// </summary>
    private void EixtDetection()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_CountDown == 0)
            {
                _CountDown = Time.time;
                _IsTiming = true;
            }
            else
            {
                GameStatic.QuitApplication();
            }
        }

        if (_IsTiming)
        {
            if ((Time.time - _CountDown) > 0.5f)
            {
                _CountDown = 0;
                _IsTiming = false;
            }
        }
    }
}
