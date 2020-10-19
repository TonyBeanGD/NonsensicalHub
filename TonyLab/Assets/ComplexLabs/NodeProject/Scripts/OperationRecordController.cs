using System.Collections.Generic;
using UnityEngine;

public class OperationRecordController : MonoBehaviour
{
    private Stack<OperationRecord> operationRecords = new Stack<OperationRecord>();    //操作记录栈
    private Stack<OperationRecord> revokeRecords = new Stack<OperationRecord>();     //撤销记录栈

    /// <summary>
    /// 记录操作
    /// </summary>
    private void RecordOperater(OperationRecord noti)
    {
        OperationRecord temp = noti;
        operationRecords.Push(temp);
        revokeRecords.Clear();
    }

    /// <summary>
    /// 撤销操作
    /// </summary>
    private void RevokeOperater()
    {
        if (operationRecords.Count == 0)
        {
            Debug.Log("无法进行撤销操作，因为操作记录为空");
            return;
        }

        OperationRecord operateRecord = operationRecords.Pop();

        OperationRecord revokeRecord = operateRecord.Clone();
        revokeRecords.Push(revokeRecord);

        switch (operateRecord.type)
        {
            case OperateType.Move:
                {
                    List<Transform> transforms = operateRecord.transforms;
                    for (int i = 0; i < transforms.Count; i++)
                    {
                        transforms[i].position = operateRecord.positions[i];
                    }
                }
                break;
            case OperateType.Rotate:
                {
                    List<Transform> transforms = operateRecord.transforms;
                    for (int i = 0; i < transforms.Count; i++)
                    {
                        transforms[i].eulerAngles = operateRecord.rotations[i];
                    }
                }
                break;
            case OperateType.Select:
                {

                }
                break;
            case OperateType.CopyPaste:
                {


                }
                break;
            case OperateType.ShearPaste:
                {

                }
                break;
            case OperateType.Delete:
                {

                }
                break;
            case OperateType.Import:
                {

                }
                break;
            case OperateType.Alignment:
                {

                }
                break;
            default:
                {
                    Debug.LogWarning("未知操作类型");
                }
                break;
        }

    }

    /// <summary>
    /// 恢复操作
    /// </summary>
    private void RecoveryOperater()
    {
        if (revokeRecords.Count == 0)
        {
            Debug.Log("无法进行恢复操作，因为撤销记录为空");
            return;
        }

        OperationRecord revokeRecord = revokeRecords.Pop();

        OperationRecord operateRecord = revokeRecord.Clone();
        operationRecords.Push(operateRecord);

        switch (revokeRecord.type)
        {
            case OperateType.Move:
                {
                    List<Transform> transforms = revokeRecord.transforms;
                    for (int i = 0; i < transforms.Count; i++)
                    {
                        transforms[i].position = revokeRecord.positions[i];
                    }
                }
                break;
            case OperateType.Rotate:
                {
                    List<Transform> transforms = revokeRecord.transforms;
                    for (int i = 0; i < transforms.Count; i++)
                    {
                        transforms[i].eulerAngles = revokeRecord.rotations[i];
                    }
                }
                break;
            case OperateType.Select:
                {

                }
                break;
            case OperateType.CopyPaste:
                {

                }
                break;
            case OperateType.ShearPaste:
                {

                }
                break;
            case OperateType.Delete:
                {

                }

                break;
            case OperateType.Import:
                {

                }

                break;
            case OperateType.Alignment:
                {

                }
                break;
            default:
                {
                    Debug.LogWarning("未知操作类型");
                }
                break;
        }
    }
}

/// <summary>
/// 操作类型
/// </summary>
public enum OperateType
{
    Move,       //移动
    Rotate,     //旋转
    Select,     //选择
    CopyPaste,  //复制粘贴
    ShearPaste, //剪切粘贴
    Delete,     //删除
    Import,     //导入
    Alignment,  //对齐
}

/// <summary>
/// 操作记录类
/// </summary>
public class OperationRecord
{
    public OperateType type;

    public List<Transform> parentTsf;
    public List<Transform> transforms;
    public List<Vector3> positions;
    public List<Vector3> rotations;
    public List<bool> activeSelfs;

    /// <summary>
    /// 传入操作对象链表和操作类型的构造函数，选择操作不能使用这个构造函数
    /// </summary>
    /// <param name="transforms"></param>
    /// <param name="type"></param>
    public OperationRecord(List<Transform> transforms, OperateType type)
    {
        this.transforms = new List<Transform>(transforms.ToArray());
        this.type = type;
        switch (type)
        {
            case OperateType.Move:
                {
                    this.positions = new List<Vector3>();
                    for (int i = 0; i < transforms.Count; i++)
                    {
                        this.positions.Add(transforms[i].position);
                    }
                }
                break;
            case OperateType.Rotate:
                {
                    this.rotations = new List<Vector3>();
                    for (int i = 0; i < transforms.Count; i++)
                    {
                        this.rotations.Add(transforms[i].eulerAngles);
                    }
                }
                break;
            case OperateType.Select:
                {
                    Debug.LogWarning("使用了错误的构造函数");
                }
                break;
            case OperateType.CopyPaste:
                {

                }
                break;
            case OperateType.ShearPaste:
                {
                    parentTsf = new List<Transform>();
                    foreach (var item in transforms)
                    {
                        parentTsf.Add(item.parent);
                    }
                }
                break;
            case OperateType.Delete:
                {
                    this.activeSelfs = new List<bool>();
                    for (int i = 0; i < transforms.Count; i++)
                    {
                        this.activeSelfs.Add(transforms[i].gameObject.activeSelf);
                    }
                }
                break;
            case OperateType.Import:
                {

                }
                break;
            case OperateType.Alignment:
                {
                    this.positions = new List<Vector3>();
                    this.rotations = new List<Vector3>();
                    for (int i = 0; i < transforms.Count; i++)
                    {
                        this.positions.Add(transforms[i].position);
                        this.rotations.Add(transforms[i].eulerAngles);
                    }
                }
                break;
            default:
                Debug.Log("未判断的枚举" + type.ToString());
                break;
        }
    }

    /// <summary>
    /// 获取这个操作记录的克隆
    /// </summary>
    /// <returns></returns>
    public OperationRecord Clone()
    {
        switch (type)
        {
            case OperateType.Move:
            case OperateType.Rotate:
            case OperateType.CopyPaste:
            case OperateType.ShearPaste:
            case OperateType.Delete:
            case OperateType.Import:
            case OperateType.Alignment:
            case OperateType.Select:
                return new OperationRecord(this.transforms, this.type);
            default:
                Debug.LogWarning("未作判断的操作类型");
                return null;
        }
    }
}