using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemLab : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler,
    IDragHandler, IBeginDragHandler, IEndDragHandler, ISubmitHandler, ICancelHandler, IMoveHandler, ISelectHandler, IDropHandler, IScrollHandler, IInitializePotentialDragHandler, IUpdateSelectedHandler, IDeselectHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        //当鼠标置入挂载物体上时启用
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //鼠标放在挂载物体上，按下鼠标左键或右键时启用
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //鼠标放在挂载物体上，按下鼠标左键或右键后，在挂载物体上放开对应鼠标按键时启用
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        //当鼠标放在挂载物体上，按下鼠标左键或右键，在任意地方放开对应鼠标按键时启用
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ///当鼠标移出挂载物体上时启用
    }



    public void OnDrag(PointerEventData eventData)
    {
        //鼠标放在挂载物体上，按下鼠标左键或右键后，在放开对应鼠标按键前，鼠标移动时每帧启用
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //鼠标放在挂载物体上，按下鼠标左键或右键后，在放开对应鼠标按键前，第一次移动时启用
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //当挂载物体的OnDrag事件正在运行时（移动过），放开对应鼠标按键时启用
        //鼠标放在挂载物体上，按下鼠标左键或右键后，假如移动过鼠标，放开对应鼠标按键时就会启用
    }

    public void OnDrop(PointerEventData eventData)
    {
        //鼠标放在挂载物体上，当前有其他物体的OnDrag事件正在运行时（移动过），放开对应鼠标按键时启用
    }



    public void OnScroll(PointerEventData eventData)
    {
        //当鼠标放在挂载物体上，滚动滚轮时运行
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        //当鼠标放在挂载物体上，并且挂载物体上的脚本中有OnDrag事件时，按下鼠标左键或右键时运行，运行时机在OnPoint后
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
        //当挂载物体被选中后（要含如Toggle一类的组件），点击挂载物体以外地方前，每帧调用
    }

    public void OnSelect(BaseEventData eventData)
    {
        //当挂载物体被选中时（要含如Toggle一类的组件）启用
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //当挂载物体被取消选中时（要含如Toggle一类的组件）启用
    }

    public void OnMove(AxisEventData eventData)
    {
        //当挂载物体被选中时（要含如Toggle一类的组件），InputManager里的Horizontal和Vertical改变时启用
    }

    public void OnSubmit(BaseEventData eventData)
    {
        //当挂载物体被选中时（要含如Toggle一类的组件），InputManager里的Submit按键（PC默认为Enter键）被按下时启用
    }

    public void OnCancel(BaseEventData eventData)
    {
        //当挂载物体被选中时（要含如Toggle一类的组件），InputManager里的Cancel按键（PC默认为Esc键）被按下时启用
    }

    /*
eventData:返回鼠标的屏幕坐标

eventData.button：触发此方法时按下的鼠标按键，返回值为：PointerEventData.InputButton.Left（左键），PointerEventData.InputButton.Right（右键），PointerEventData.InputButton.Middle（滚轮）三种

eventData.clickcount：连续点击次数, 返回值类型：int

eventData.clickTime：最近一次完成点击（click）的时间, 比Time.time要快1.25s到1.27s, 返回值类型：float

eventData.currentInputModule：未知

eventData.delta：
    */
}
