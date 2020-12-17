using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionQuestionLab : MonoBehaviour
{
    [SerializeField] private Button[] _buttonGroup1;
    [SerializeField] private Button[] _buttonGroup2;
    [SerializeField] private int[] _correctAnswer;

    private bool[] buffer;

    private int _crtIndex;

    private void Awake()
    {
        if (_buttonGroup1.Length != _buttonGroup2.Length || _buttonGroup1.Length != _correctAnswer.Length)
        {
            Debug.LogWarning("连线题数量设置不匹配");
            return;
        }

        buffer = new bool[_buttonGroup1.Length * 2];
        _crtIndex = -1;
        InitButton();
    }


    private void InitButton()
    {
        for (int i = 0; i < _buttonGroup1.Length; i++)
        {
            int tempInt = i;
            _buttonGroup1[tempInt].onClick.AddListener(() => OnButtonClick(tempInt));
        }
        for (int i = 0; i < _buttonGroup2.Length; i++)
        {
            int tempInt = i;
            _buttonGroup2[tempInt].onClick.AddListener(() => OnButtonClick(_buttonGroup2.Length + tempInt));
        }
    }

    private void OnButtonClick(int index)
    {
        if (_crtIndex == -1)
        {
            _crtIndex = index;
            buffer[index] = true;
        }
        else
        {
            if ((index >= _buttonGroup1.Length && _crtIndex >= _buttonGroup1.Length) || (index < _buttonGroup1.Length && _crtIndex < _buttonGroup1.Length))
            {
                Debug.LogWarning("两次选择了同一组的对象");
            }
            else
            {
                if (buffer[index] == true)
                {
                    Debug.LogWarning("选择了已经被选择过的对象");
                }
                else
                {
                    int min = Mathf.Min(index, _crtIndex);
                    int max = Mathf.Max(index, _crtIndex);
                    if (_correctAnswer[min] == max - _buttonGroup1.Length)
                    {
                        Debug.Log("选择正确");
                    }
                    else
                    {
                        Debug.Log("选择错误");
                    }

                    buffer[index] = true;
                    _crtIndex = -1;
                }
            }
        }
    }
}
