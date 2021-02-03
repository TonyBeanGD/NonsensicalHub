using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VideoProgressSlider : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public delegate void ProgressSliderDragHandle(bool isDrag);
    public event ProgressSliderDragHandle OnProgressSliderDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnProgressSliderDrag(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnProgressSliderDrag(false);
    }
}
