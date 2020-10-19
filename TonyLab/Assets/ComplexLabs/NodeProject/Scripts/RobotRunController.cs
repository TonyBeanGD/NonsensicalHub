using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RobotRunController : MonoBehaviour, IUseArgsClass
{
    public bool isIncomplete = false;

    public bool isPause = false;

    public RobotArmArgs robotArmArgs;

    public Transform[] joints;

    private Vector3[] OriginPoss;
    private Vector3[] OriginRots;

    private string containerID;
    private bool isFromSimulation;
    
    public RobotArmArgs GetArgsClone()
    {
        return robotArmArgs.Clone() as RobotArmArgs;
    }

    private IEnumerator DORotate(Transform targetTsf, Vector3 targetLocalEuler, float time)
    {
        if (time <= 0)
        {
            targetTsf.localEulerAngles = targetLocalEuler;
            yield break;
        }

        float timer = 0;
        Quaternion startQuaternion = targetTsf.localRotation;
        Quaternion targetQuaternion = Quaternion.Euler(targetLocalEuler);
        while (true)
        {
            yield return null;
            timer += Time.deltaTime;

            if (timer > time)
            {
                targetTsf.localEulerAngles = targetLocalEuler;
                yield break;
            }
            else
            {
                targetTsf.localRotation = Quaternion.Lerp(startQuaternion, targetQuaternion, timer / time);
            }

        }
    }

    private IEnumerator DoMove(Transform targetTsf, Vector3 targetLocalPosition, float time)
    {
        if (time <= 0)
        {
            targetTsf.localPosition = targetLocalPosition;
            yield break;
        }

        float timer = 0;

        Vector3 startLocalPosition = targetTsf.localPosition;

        while (true)
        {
            yield return null;
            timer += Time.deltaTime;

            if (timer > time)
            {
                targetTsf.localPosition = targetLocalPosition;
                yield break;
            }
            else
            {
                targetTsf.localPosition = Vector3.Lerp(startLocalPosition, targetLocalPosition, timer / time);
            }
        }
    }

    public ArgsBase GetArgs()
    {
        return robotArmArgs;
    }

    public void Init(ArgsBase args)
    {
        robotArmArgs = args as RobotArmArgs;

        robotArmArgs.Update();

        joints = new Transform[robotArmArgs.jointsNodePath.Length];
        
        //Do Something
    }
}
