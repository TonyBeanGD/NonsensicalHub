using NonsensicalKit;
using NonsensicalKit.Custom;
using NonsensicalKit.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctreeCuttingLab : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform target2;

    private void Update()
    {
        Matrix4x4 m4 = transform.worldToLocalMatrix * target.localToWorldMatrix;
        NonsensicalDebugger.Log(m4.inverse.TransformWithPos(Vector3.zero), transform.position);
    }

    public void test()
    {
        Vector3 afuck = target.localToWorldMatrix * new Vector4(0.5f, 0.5f, 0.5f, 1);
        Vector3 bfuck = transform.worldToLocalMatrix * afuck;
        Vector3 cfuck = transform.localToWorldMatrix * bfuck;
        target2.position = cfuck;
    }

    public void test2()
    {
        Matrix4x4 tran = transform.worldToLocalMatrix;
        Vector3 localPos = target.position - transform.position;
        Vector3 newPos = tran * new Vector4(localPos.x, localPos.y, localPos.z, 0);
        NonsensicalDebugger.Log(newPos);
        NonsensicalDebugger.Log(newPos.x > 0.5f || newPos.x < -0.5f || newPos.y > 0.5f || newPos.y < -0.5f || newPos.z > 0.5f || newPos.z < -0.5f);
    }




}


public class OctreeNode
{
    private const float radical3 = 1.73205f;

    /// <summary>
    /// 大小，边长
    /// </summary>
    public float Size;
    /// <summary>
    /// 中心点
    /// </summary>
    public Vector3 Center;
    /// <summary>
    /// 父节点
    /// </summary>
    public OctreeNode Parent;
    /// <summary>
    /// 子节点数组，如果没有子节点应当为空
    /// </summary>
    public OctreeNode[] Childs;
    /// <summary>
    /// 0：未知，1：白，全无，2：灰，部分，3：黑，全有
    /// </summary>
    public uint State;

    public OctreeNode(OctreeNode parent, Vector3 center, float size)
    {
        Parent = parent;
        Center = center;
        Size = size;
    }

    /// <summary>
    /// 碰撞检测，需要传入对方本地转世界矩阵和自己世界转本地矩阵的积（右乘）
    /// </summary>
    /// <param name="otherNode"></param>
    /// <param name="tran"></param>
    /// <returns></returns>
    private bool CollisionDetection(OctreeNode otherNode, Matrix4x4 tran, Matrix4x4 inverse)
    {
        float halfsumsize = 0.5f * (Size + otherNode.Size);
        float r3sumsize = radical3 * halfsumsize;
        Vector3 otherCenter = tran.TransformWithPos(otherNode.Center);

        float distance = Vector3.Distance(Center, otherCenter);
        if (halfsumsize > distance)
        {
            return true;
        }
        else if (r3sumsize < distance)
        {
            return false;
        }

        for (int i = 0; i < 8; i++)
        {
            if (Size >= otherNode.Size)
            {
                float halfSize = 0.5f * Size;
                Vector3 otherPoint = tran.TransformWithPos(otherNode.Center + VectorHelper.GetEightPos(i, 0.5f * otherNode.Size));
                Vector3 offsetPoint = otherPoint - Center;
                if (offsetPoint.x >= -halfSize && offsetPoint.x <= halfSize && offsetPoint.y >= -halfSize && offsetPoint.y <= halfSize && offsetPoint.z >= -halfSize && offsetPoint.z <= halfSize)
                {
                    return true;
                }
            }
            else
            {
                float halfSize = 0.5f * otherNode.Size;
                Vector3 otherPoint = inverse.TransformWithPos(Center + VectorHelper.GetEightPos(i, 0.5f * Size));
                Vector3 offsetPoint = otherPoint - otherNode.Center;
                if (offsetPoint.x >= -halfSize && offsetPoint.x <= halfSize && offsetPoint.y >= -halfSize && offsetPoint.y <= halfSize && offsetPoint.z >= -halfSize && offsetPoint.z <= halfSize)
                {
                    return true;
                }
            }

        }
        return false;
    }

