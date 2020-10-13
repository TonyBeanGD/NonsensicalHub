using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class WebsocketHelper : MonoBehaviour
{
    /// <summary>
    /// 发送消息回调Action
    /// </summary>
    public Action<bool> _SendMessageCallback;

    /// <summary>
    /// 接受消息Action
    /// </summary>
    public Action<string> _ReceiveMessage;

    public static WebsocketHelper _Instance;    //单例

    private WebSocketSharp.WebSocket _ws;       //websocket实例
    private bool _UseTips = false;              //子线程开启面板用
    private bool _ReConnect = false;            //子线程重连用
    private bool _isQuitting = false;           //是否正在退出应用，用于防止在退出时尝试实例化新物体
    private bool _isReconnection = false;       //是否正在重连，用于只在重连时显示提示消息

    private Thread _HeartbeatThread;            //心跳线程

    private float _ReconnectionInterval;        //重连间隔
    private float _HeartbeatInterval;           //心跳间隔


    void Awake()
    {
        GameStatic.OnLogout += OnLogout;
        GameStatic.OnLogin += WebsocketConnect;

        _Instance = this;

        _ReconnectionInterval = 2f;
        _HeartbeatInterval = 29f;
    }

    void Update()
    {
        if (_UseTips)
        {
            TipsManager._Instance.OpenTipsText("网络连接中断");
            _UseTips = false;
        }

        if (_ReConnect)
        {
            StartCoroutine(ReConnect());
            _ReConnect = false;
        }
    }

    void OnDestroy()
    {
        if (_HeartbeatThread != null)
        {
            _HeartbeatThread.Abort();
        }

        if (_ws != null && _ws.IsConnected)
        {
            _ws.Close(WebSocketSharp.CloseStatusCode.Normal);
            _UseTips = false;
            _ReConnect = false;
            _isReconnection = false;
        }
    }

    /// <summary>
    /// 登出时执行
    /// </summary>
    private void OnLogout()
    {
        if (_HeartbeatThread != null)
        {
            _HeartbeatThread.Abort();
        }

        if (_ws != null && _ws.IsConnected)
        {
            _ws.Close(WebSocketSharp.CloseStatusCode.Normal);
            _UseTips = false;
            _ReConnect = false;
            _isReconnection = false;
        }
    }

    /// <summary>
    /// 初始化websocket并连接
    /// </summary>
    public void WebsocketConnect()
    {
        _ws = new WebSocketSharp.WebSocket(GameStatic.WebsocketUrl + LocalFileManager._Instance._GameData._Token);

        _ws.OnError += (sender, e) => { Debug.LogError($"Websocket发生错误:{sender.ToString()}|{e.Message}"); };

        _ws.OnMessage += (sender, e) => { Debug.Log("收到消息：" + e.Data); _ReceiveMessage?.Invoke(e.Data); };

        _ws.OnOpen += (sender, e) => { Debug.Log("Websocket已连接"); };

        _ws.OnClose += (sender, e) =>
        {
            if (!_isQuitting)
            {
                _isReconnection = true;
                _UseTips = true;
                _ReConnect = true;
            }
            Debug.Log($"Websocket关闭:{ e.Code } - {e.Reason}");
        };

        _ws.ConnectAsync();

        if (_isReconnection)
        {
            if (_ws.IsConnected)
            {
                TipsManager._Instance.OpenTipsText("重连成功");
            }
        }

        StartCoroutine(StartHeartbeat());
    }

    /// <summary>
    /// 重连协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReConnect()
    {
        yield return new WaitForSeconds(_ReconnectionInterval);
        if (_ws.IsAlive)
        {
            _ws.Close(WebSocketSharp.CloseStatusCode.Normal);
        }
        WebsocketConnect();
    }

    /// <summary>
    /// 开启心跳协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartHeartbeat()
    {
        while (_ws.IsConnected == false)
        {
            yield return new WaitForSeconds(0.5f);
        }
        _HeartbeatThread = new Thread(new ThreadStart(() => { Heartbeat(); }));
        _HeartbeatThread.Start();
    }

    /// <summary>
    /// 心跳线程方法
    /// </summary>
    private void Heartbeat()
    {
        while (_ws.IsConnected)
        {
            SendMessageToServer("");
            Thread.Sleep((int)(_HeartbeatInterval * 1000));
        }
    }

    /// <summary>
    /// 向服务器发送消息
    /// </summary>
    /// <param name="msg"></param>
    public void SendMessageToServer(string msg)
    {
        if (_ws.IsConnected)
        {
            _ws.SendAsync(msg, _SendMessageCallback);
        }
        else
        {
            Debug.Log("Websocket无连接");
        }
    }
}
