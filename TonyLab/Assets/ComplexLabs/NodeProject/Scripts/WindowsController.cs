using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;


public struct RECT
{
    public int Left;        //最左坐标
    public int Top;         //最上坐标
    public int Right;       //最右坐标
    public int Down;        //最下坐标
}

public struct Point
{
    public int x;
    public int y;
}

public class WindowsController : MonoBehaviour
{
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out Point pt);

    [DllImport("user32.dll")]
    public static extern int SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);

    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private extern static bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private extern static IntPtr SetFocus(IntPtr hWnd);

    RECT screenRect = new RECT(); //屏幕矩形位置

    //设置边缘时的鼠标位置
    private void SetSursor()
    {
        IntPtr hWnd = GetForegroundWindow();    //获取当前窗口句柄
        GetWindowRect(hWnd, ref screenRect);
        Point p;
        GetCursorPos(out p);
        if (p.x <= screenRect.Left + 9)
        {
            SetCursorPos(screenRect.Right - 10, p.y);
        }
        if (p.x >= screenRect.Right - 9)
        {
            SetCursorPos(screenRect.Left + 10, p.y);
        }
        if (p.y <= screenRect.Top + 9)
        {
            SetCursorPos(p.x, screenRect.Down - 10);
        }
        if (p.y >= screenRect.Down - 9)
        {
            SetCursorPos(p.x, screenRect.Top + 10);
        }
    }

    //设置窗口
    private void SetWindows(Tuple<int, int> noti)
    {
        Tuple<int, int> temp = noti;
        int width = temp.Item1;
        int height = temp.Item2;

        Debug.Log($"设置窗口大小宽度：{width},高度：{height}");
        Screen.SetResolution(width, height, false);
        Debug.Log($"设置分辨率,设置完后宽度：{Screen.width},高度：{Screen.height}");

        IntPtr hWnd = GetForegroundWindow();    //获取当前窗口句柄
        GetWindowRect(hWnd, ref screenRect);
        Debug.Log($"通过句柄获取大小左：{screenRect.Left},右：{screenRect.Right},上：{screenRect.Top},下：{screenRect.Down}");
    }
}
