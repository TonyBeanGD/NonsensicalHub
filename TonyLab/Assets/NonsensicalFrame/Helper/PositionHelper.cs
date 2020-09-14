﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionHelper
{
    /// <summary>
    /// 获取鼠标位置的世界坐标（深度由所选物体决定）
    /// </summary>
    /// <returns></returns>
    private Vector3 GetWorldPos(Transform target)
    {
        //获取需要移动物体的世界转屏幕坐标
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
        //获取鼠标位置
        Vector3 mousePos = Input.mousePosition;
        //因为鼠标只有X，Y轴，所以要赋予给鼠标Z轴
        mousePos.z = screenPos.z;
        //把鼠标的屏幕坐标转换成世界坐标
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
