using NonsensicalKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingObject : GranulationObject
{
    [SerializeField]
    private CutObject cutObject;

    protected override void Awake()
    {
        base.Awake();

        gameObject.AddComponent<MeshFilter>().mesh = NonsensicalKit.ModelHelper.GetCube(0.5f, 1f, 0.5f);
        gameObject.AddComponent<MeshRenderer>().material=Resources.Load<Material>("Materials/white");
    }

    protected override void Update()
    {
        base.Update();

        if (cutObject != null)
        {
            if (Time.frameCount % 1 == 0)
            {
                CheckCutting();
            }
        }
    }

    private void DoCheck(CoordinateSystem cs1, CoordinateSystem cs2, Granulation cutGranulation)
    {
        float step = Mathf.Pow(10, level);

        CoordinateSystemDiff csd = new CoordinateSystemDiff(cs1, cs2, step);

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

        if (changeFlag==true)
        {
            cutObject.needRefresh = true;
        }
    }

    private void CheckCutting()
    {
        Bounds thisBounds = GetComponent<MeshFilter>().mesh.bounds;
        thisBounds.center += transform.position;
        Bounds cuttingBounds = cutObject.GetComponent<MeshFilter>().mesh.bounds;
        cuttingBounds.center += cutObject.transform.position;

        if (thisBounds.Intersects(cuttingBounds) == false)
        {
            return;
        }

        Granulation cutGranulation = cutObject.granulation;
        if (cutGranulation.level != granulation.level)
        {
            return;
        }

        CoordinateSystem cs1 = new CoordinateSystem(transform.position + granulation.origin, transform.right, transform.up, transform.forward);
        CoordinateSystem cs2 = new CoordinateSystem(cutObject.transform.position + cutGranulation.origin, cutObject.transform.right, cutObject.transform.up, cutObject.transform.forward);

        DoCheck(cs1, cs2, cutGranulation);
    }
}
