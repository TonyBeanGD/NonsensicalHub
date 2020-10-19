using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCubeController : MonoBehaviour
{
    [SerializeField]
    private Camera targetCamera;
    [SerializeField]
    private RectTransform pos;
    [SerializeField]
    private RectTransform canvas;

    private Transform cubeParentPoint;

    private void Awake()
    {
        cubeParentPoint = transform;
    }

    void LateUpdate()
    {
        AutoPosition();
        AutoSize();
    }

    private void AutoPosition()
    {
        float x = (pos.localPosition.x + canvas.rect.width * 0.5f) / canvas.rect.width * Screen.width;
        float y = (pos.localPosition.y + canvas.rect.height * 0.5f) / canvas.rect.height * Screen.height;

        Vector3 screenPoint = new Vector3(x, y, 10);

        Vector3 worldPoint = targetCamera.ScreenToWorldPoint(screenPoint);

        cubeParentPoint.position = worldPoint;
    }

    private void AutoSize()
    {
        int width = Screen.width;
        int height = Screen.height;
        float realSize = (float)width / height;
        float settingSize = (float)1920 / 1080;

        cubeParentPoint.localScale = Vector3.one * 1.2f * realSize / settingSize;
    }
}
