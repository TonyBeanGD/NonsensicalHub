using NonsensicalKit.Utility;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.WSA.WebCam;

public class CaptureLab : MonoBehaviour
{
    void Start()
    {
        ScreenCapture.CaptureScreenshot(Path.Combine(Application.streamingAssetsPath, "Game.png"));
    }
}