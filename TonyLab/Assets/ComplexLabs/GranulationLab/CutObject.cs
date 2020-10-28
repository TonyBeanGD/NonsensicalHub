using NonsensicalFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutObject : GranulationObject
{
    [SerializeField]
    private CuttingObject cuttingObject;

    private void Update()
    {
        if (Time.frameCount % 10 == 0)
        {
            CheckCutting();
        }
    }

    private void CheckCutting()
    {
        Bounds thisBounds = GetComponent<MeshFilter>().mesh.bounds;
        Bounds cuttingBounds = cuttingObject.GetComponent<MeshFilter>().mesh.bounds;

        if (thisBounds.Intersects(cuttingBounds)==false)
        {
            return;
        }

        Granulation cuttingGranulation = cuttingObject.granulation;
        if (cuttingGranulation.level != granulation.level)
        {
            return;
        }

        Vector3 posOffset =( transform.position+ granulation.origin) - (cuttingObject.transform.position + cuttingGranulation.origin);

        int xOffset = Mathf.RoundToInt(NumHelper.GetNearValue(posOffset.x, granulation.level) / Mathf.Pow(10, granulation.level));
        int yOffset = Mathf.RoundToInt(NumHelper.GetNearValue(posOffset.y, granulation.level) / Mathf.Pow(10, granulation.level));
        int zOffset = Mathf.RoundToInt(NumHelper.GetNearValue(posOffset.z, granulation.level) / Mathf.Pow(10, granulation.level));
        
        bool changeFlag=false;

        for (int i = 0; i < granulation.points.GetLength(0); i++)
        {
            for (int j = 0; j < granulation.points.GetLength(1); j++)
            {
                for (int k = 0; k < granulation.points.GetLength(2); k++)
                {
                    if (granulation.points[i, j, k] == true)
                    {
                        if (i + xOffset < 0 || j + yOffset < 0 || k + zOffset < 0)
                        {
                            continue;
                        }
                        if (i + xOffset >= cuttingGranulation.points.GetLength(0) || j + yOffset >= cuttingGranulation.points.GetLength(1) || k + zOffset >= cuttingGranulation.points.GetLength(2))
                        {
                            continue;
                        }
                        if (cuttingGranulation.points[i + xOffset, j + yOffset, k + zOffset] == true)
                        {
                            changeFlag = true;
                            granulation.points[i, j, k] = false;
                        }
                    }
                }
            }
        }

        if (changeFlag)
        {
            ReRenderMesh();
        }
    }
}
