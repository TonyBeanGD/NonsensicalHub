using NonsensicalKit.Custom;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeInfoArgs
{
    public string topName;              //顶节点名称(显示名称)
    public string modelName;            //模型名称（加载路径）
    public string changeModelName;      //可以改变的模型名称(显示名称)

    public string topID;                //当前节点最外层对象ID
    public string mountNodeID;          //父节点ID
    public string selfID;               //当前节点ID

    public string nodePath;             //节点路径（原模型中此节点路径）
    public string rawModelName;         //从属模型名（原模型顶节点模型名）
    public string rawNodeName;          //原节点名（原模型此节点模型名）

    public bool needInstantiate;        //是否需要加载（加载场景时使用）
    public List<string> rawChild;       //原模型中子节点名称

    public bool isHidden;               //模型是否隐藏
    public PointF3 nodePos;           //本地坐标
    public PointF3 nodeRot;            //旋转欧拉角

    public List<NodeInfoArgs> childNodes;

    public NodeInfoArgs Clone()
    {
     
        NodeInfoArgs temp = new NodeInfoArgs
        {
            topName = this.topName,
            modelName = this.modelName,
            changeModelName = this.changeModelName,
            
            topID = this.topID,
            mountNodeID = this.mountNodeID,
            selfID = this.selfID,
            
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

