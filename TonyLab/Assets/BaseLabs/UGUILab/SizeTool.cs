using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeTool : MonoBehaviour {
   [SerializeField] private RectTransform target;

    private RectTransform recttransform;

    private void Start()
    {
        recttransform = GetComponent<RectTransform>();
    }

    private void Update () {
        Vector2 size = target.sizeDelta;
        size.x = recttransform.sizeDelta.x; ;
        recttransform.sizeDelta = size;

    }
}
