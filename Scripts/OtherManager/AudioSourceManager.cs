using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AudioSourceManager : MonoBehaviour
{
    public static AudioSourceManager _Instance;          //单例

    [SerializeField]
    private AudioSource _AudioSourceOfBackgroundMusic;      //背景音乐用AudioSource
    [SerializeField]
    private AudioSource _AudioSourceOfSoundEffect;          //音效用AudioSource
    [SerializeField]
    private AudioSource _AudioSourceOfCharacterSpeech;      //人物语音

    void Awake()
    {
        _Instance = this;

        GameStatic.OnMainPanelInit += () =>
        {
            LoadingManager.LoadingStart(this.GetType().ToString());
                SetBGMSwitch(true);
            
            LoadingManager.LoadingComplete(this.GetType().ToString());
        };
    }

    void Start()
    {
        //获取全部Button，遍历添加按键音效监听
        Button[] buttons = CanvasManager._Instance._Canvas.GetComponentsInChildren<Button>(true);

        foreach (var item in buttons)
        {
            item.onClick.AddListener(() => { Boing(); });
        }
    }

    /// <summary>
    /// 设置背景音乐开关
    /// </summary>
    /// <param name="isOn"></param>
    private void SetBGMSwitch(bool isOn)
    {
        if (isOn)
        {
            if (!_AudioSourceOfBackgroundMusic.isPlaying)
            {
                _AudioSourceOfBackgroundMusic.Play();
            }
        }
        else
        {
            _AudioSourceOfBackgroundMusic.Stop();
        }
    }

    public void SetBGMVolume(float value)
    {
        _AudioSourceOfBackgroundMusic.volume = value;
    }
    public void SetSEVolume(float value)
    {
        _AudioSourceOfSoundEffect.volume = value;
    } 
    
    public void SetCSVolume(float value)
    {
        _AudioSourceOfCharacterSpeech.volume = value;
    }

    public void PauseBGM()
    {
        if (_AudioSourceOfBackgroundMusic.isPlaying)
        {
            _AudioSourceOfBackgroundMusic.volume = 0;
        }
    }

    public void ResumeBGM()
    {
        if (_AudioSourceOfBackgroundMusic.isPlaying)
        {
            _AudioSourceOfBackgroundMusic.volume = (float)LocalFileManager._Instance._GameData._BGMVolume;
        }
    }

    public void SetVolume(float value1,float value2,float value3)
    {
        _AudioSourceOfBackgroundMusic.volume = value1;
        _AudioSourceOfSoundEffect.volume = value2;
        _AudioSourceOfCharacterSpeech.volume = value3;
    }

    /// <summary>
    /// 按键音效Boing
    /// </summary>
    private void Boing()
    {
        //_AudioSourceOfSoundEffect.clip = InitialResourcesManager.aud_boing;
        // _AudioSourceOfSoundEffect.Play();
    }

    public void RandomCharaterSpeech()
    {
        int i = Random.Range(0,5)+1;

        PlayCharacterSpeech(InitialResourcesManager.aud_gr_Start[i]);
    }

    public void PlayCharacterSpeech(AudioClip audioClip)
    {
        _AudioSourceOfCharacterSpeech.clip = audioClip;
        _AudioSourceOfCharacterSpeech.Play();
    }
}
