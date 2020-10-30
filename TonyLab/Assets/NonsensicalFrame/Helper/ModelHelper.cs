using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonsensicalFrame
{
    public static class ModelHelper
    {
        public static Mesh GetCube(float width, float height, float depth)
        {
            MeshBuffer meshbuffer = new MeshBuffer();

            Vector3[] point = new Vector3[8];
            point[0] = new Vector3(-width * 0.5f, -height * 0.5f, -depth * 0.5f);
            point[1] = new Vector3(-width * 0.5f, height * 0.5f, -depth * 0.5f);
            point[2] = new Vector3(width * 0.5f, height * 0.5f, -depth * 0.5f);
            point[3] = new Vector3(width * 0.5f, -height * 0.5f, -depth * 0.5f);
            point[4] = new Vector3(-width * 0.5f, -height * 0.5f, depth * 0.5f);
            point[5] = new Vector3(-width * 0.5f, height * 0.5f, depth * 0.5f);
            point[6] = new Vector3(width * 0.5f, height * 0.5f, depth * 0.5f);
            point[7] = new Vector3(width * 0.5f, -height * 0.5f, depth * 0.5f);

            //init
            Vector2 middle = Vector2.one * 0.5f;

            //front
            meshbuffer.AddQuad(new Vector3[] { point[0], point[1], point[2], point[3] }, new Vector3(0, 0, -1), middle);

            //back
            meshbuffer.AddQuad(new Vector3[] { point[7], point[6], point[5], point[4] }, new Vector3(0, 0, 1), middle);

            //left
            meshbuffer.AddQuad(new Vector3[] { point[4], point[5], point[1], point[0] }, new Vector3(-1, 0, 0), middle);

            //right
            meshbuffer.AddQuad(new Vector3[] { point[3], point[2], point[6], point[7] }, new Vector3(1, 0, 0), middle);

            //down
            meshbuffer.AddQuad(new Vector3[] { point[0], point[3], point[7], point[4] }, new Vector3(0, -1, 0), middle);

            //up
            meshbuffer.AddQuad(new Vector3[] { point[1], point[5], point[6], point[2] }, new Vector3(0, 1, 0), middle);
           

            return meshbuffer.GetMesh();
        }
    }

}

