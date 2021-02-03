using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 自适应大小工具类，当大小大于一定数值时才开启大小自适应
/// </summary>
[RequireComponent(typeof(ContentSizeFitter))]
[RequireComponent(typeof(RectTransform))]
public class AutoSizeController : MonoBehaviour
{

    [SerializeField] private float verticalArg=-1;
    [SerializeField] private float horizontalArg=-1;

    private ContentSizeFitter contentSizeFitter;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        contentSizeFitter = GetComponent<ContentSizeFitter>();
    }

    private void Update()
    {
        if (horizontalArg>0)
        {
            contentSizeFitter.horizontalFit = rectTransform.rect.width< horizontalArg? ContentSizeFitter.FitMode.Unconstrained: ContentSizeFitter.FitMode.PreferredSize;
            rectTransform.sizeDelta = new Vector2(horizontalArg-1, rectTransform.sizeDelta.y);
        }
        if (verticalArg>0)
        {
            contentSizeFitter.verticalFit = rectTransform.rect.height < verticalArg ? ContentSizeFitter.FitMode.Unconstrained : ContentSizeFitter.FitMode.PreferredSize;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, verticalArg-1);
        }
    }
}