    /// <summary>
    /// 对节点进行碰撞判断，碰到的情况下
    /// 只对自己为非白且对方为非白的节点进行判断
    /// 对方为黑则八分自己，分别与之进行判断，如果是最低级节点则进行消除
    /// 对方为灰则八分对方，与之分别进行判断
    /// </summary>
    /// <param name="otherNode"></param>
    /// <param name="tran"></param>
    public bool CuttingBy(OctreeNode otherNode, Matrix4x4 tran, bool justCheck = false)
    {
        Matrix4x4 inverse = tran.inverse;

        Stack<OctreeNode> selfNodes = new Stack<OctreeNode>();
        Stack<OctreeNode> otherNodes = new Stack<OctreeNode>();

        selfNodes.Push(this);
        otherNodes.Push(otherNode);

        bool changed = false;

        while (selfNodes.Count > 0)
        {
            OctreeNode crtSelfNode = selfNodes.Pop();
            OctreeNode crtOtherNode = otherNodes.Pop();

            if (crtSelfNode.State != 1 && crtOtherNode.State != 1)
            {
                if (crtSelfNode.CollisionDetection(crtOtherNode, tran, inverse))
                {
                    if (crtOtherNode.State == 2)
                    {
                        foreach (var item in crtOtherNode.Childs)
                        {
                            selfNodes.Push(crtSelfNode);
                            otherNodes.Push(item);
                        }
                    }
                    if (crtOtherNode.State == 3)
                    {
                        if (crtSelfNode.Childs != null)
                        {
                            foreach (var item in crtSelfNode.Childs)
                            {
                                selfNodes.Push(item);
                                otherNodes.Push(crtOtherNode);
                            }
                        }
                        else
                        {
                            if (!justCheck)
                            {
                                changed = true;
                                crtSelfNode.State = 1;

                                OctreeNode parent = crtSelfNode.Parent;
                                while (parent != null)
                                {
                                    parent.State = 0;
                                    parent = parent.Parent;
                                }
                            }

                        }
                    }
                }
            }
        }

        if (changed)
        {
            UpdateState();
        }

        return changed;
    }

    /// <summary>
    /// 更新状态信息
    /// </summary>
    private void UpdateState()
    {
        Stack<OctreeNode> octreeNodes = new Stack<OctreeNode>();
        Stack<bool> check = new Stack<bool>();

        octreeNodes.Push(this);
        check.Push(false);

        while (octreeNodes.Count > 0)
        {
            OctreeNode crtNode = octreeNodes.Pop();
            bool crtCheck = check.Pop();

            if (crtNode.State == 0)
            {
                if (crtCheck)
                {
                    //为true时一定有子节点
                    bool white = false;
                    bool gray = false;
                    bool black = false;
                    foreach (var item in crtNode.Childs)
                    {
                        if (item.State == 2)
                        {
                            gray = true;
                            break;
                        }
                        white |= item.State == 1;
                        black |= item.State == 3;
                    }
                    if (gray || (white && black))
                    {
                        crtNode.State = 2;
                    }
                    else if (!black)
                    {
                        crtNode.State = 1;
                    }
                    else
                    {
                        crtNode.State = 3;
                    }
                }
                else
                {
                    if (crtNode.Childs != null)
                    {
                        octreeNodes.Push(crtNode);
                        check.Push(true);
                        foreach (var item in crtNode.Childs)
                        {
                            octreeNodes.Push(item);
                            check.Push(false);
                        }
                    }
                }
            }



        }
    }


}

public class OctreeModel
{
    public OctreeNode _root;    //八叉树根节点

    private float _size;
    private MeshBuffer _meshBuffer;

