using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonsensicalKit.Utility
{
    public static class ModelHelper
    {
        public static Mesh GetCube(Vector3 size)
        {
            return GetCube(size.x, size.y, size.z);
        }
        public static Mesh GetCube(Vector3 offset, Vector3 size)
        {
            return GetCube(offset.x, offset.y, offset.z, size.x, size.y, size.z);
        }
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

        public static Mesh GetCube(float offsetX, float offsetY, float offsetZ, float width, float height, float depth)
        {
            MeshBuffer meshbuffer = new MeshBuffer();
            Vector3 offset = new Vector3(offsetX, offsetY, offsetZ);
            Vector3[] point = new Vector3[8];
            point[0] = offset + new Vector3(-width * 0.5f, -height * 0.5f, -depth * 0.5f);
            point[1] = offset + new Vector3(-width * 0.5f, height * 0.5f, -depth * 0.5f);
            point[2] = offset + new Vector3(width * 0.5f, height * 0.5f, -depth * 0.5f);
            point[3] = offset + new Vector3(width * 0.5f, -height * 0.5f, -depth * 0.5f);
            point[4] = offset + new Vector3(-width * 0.5f, -height * 0.5f, depth * 0.5f);
            point[5] = offset + new Vector3(-width * 0.5f, height * 0.5f, depth * 0.5f);
            point[6] = offset + new Vector3(width * 0.5f, height * 0.5f, depth * 0.5f);
            point[7] = offset + new Vector3(width * 0.5f, -height * 0.5f, depth * 0.5f);

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


        public static Mesh GetCylinder(float radius, float height)
        {
            MeshBuffer meshbuffer = new MeshBuffer();

            meshbuffer.AddRing(Vector3.zero, 0, radius, Vector3.up, 32);
            meshbuffer.AddRing3D(Vector3.zero, radius, new Vector3(0, height, 0), radius, Vector3.up, 32);
            meshbuffer.AddRing(new Vector3(0, height, 0), 0, radius, -Vector3.up, 32);

            return meshbuffer.GetMesh();
        }
    }

}

