using System;
using UnityEngine;

//public enum AxisDirection
//{
//    x,y,z,
//}

public class ClickMove : MonoBehaviour
{
//    [SerializeField]
//    private AxisDirection dir;//当前脚本挂载的轴的方向
//    [SerializeField]
//    private AxisDirection secondDir;//当前脚本挂载的轴的第二方向

//    private void OnMouseDown()
//    {
//        NotificationCenter.Instance().PostDispatch(ActionType.PosAxisActivation, new Notification(new Tuple<AxisDirection, AxisDirection>(dir, secondDir), this));
//        NotificationCenter.Instance().PostDispatch(ActionType.AxisMouseDown, new Notification(this.transform, this));
//    }

//    private void OnMouseUp()
//    {
//        NotificationCenter.Instance().PostDispatch(ActionType.PosAxisDormancy, new Notification(this));
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

//    public void OpenHighLight()
//    {
//        GetComponent<MeshRenderer>().material.SetInt("_MainIntensity", 0);
//        GetComponent<MeshRenderer>().material.SetInt("_SecondIntensity", 1);
//    }
//    //关闭高亮
//    public void CloseHighLight()
//    {
//        GetComponent<MeshRenderer>().material.SetInt("_MainIntensity", 1);
//        GetComponent<MeshRenderer>().material.SetInt("_SecondIntensity", 0);
//    }
}