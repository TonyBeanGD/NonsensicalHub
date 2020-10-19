using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class TopUIBase : MonoBehaviour
{
    protected CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        canvasGroup = transform.GetComponent<CanvasGroup>();
        CloseSelf();
    }

    protected void OpenSelf()
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
    }

    protected void CloseSelf()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
    }

    protected void SwitchSelf()
    {
        if (canvasGroup.blocksRaycasts == true)
        {
            CloseSelf();
        }
        else
        {
            OpenSelf();
        }
    }

    protected void Open(CanvasGroup cg)
    {
        cg.blocksRaycasts = true;
        cg.alpha = 1;
    }

    protected void Close(CanvasGroup cg)
    {
        cg.blocksRaycasts = false;
        cg.alpha = 0;
    }

    protected void DelayTopping(Scrollbar scrollbar)
    {
        StartCoroutine(Topping(scrollbar));
    }

    private IEnumerator Topping(Scrollbar scrollbar)
    {
        yield return null;
        scrollbar.value = 1;
    }
}
