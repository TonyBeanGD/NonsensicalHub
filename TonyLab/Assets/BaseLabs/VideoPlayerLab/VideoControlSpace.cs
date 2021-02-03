using NonsensicalKit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VideoControlSpace : UIBase, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void OnControlPointHandle();
    public event OnControlPointHandle OnControlPointEnter;
    public event OnControlPointHandle OnControlPointExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnControlPointEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnControlPointExit();
    }
}
