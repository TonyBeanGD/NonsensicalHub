using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SwitchButton : MonoBehaviour {

    [SerializeField] private CanvasGroup target;
    [SerializeField] private bool initState;

    [SerializeField] private CanvasGroup[] associatedObjects;

    private Button btn_Self;
    private void Awake()
    {
        btn_Self = GetComponent<Button>();
        btn_Self.onClick.AddListener(OnSwitch);
        SwitchCanvasGroup(target,initState);
    }

    private void OnSwitch()
    {
        if (target.alpha == 0)
        {
            foreach (var item in associatedObjects)
            {
                SwitchCanvasGroup(item,false);
            }
        }
        SwitchCanvasGroup(target, target.alpha == 0);

    }

    private void SwitchCanvasGroup(CanvasGroup cg ,bool targetState)
    {
        if (targetState)
        {
            cg.alpha = 1;
            cg.blocksRaycasts = true;
        }
        else
        {
            cg.alpha = 0;
            cg.blocksRaycasts = false;
        }
    }
}
