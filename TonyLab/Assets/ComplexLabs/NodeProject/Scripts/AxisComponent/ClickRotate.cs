using System;
using UnityEngine;

public class ClickRotate : MonoBehaviour
{
//    public AxisDirection dir;//当前脚本挂载的轴的方向

//    private void OnMouseDown()
//    {
//        NotificationCenter.Instance().PostDispatch(ActionType.RotAxisActivation, new Notification(dir, this));
//        NotificationCenter.Instance().PostDispatch(ActionType.AxisMouseDown, new Notification(this.transform, this));
//    }

//    private void OnMouseUp()
//    {
//        NotificationCenter.Instance().PostDispatch(ActionType.RotAxisDormancy, new Notification(this));
//        NotificationCenter.Instance().PostDispatch(ActionType.AxisMouseUp, new Notification(new Tuple<Transform, Action>(this.transform, CloseHighLight), this));
//    }

//    private void OnMouseEnter()
//    {
//        NotificationCenter.Instance().PostDispatch(ActionType.AxisMouseEnter, new Notification(new Tuple<Transform, Action>(this.transform, OpenHighLight), this));
//    }

//    private void OnMouseExit()
//    {
//        NotificationCenter.Instance().PostDispatch(ActionType.AxisMouseExit, new Notification(new Tuple<Transform, Action>(this.transform, CloseHighLight), this));
//    }

//    private void OpenHighLight()
//    {
//        transform.GetChild(0).GetComponent<MeshRenderer>().material.SetInt("_MainIntensity", 0);
//        GetComponent<MeshRenderer>().material.SetInt("_MainIntensity", 0);
//        transform.GetChild(0).GetComponent<MeshRenderer>().material.SetInt("_SecondIntensity", 1);
//        GetComponent<MeshRenderer>().material.SetInt("_SecondIntensity", 1);
//    }

//    private void CloseHighLight()
//    {
//        transform.GetChild(0).GetComponent<MeshRenderer>().material.SetInt("_MainIntensity", 1);
//        GetComponent<MeshRenderer>().material.SetInt("_MainIntensity", 1);
//        transform.GetChild(0).GetComponent<MeshRenderer>().material.SetInt("_SecondIntensity", 0);
//        GetComponent<MeshRenderer>().material.SetInt("_SecondIntensity", 0);
//    }
}