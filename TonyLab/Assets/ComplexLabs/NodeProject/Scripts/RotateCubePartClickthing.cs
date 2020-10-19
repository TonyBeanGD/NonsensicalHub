
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class RotateCubePartClickthing : MonoBehaviour
{

    protected virtual void OnMouseEnter()
    {
        //NotificationCenter.Instance().PostDispatch(ActionType.PartEnter, new Notification( this));
    }

    protected virtual void OnMouseUp()
    {
        //NotificationCenter.Instance().PostDispatch(ActionType.PartClick, new Notification(transform.localPosition, this));
    }

    protected virtual void OnMouseExit()
    {
        //NotificationCenter.Instance().PostDispatch(ActionType.PartExit, new Notification( this));
    }
}
