using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoHide : MonoBehaviour
{
    Image image;
    void Awake()
    {
        image = transform.GetComponent<Image>();
    }

	void Update()
	{
        if (Time.frameCount%3==0)
        {
            float screenPosX = CanvasManager._Instance._MainCamera.WorldToScreenPoint(transform.position).x;

            if (screenPosX<-300||screenPosX> CanvasManager._Instance.ScreenWidth+300)
            {
                image.enabled = false;
            }
            else
            {
                image.enabled = true;
            }
        }
	}
}