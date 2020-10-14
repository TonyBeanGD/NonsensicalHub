using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UniWebViewTest : MonoBehaviour
{

    public RectTransform WebBG;

    private UniWebView WebView;

    private void OnReceivedMessage(UniWebView webView, UniWebViewMessage message)
    {
        CloseWebView();
    }

    private void OnLoadComplete(UniWebView webView, int success, string errorMessage)
    {
        webView.Show();
        webView.AddJavaScript("var isDown=false;var isMove=true;var move_div;var m_move_x,m_move_y,m_down_x,m_down_y,dx,dy,md_x,md_y,ndx,ndy;function createWindow(){if(!document.getElementById('movediv')){isDown=false;isMove=true;var cont0=document.createElement('div');cont0.id='movediv';cont0.style.position='absolute';cont0.style.height='100px';cont0.style.width='100px';cont0.style.background='aqua';cont0.style.zIndex='9998';cont0.innerHTML='返回';cont0.onselectstart=function(){return false};cont0.style.textAlign='center';cont0.style.lineHeight='100px';var first=document.body.firstChild;document.body.insertBefore(cont0,first);move_div=document.getElementById('movediv');cont0.addEventListener('touchstart',down,false);cont0.addEventListener('touchend',up,false);document.addEventListener('touchmove',move,false);up()}}function down(e){isDown=true;isMove=false;m_down_x=e.targetTouches[0].pageX;m_down_y=e.targetTouches[0].pageY;dx=move_div.offsetLeft;dy=move_div.offsetTop;md_x=m_down_x-dx;md_y=m_down_y-dy}function move(e){isMove=true;dx=move_div.offsetLeft;dy=move_div.offsetTop;m_move_x=e.targetTouches[0].pageX;m_move_y=e.targetTouches[0].pageY;if(isDown){ndx=m_move_x-md_x;ndy=m_move_y-md_y;var width=document.documentElement.clientWidth;var height=document.documentElement.clientHeight;var rect=move_div.getBoundingClientRect();if(event.pageX<0||width-event.pageX<0||event.pageY<0||height-event.pageY<0){up()}if(ndx<0){ndx=0}if(ndx>width-rect.width){ndx=width-rect.width}if(ndy<0){ndy=0}if(ndy>height-rect.height){ndy=height-rect.height}move_div.style.left=ndx+'px';move_div.style.top=ndy+'px'}}function up(e){if(isMove==false){window.location.href='uniwebview://JustMessage'}else{var width=document.documentElement.clientWidth;var height=document.documentElement.clientHeight;var rect=move_div.getBoundingClientRect();if(rect.top<=rect.top&&rect.top<=height-rect.bottom&&rect.top<=rect.left&&rect.top<=width-rect.right){move_div.style.top='0px';move_div.style.bottom='auto'}if(height-rect.bottom<=rect.top&&height-rect.bottom<=height-rect.bottom&&height-rect.bottom<=rect.left&&height-rect.bottom<=width-rect.right){move_div.style.top='auto';move_div.style.bottom='0px'}if(rect.left<=rect.top&&rect.left<=height-rect.bottom&&rect.left<=rect.left&&rect.left<=width-rect.right){move_div.style.left='0px';move_div.style.right='auto'}if(width-rect.right<=rect.top&&width-rect.right<=height-rect.bottom&&width-rect.right<=rect.left&&width-rect.right<=width-rect.right){move_div.style.left='auto';move_div.style.right='0px'}}isDown=false;isMove=false}", (payload) => { });
        webView.EvaluateJavaScript("createWindow();", (payload) => { });
    }

    public void Open_TestPage()
    {
#if UNITY_IOS || UNITY_ANDROID

        LocalNotification.SendNotification(1, 1, "游戏大厅", "游戏已开启", new Color32(0xff, 0x44, 0x44, 255));

        if (WebView == null)
        {
            WebView = gameObject.AddComponent<UniWebView>();
            WebView.ReferenceRectTransform = WebBG;
            WebView.OnMessageReceived += OnReceivedMessage;
            WebView.OnPageFinished += OnLoadComplete;
        }

        WebView.Load("https://demo.leg666.com/");
#else
        Application.ExternalEval("document.getElementById('mydiv').style.display='block';document.getElementById('myframe').src = 'https://demo.leg666.com';");
        Application.ExternalCall("createWindow");
#endif

    }

    void CloseWebView()
    {
        Destroy(WebView);
        WebView = null;
    }

    void OnDestroy()
    {
        CloseWebView();
    }
}
