using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NonsensicalKit;

/// <summary>
/// 鼠标拖拽2d模型
/// </summary>
public class DraggingModel2D : MonoBehaviour
{
    private string ModelName;
    private BackpackItemInfo buffer;
    
    private const string _2DMask = "2DObject";
    private const string _NormalMask = "Default";

    private void Awake()
    {
        MessageAggregator<int>.Instance.Subscribe("SetDragModel2DtoBackpack", SetDragModel2DtoBackpack);
    }

    private void Update()
    {
        Camera oc = Camera.main.transform.Find("OrthographicCamera").GetComponent<Camera>();

        transform.position = oc.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 10));
        

        if (Input.GetMouseButtonDown(1))
        {
            ReturnToBackpack();
        }
    }

    public void OnDestroy()
    {
        MessageAggregator<int>.Instance.Unsubscribe("SetDragModel2DtoBackpack", SetDragModel2DtoBackpack);
    }

    private void SetDragModel2DtoBackpack(MessageArgs<int> value)
    {

        MessageAggregator<BackpackItemInfo>.Instance.Publish("SetItemToBackpack", new MessageArgs<BackpackItemInfo>(this, buffer));
        Destroy(gameObject);
    }

    public void Init(string _modelName, BackpackItemInfo _buffer)
    {
        this.ModelName = _modelName;
        this.buffer = _buffer;

        foreach (var item in transform.GetComponentsInChildren<Transform>())
        {
            item.gameObject.layer = LayerMask.NameToLayer(_2DMask);
        }
        transform.localScale *= 0.2f;

    }

    private void SetModel(Transform targetParent)
    {
        transform.SetParent(targetParent);
        foreach (var item in transform.GetComponentsInChildren<Transform>())
        {
            item.gameObject.layer = LayerMask.NameToLayer(_NormalMask);
        }

        transform.localScale *= 5f;

    }

    private void ReturnToBackpack()
    {
        MessageAggregator<BackpackItemInfo>.Instance.Publish("SetItemToBackpack", new MessageArgs<BackpackItemInfo>(this, buffer));

        Destroy(gameObject);
    }
}
