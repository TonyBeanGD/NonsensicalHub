using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Float3
{
    public float x;
    public float y;
    public float z;
    public Float3(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public Float3(Vector3 _vector3)
    {
        x = _vector3.x;
        y = _vector3.y;
        z = _vector3.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

public class NodeInfoArgs
{
    public string ID;                   //当前模型ID

    public string nodePath;             //节点路径(节点对应其预制体的节点路径)
    public string rawModelName;         //从属模型名
    public string rawNodeName;          //原节点名
    public bool needInstantiate;        //是否需要加载(当一个节点被复制剪切出来时，就代表其在加载场景是需要单独实例化一个对应模型)
    public List<string> rawChild;       //记录原本的子物体，用于模型更新时判断节点的删除

    public bool isHidden;               //模型是否隐藏
    public Float3 nodePos;              //节点相对坐标
    public Float3 nodeRot;              //节点相对旋转

    public List<NodeInfoArgs> childNodes;   //子物体信息链表

    /// <summary>
    /// 获取副本
    /// </summary>
    /// <returns></returns>
    public NodeInfoArgs Clone()
    {
        NodeInfoArgs temp = new NodeInfoArgs
        {
            ID = this.ID,

            nodePath = this.nodePath,
            rawModelName = this.rawModelName,
            rawNodeName = this.rawNodeName,

            needInstantiate = this.needInstantiate,
            rawChild = this.rawChild,

            isHidden = this.isHidden,
            nodePos = this.nodePos,
            nodeRot = this.nodeRot
        };
        return temp;
    }
}
