using NonsensicalKit;
using NonsensicalKit.Custom;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class CuttingObject : GranulationObject
{
    [SerializeField] protected CutObject cutObject;

    public delegate void CheckOverHandle();
    protected event CheckOverHandle OnCheckOver;

    protected bool CheckCutting()
    {
        //TODO：计算边界，减少远离时的计算量
        Granulation cutGranulation = cutObject.granulation;
        if (cutGranulation.Level != granulation.Level)
        {
            Debug.LogWarning("切削物体和被切削物体划分等级不一致");
            return false;
        }

        CoordinateSystem cs1 = new CoordinateSystem(granulation.point + granulation.originOffset, granulation._right, granulation._up, granulation._forward);
        CoordinateSystem cs2 = new CoordinateSystem(cutGranulation.point + cutGranulation.originOffset, cutGranulation._right, cutGranulation._up, cutGranulation._forward);

        CoordinateSystemDiff csd = new CoordinateSystemDiff(cs1, cs2, granulation.step);

        bool changeFlag = false;

        for (int i = 0; i < granulation.length0; i++)
        {
            for (int j = 0; j < granulation.length1; j++)
            {
                for (int k = 0; k < granulation.length2; k++)
                {
                    if (granulation.points[i, j, k] == true)
                    {
                        Float3 point_data = csd.GetCoordinate(new Float3(i, j, k));
                        Int3 int3 = new Int3(point_data);

                        if (int3.i1 < 0 || int3.i2 < 0 || int3.i3 < 0
                            || int3.i1 >= cutGranulation.length0
                            || int3.i2 >= cutGranulation.length1
                            || int3.i3 >= cutGranulation.length2)
                        {
                            continue;
                        }

                        if (cutGranulation.points[int3.i1, int3.i2, int3.i3] == true)
                        {
                            changeFlag = true;
                            cutGranulation.points[int3.i1, int3.i2, int3.i3] = false;
                        }
                    }
                }
            }
        }

        OnCheckOver?.Invoke();
        if (changeFlag == true)
        {
            Debug.Log("change");
            return true;
        }
        else
        {
            return false;
        }

    }
}
