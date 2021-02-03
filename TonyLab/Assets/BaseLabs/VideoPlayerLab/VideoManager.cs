using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using NonsensicalKit;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using System;

public class VideoManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform videoSpace;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private GameObject videoPlayerTarget;

    [SerializeField] private Button btn_Play;
    [SerializeField] private Button btn_Pause;
    [SerializeField] private Button btn_Full;
    [SerializeField] private Button btn_Reset;
    [SerializeField] private Image img_FullSceneBackground;
    [SerializeField] private Slider sld_Progress;

    [SerializeField] private VideoControlSpace videoControlSpace;

    [SerializeField] private Button btn_Sound;
    [SerializeField] private Button btn_SoundMute;
    [SerializeField] private Slider sld_Sound;
    [SerializeField] private GameObject SoundParent;

    private VideoPlayer videoPlayer;

    private bool fullScene;
    private bool _isDrag;
    private Transform _oldParent;
    private bool _needWait;
    private bool isPlaying;
    private float sound = 1;

    private string NewestUrl;

    private float controlSpaceTimer;
    private bool lockControlTimer;

    private void Awake()
    {
        btn_Play.onClick.AddListener(() => VideoPlay());
        btn_Pause.onClick.AddListener(VideoPause);
        btn_Full.onClick.AddListener(FullScene);
        btn_Reset.onClick.AddListener(FullScene);
        sld_Progress.GetComponent<VideoProgressSlider>().OnProgressSliderDrag += OnDragChanged;

        if (btn_Sound != null)
        {
            btn_Sound.onClick.AddListener(SwitchSoundPanel);
            btn_SoundMute.onClick.AddListener(SwitchSoundPanel);
            sld_Sound.onValueChanged.AddListener(OnSoundChange);
        }


        if (videoControlSpace != null)
        {
            videoControlSpace.OnControlPointEnter += OnControlSpaceEnter;
            videoControlSpace.OnControlPointExit += OnControlSpaceExit;
        }
    }
    private void Update()
    {
        if (videoPlayer != null)
        {
            if (_isDrag == true)
            {
                videoPlayer.frame = (long)(sld_Progress.value * videoPlayer.frameCount);
            }
            else
            {
                sld_Progress.value = (float)videoPlayer.frame / videoPlayer.frameCount;
            }

            if (lockControlTimer)
            {
                controlSpaceTimer = 0;
            }
            else
            {
                controlSpaceTimer += Time.deltaTime;
            }
            if (videoControlSpace != null) videoControlSpace.gameObject.GetComponent<CanvasGroup>().alpha = Time.deltaTime < 5 ? 1 : 0;
        }

        if (fullScene && Input.GetKeyDown(KeyCode.Escape))
        {
            SwitchFullScene(false);
        }
    }

    private void SwitchSoundPanel()
    {
        SoundParent.gameObject.SetActive(SoundParent.activeSelf);
    }

    private void OnSoundChange(float value)
    {
        if (value != sound)
        {
            sound = value;
            btn_Sound.gameObject.SetActive(value != 0);
            btn_SoundMute.gameObject.SetActive(value == 0);
            videoPlayer?.SetDirectAudioVolume(0, value);
        }
    }

    private void OnControlSpaceEnter()
    {
        lockControlTimer = true;
    }

    private void OnControlSpaceExit()
    {
        lockControlTimer = false;
    }

    private void CreateVidoePlayer(string url)
    {
        if (videoPlayer != null)
        {
            Destroy(videoPlayer);
        }

        videoPlayer = videoPlayerTarget.AddComponent<VideoPlayer>();

        InitvideoPlayer();

        videoPlayer.url = url;
    }

    private void CreateVidoePlayer(VideoClip clip)
    {
        if (videoPlayer != null)
        {
            Destroy(videoPlayer);
        }

        videoPlayer = videoPlayerTarget.AddComponent<VideoPlayer>();

        InitvideoPlayer();
        videoPlayer.clip = clip;
    }

    private void InitvideoPlayer()
    {
        videoPlayer.playOnAwake = false;
        videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.frameReady += OnNewFrame;
        videoPlayer.loopPointReached += OnLoopPoint;
        videoPlayer.errorReceived += OnErrorReceived;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
    }

    private void OnErrorReceived(VideoPlayer source, string message)
    {
        Debug.Log("视频播放错误" + message);
    }

    private void OnLoopPoint(VideoPlayer videoPlayer)
    {
        VideoStop();
    }

    private IEnumerator PlayVideoByUrl(string url, Action<bool> callback)
    {
        using (var uwr = UnityWebRequest.Get(url))
        {
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogWarning("加载视频出错" + uwr.error);
                callback(false);
                yield break;
            }
            callback(true);
        }

    }

    public void PlayVideo(string url, bool needwait = true)
    {
        Debug.Log("播放视频：" + url);
        if (videoPlayer != null)
        {
            VideoPause();
            videoPlayer.frame = 0;
        }
        NewestUrl = url;
        StartCoroutine(PlayVideoByUrl(url, (exist) =>
        {
            if (exist && url == NewestUrl)
            {
                CreateVidoePlayer(url);

                _needWait = needwait;
                videoPlayer.frame = 0;
                VideoPlay(true);
            }
        }));
    }

    public void PlayVideo(VideoClip clip, bool needwait = true)
    {
        Debug.Log("播放视频：" + clip.name);
        if (videoPlayer != null)
        {
            VideoPause();
            videoPlayer.frame = 0;
        }
        if (clip != null)
        {
            CreateVidoePlayer(clip);

            _needWait = needwait;
            videoPlayer.frame = 0;
            VideoPlay(true);
        }
    }

    private bool waitBuffer = false;

    private void OnNewFrame(VideoPlayer source, long frameIdx)
    {
        if (_needWait)
        {
            if (waitBuffer == false)
            {
                waitBuffer = true;
            }
            else
            {
                waitBuffer = false;
                _needWait = false;
                VideoPause();
            }
        }
    }

    private void OnDragChanged(bool isDrag)
    {
        _isDrag = isDrag;
        if (isDrag)
        {

            videoPlayer.Pause();
        }
        else
        {
            if (isPlaying)
            {
                videoPlayer.Play();
            }
        }
    }

    private void VideoStop()
    {
        videoPlayer.frame = 0;

        VideoPause();
    }

    private void FullScene()
    {
        fullScene = !fullScene;
        btn_Full.gameObject.SetActive(!fullScene);
        btn_Reset.gameObject.SetActive(fullScene);
        SwitchFullScene(fullScene);
    }

    private void SwitchFullScene(bool isFull)
    {
        fullScene = isFull;
        img_FullSceneBackground.gameObject.SetActive(isFull);
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        if (isFull)
        {
            _oldParent = videoSpace.parent;
            videoSpace.SetParent(canvasRect);
        }
        else
        {
            videoSpace.SetParent(_oldParent);
        }
        videoSpace.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        videoSpace.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        videoSpace.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        videoSpace.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
    }

    private void VideoPlay(bool prepare = false)
    {
        if (videoPlayer == null)
        {
            return;
        }
        isPlaying = true;
        btn_Play.gameObject.SetActive(false);
        btn_Pause.gameObject.SetActive(true);
        if (prepare)
        {
            //videoPlayer.Prepare();    
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Play();
        }
    }

    private void VideoPause()
    {
        if (videoPlayer != null)
        {
            isPlaying = false;
            btn_Play.gameObject.SetActive(true);
            btn_Pause.gameObject.SetActive(false);
            videoPlayer.Pause();
        }
    }
}
