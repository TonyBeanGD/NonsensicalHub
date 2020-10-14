using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpToDie : MonoBehaviour
{
    private RectTransform rect;
    private float height;
    private float timer;

    void OnEnable()
    {
        timer = 0;
        rect = transform.GetComponent<RectTransform>();
        height = Screen.height * 0.15f;
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, rect.sizeDelta.y);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < 0.5f)
        {
            rect.anchoredPosition = new Vector2(0, timer * 2 * height);
        }
        if (timer > 2.5f)
        {
            gameObject.SetActive(false);
        }
    }
}
