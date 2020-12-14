using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using NonsensicalKit;

namespace mattatz.MeshSmoothingSystem
{

    public class VertexConnection
    {

        public HashSet<int> Connection { get; }

        public HashSet<int> Points;

        public VertexConnection()
        {
            this.Connection = new HashSet<int>();
            this.Points = new HashSet<int>();
        }

        public void Connect(int to)
        {
            Connection.Add(to);
        }

        public static Dictionary<Vector3, VertexConnection> BuildNetwork(MeshBuffer meshbuffer)
        {
            var table = new Dictionary<Vector3, VertexConnection>();

            for (int i = 0, n = meshbuffer.triangles.Count; i < n; i += 3)
            {
                Vector3 a = meshbuffer.GetVerticeByTrianglesIndex(i), b = meshbuffer.GetVerticeByTrianglesIndex(i + 1), c = meshbuffer.GetVerticeByTrianglesIndex(i + 2);
                if (!table.ContainsKey(a))
                {
                    table.Add(a, new VertexConnection());
                }
                table[a].Points.Add(meshbuffer.triangles[i + 0]);
                if (!table.ContainsKey(b))
                {
                    table.Add(b, new VertexConnection());
                }
                table[b].Points.Add(meshbuffer.triangles[i + 1]);
                if (!table.ContainsKey(c))
                {
                    table.Add(c, new VertexConnection());
                }
                table[c].Points.Add(meshbuffer.triangles[i + 2]);


                table[a].Connect(meshbuffer.triangles[i + 1]); table[a].Connect(meshbuffer.triangles[i + 2]);
                table[b].Connect(meshbuffer.triangles[i + 0]); table[b].Connect(meshbuffer.triangles[i + 2]);
                table[c].Connect(meshbuffer.triangles[i + 0]); table[c].Connect(meshbuffer.triangles[i + 1]);
            }

            return table;
        }

    }

}

