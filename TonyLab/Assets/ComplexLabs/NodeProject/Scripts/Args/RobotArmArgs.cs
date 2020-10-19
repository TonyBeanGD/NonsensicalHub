using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//轴数据类型
public enum DataType
{
    noUse,
    rX,     //x旋转
    rY,     //y旋转
    rZ,     //z旋转
    rX_,    //X旋转负向
    rY_,    //y旋转负向
    rZ_,    //z旋转负向
    pX,     //x位移
    pY,     //y位移
    pZ,     //z位移
    pX_,    //x位移负向
    pY_,    //y位移负向
    pZ_,    //z位移负向
}

[System.Serializable]
public struct float3
{
    public float x;
    public float y;
    public float z;

    public float3(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
    }

    public Vector3 GetVector3()
    {
        return new Vector3(x, y, z);
    }
    
}

[System.Serializable]
public class RobotArmArgs : ArgsBase, IArgsClass
{
    public DataType[] axises;//旋转轴
    public string[] jointsNodePath;//旋转关节对象
    public float[] InitialValue;//正常状态的初始值
    public float[] ConversionRate;//转换率
    public string[] OperationID;//操作对象
    public bool[] fixedRot;
    public float3[] savePos;
    public float3[] saveRot;

    public override ArgsBase Clone()
    {
        RobotArmArgs robotArmArgs = new RobotArmArgs
        {
            axises = this.axises,
            jointsNodePath = this.jointsNodePath,
            InitialValue = this.InitialValue,
            OperationID = new string[InitialValue.Length],
            ConversionRate = new float[InitialValue.Length],
            fixedRot = this.fixedRot,
            savePos = new float3[InitialValue.Length],
            saveRot = new float3[InitialValue.Length]
        };
        return robotArmArgs;
    }

    public void Init(object[] objs)
    {
        axises = (DataType[])objs[0];
        jointsNodePath = (string[])objs[1];
        InitialValue = (float[])objs[2];
    }

    public void Update()
    {
        if (axises == null)
        {
            if (jointsNodePath==null)
            {
                jointsNodePath = new string[axises.Length];
            }
            if (InitialValue == null)
            {
                InitialValue = new float[axises.Length];
            }
            if (ConversionRate == null)
            {
                ConversionRate = new float[axises.Length];
            }
            if (OperationID == null)
            {
                OperationID = new string[axises.Length];
            }
            if (fixedRot == null)
            {
                fixedRot = new bool[axises.Length];
            }
            if (savePos == null)
            {
                savePos = new float3[axises.Length];
            }
            if (saveRot == null)
            {
                saveRot = new float3[axises.Length];
            }
        }
    }
}
