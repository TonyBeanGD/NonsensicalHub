using UnityEngine;
using System.Collections;

public class NotificationTest : MonoBehaviour
{
    void Awake()
    {
        LocalNotification.ClearNotifications();
    }

    public void OneTime()
    {
        LocalNotification.SendNotification(1, 1, "Title", "1s一条消息", new Color32(0xff, 0x44, 0x44, 255));
    }

    public void OneTimeBigIcon()
    {
        LocalNotification.SendNotification(1, 10, "Title", "10s一条有大图标的消息", new Color32(0xff, 0x44, 0x44, 255), true, true, true, "app_icon");
    }

    public void OneTimeWithActions()
    {
        LocalNotification.Action action1 = new LocalNotification.Action("background", "In Background", this);
        action1.Foreground = false;
        LocalNotification.Action action2 = new LocalNotification.Action("foreground", "In Foreground", this);
        LocalNotification.SendNotification(1, 20, "Title", "20s消息事件", new Color32(0xff, 0x44, 0x44, 255), true, true, true, null, "boing", "default", action1, action2);
    }

    public void Repeating()
    {
        LocalNotification.SendRepeatingNotification(1, 30, 60000, "Title", "30s长消息", new Color32(0xff, 0x44, 0x44, 255));
    }

    public void Stop()
    {
        LocalNotification.CancelNotification(1);
    }

    public void OnAction(string identifier)
    {
        Debug.Log("Got action " + identifier);
    }
}
