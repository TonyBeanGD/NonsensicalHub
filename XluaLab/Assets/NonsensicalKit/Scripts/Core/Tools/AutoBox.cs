using NonsensicalKit.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonsensicalKit
{
    public class AutoBox : MonoBehaviour
    {

        [SerializeField] private Vector3 autoBoxSize;

        private void Awake()
        {
            gameObject.AddComponent<MeshFilter>().mesh = ModelHelper.GetCube(autoBoxSize.x, autoBoxSize.y, autoBoxSize.z);
            gameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/white");

        }

    }


}
