using NonsensicalFrame;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CutObject : GranulationObject
{
    [SerializeField]
    private CuttingObject cuttingObject;

    private bool checkFlag;
    private bool changeFlag;

    Thread reRender;

    Thread check;

    protected override void Awake()
    {
        base.Awake();

        gameObject.AddComponent<MeshFilter>().mesh = NonsensicalFrame.ModelHelper.GetCube(1.5f, 1.5f, 1.5f);
        gameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/white");

        checkFlag = true;
        changeFlag = false;

    }

    protected override void Update()
    {
        base.Update();

        if (cuttingObject != null)
        {
            if (Time.frameCount % 10 == 0)
            {
                if (checkFlag == true)
                {
                    checkFlag = false;

                    if (changeFlag == true)
                    {
                        changeFlag = false;

                        reRender = new Thread(ReRenderMeshThread);
                        reRender.Start();
                    }

                    CheckCutting();
                }
            }
        }
    }

    private void CheckThread(CoordinateSystem cs1, CoordinateSystem cs2, Granulation cuttingGranulation)
    {
        float step = Mathf.Pow(10, level);

        changeFlag = false;

        for (int i = 0; i < granulation.points.GetLength(0); i++)
        {
            for (int j = 0; j < granulation.points.GetLength(1); j++)
            {
                for (int k = 0; k < granulation.points.GetLength(2); k++)
                {
                    if (granulation.points[i, j, k] == true)
                    {
                        Float3 point_data = cs2.CoordinateSystemTransform(cs1, new Float3(i, j, k) * step) / step;
                        Int3 int3 = new Int3(point_data);

                        if (int3.i1 < 0 || int3.i2 < 0 || int3.i3 < 0)
                        {
                            continue;
                        }

                        if (int3.i1 >= cuttingGranulation.points.GetLength(0) || int3.i2 >= cuttingGranulation.points.GetLength(1) || int3.i3 >= cuttingGranulation.points.GetLength(2))
                        {
                            continue;
                        }

                        if (cuttingGranulation.points[int3.i1, int3.i2, int3.i3] == true)
                        {
                            changeFlag = true;
                            granulation.points[i, j, k] = false;
                        }
                    }
                }
            }
        }

        checkFlag = true;
    }

    private void CheckCutting()
    {
        Bounds thisBounds = GetComponent<MeshFilter>().mesh.bounds;
        thisBounds.center += transform.position;
        Bounds cuttingBounds = cuttingObject.GetComponent<MeshFilter>().mesh.bounds;
        cuttingBounds.center += cuttingObject.transform.position;

        if (thisBounds.Intersects(cuttingBounds) == false)
        {
            checkFlag = true;
            return;
        }

        Granulation cuttingGranulation = cuttingObject.granulation;
        if (cuttingGranulation.level != granulation.level)
        {
            checkFlag = true;
            return;
        }

        CoordinateSystem cs1 = new CoordinateSystem(transform.position + granulation.origin, transform.right, transform.up, transform.forward);
        CoordinateSystem cs2 = new CoordinateSystem(cuttingObject.transform.position + cuttingGranulation.origin, cuttingObject.transform.right, cuttingObject.transform.up, cuttingObject.transform.forward);


        check = new Thread(() => CheckThread(cs1, cs2, cuttingGranulation));
        check.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        reRender?.Abort();
        check?.Abort();
    }
}