    public OctreeModel(Transform transform, Mesh mesh)
    {
        Vector3 _meshsize = Vector3.Scale(mesh.bounds.size, transform.lossyScale);

        float max = Mathf.Max(_meshsize.x, _meshsize.y);
        _size = Mathf.Max(max, _meshsize.z);

        transform.SetLossyScaleOne();
        _meshBuffer = new MeshBuffer(mesh);
        _meshBuffer.Scale(transform.lossyScale);
    }

    public void InitNode(int depth)
    {
        _root = new OctreeNode(null, Vector3.zero, _size);

        Stack<OctreeNode> octreeNodes = new Stack<OctreeNode>();
        Stack<int> depths = new Stack<int>();

        octreeNodes.Push(_root);
        depths.Push(depth);

        while (octreeNodes.Count > 0)
        {
            OctreeNode crtNode = octreeNodes.Pop();
            int crtDepth = depths.Pop();

            if (crtDepth > 0)
            {
                octreeNodes.Push(crtNode);
                depths.Push(-1);


                float childSize = crtNode.Size * 0.5f;
                float childCenterOffset = crtNode.Size * 0.25f;

                crtNode.Childs = new OctreeNode[8];
                for (int i = 0; i < 8; i++)
                {
                    crtNode.Childs[i] = new OctreeNode(crtNode, crtNode.Center + VectorHelper.GetEightPos(i, childCenterOffset), childSize);
                    octreeNodes.Push(crtNode.Childs[i]);
                    depths.Push(crtDepth - 1);
                }
            }
            else if (crtDepth == 0)
            {
                if (_meshBuffer.Contain(crtNode.Center))
                {
                    crtNode.State = 3;
                }
                else
                {
                    crtNode.State = 1;
                }
            }
            else
            {
                bool white = false;
                bool gray = false;
                bool black = false;
                foreach (var item in crtNode.Childs)
                {
                    if (item.State == 2)
                    {
                        gray = true;
                        break;
                    }
                    white |= item.State == 1;
                    black |= item.State == 3;
                }
                if (gray || (white && black))
                {
                    crtNode.State = 2;
                }
                else if (!black)
                {
                    crtNode.State = 1;
                }
                else
                {
                    crtNode.State = 3;
                }
            }
        }
    }

    public MeshBuffer DrawMesh()
    {
        MeshBuffer meshBuffer = new MeshBuffer();
        Queue<OctreeNode> octreeNodes = new Queue<OctreeNode>();

        octreeNodes.Enqueue(_root);


        while (octreeNodes.Count > 0)
        {
            OctreeNode crtNode = octreeNodes.Dequeue();

            if (crtNode.State == 3)
            {
                meshBuffer.AddCube(crtNode.Center, Vector3.one * crtNode.Size);
            }
            else if (crtNode.State == 2)
            {
                foreach (var item in crtNode.Childs)
                {
                    octreeNodes.Enqueue(item);
                }
            }
        }

        return meshBuffer;
    }
}
public class OctreeModelKnife : OctreeModel
{
    public Matrix4x4 LocalToWorldMatrix { get; private set; }

    public OctreeModelKnife(Transform knife, Mesh mesh) : base(knife, mesh)
    {
        LocalToWorldMatrix = knife.localToWorldMatrix;
    }
    public void UpdateState(Transform knife)
    {
        LocalToWorldMatrix = knife.localToWorldMatrix;
    }
}

public class OctreeModelPiece : OctreeModel
{
    private Matrix4x4 worldToLocalMatrix;

    public OctreeModelPiece(Transform piece, Mesh mesh) : base(piece, mesh)
    {
        worldToLocalMatrix = piece.worldToLocalMatrix;
    }

    public void UpdateState(Transform piece)
    {
        worldToLocalMatrix = piece.worldToLocalMatrix;
    }

    public bool CuttingBy(OctreeModelKnife knife, bool justCheck = false)
    {
        OctreeNode selfNode = base._root;
        OctreeNode knifeNode = knife._root;

        Matrix4x4 tran = worldToLocalMatrix * knife.LocalToWorldMatrix;

        return selfNode.CuttingBy(knifeNode, tran, justCheck);
    }
}
