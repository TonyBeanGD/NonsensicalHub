using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 与目标ui保持相同Height
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SameHeight : MonoBehaviour
{
    [SerializeField] private RectTransform target;
    [SerializeField] private float min;

    private RectTransform self;

    private void Awake()
    {
        self = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (target.sizeDelta.y>min)
        {
            self.sizeDelta = new Vector2(self.sizeDelta.x, target.sizeDelta.y);
        }
    }
}
