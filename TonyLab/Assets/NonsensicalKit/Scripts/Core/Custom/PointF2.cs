using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NonsensicalKit.Custom
{
    public class PointF2
    {

        public float x;
        public float y;
        public PointF2(float _f1, float _f2)
        {
            x = _f1;
            y = _f2;
        }

        public PointF2(Vector2 _vector2)
        {
            x = _vector2.x;
            y = _vector2.y;
        }

    }
}