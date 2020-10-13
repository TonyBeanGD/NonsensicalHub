using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class SoundRecordingManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{

    /// <summary>
    /// 单利
    /// </summary>
    public static SoundRecordingManager _Instance;

    /// <summary>
    /// 延迟
    /// </summary>
    private float _Ping = 0.5f;

    /// <summary>
    /// 是否开始
    /// </summary>
    private bool _IsStart = false;

    /// <summary>
    /// 上一次的时间记录
    /// </summary>
    private float _LastTime = 0;

    /// <summary>
    /// 声音组件
    /// </summary>
    private AudioClip _AudioClip;

    /// <summary>
    /// 声音源
    /// </summary>
    private AudioSource _AudioSource;

    /// <summary>
    /// 计数
    /// </summary>
    private float _Temp = 0;

    /// <summary>
    /// 保存的音频
    /// </summary>
    private List<AudioClip> _AudioClipList = new List<AudioClip>();

    void Awake()
    {
        _Instance = this;

    }

    // Use this for initialization
    void Start()
    {
        if (_AudioSource == null)
        {
            _AudioSource = this.gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //当长按超过ping值
        if (_IsStart && Time.time - _LastTime > _Ping)
        {
            //print("长按");

            _Temp += Time.deltaTime;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        LongPress(true);
        StartRecord();
        //print("按下");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
#if UNITY_ANDROID || UNITY_IOS
          Microphone.End(null);
        LongPress(false);
        _AudioClipList.Add(_AudioClip);
        byte[] b = WavUtility.FromAudioClip(_AudioClip);

      HttpManager._Instance.StartUploadAudio(@"webSocket/upload", b, (uwr) =>
        {
            if (uwr == null)
            {
                print("上传错误");
                return;
            }

            print("上传完毕");
            CustomerServicePanelManager._Instance.SendVoiceMessage(((int)_Temp).ToString() + "\"", () =>
              {
                  print(_AudioClipList.Count - 1);
                  this.PlayRecord(_AudioClipList.Count - 1);
              });
            _Temp = 0;


            uwr.Dispose();
        }
        );
#endif


        //Save(_AudioClip, Application.dataPath + "/123");
        //PlayRecord();
        //print("抬起");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //print("进入");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //print("离开");
    }

    /// <summary>
    /// 长按
    /// </summary>
    /// <param name="bStart"></param>
    public void LongPress(bool bStart)
    {
        _IsStart = bStart;
        _LastTime = Time.time;
    }

    private void StartRecord()
    {
#if UNITY_ANDROID||UNITY_IOS
        
        _AudioClip = Microphone.Start(null, false, 20, 8000);
#endif
    }

    private void PlayRecord(int index)
    {
        if (index >= 0 && _AudioClipList[index] != null)
        {
            if (!_AudioSource.isPlaying)
            {
                _AudioSource.PlayOneShot(_AudioClipList[index]);
            }
            else
            {
                _AudioSource.Stop();
            }
        }


        //Save(_AudioClip, Application.dataPath + System.DateTime.Now);
    }

    private string ToData()
    {
        float[] floatData = new float[_AudioClip.samples * _AudioClip.channels];

        _AudioClip.GetData(floatData, 0);

        byte[] outData = new byte[floatData.Length * 4];

        System.Buffer.BlockCopy(floatData, 0, outData, 0, outData.Length);
        string str = System.Convert.ToBase64String(outData);

        return str;
    }

    public static void Save(AudioClip clip, string path)
    {
        string filePath = Path.GetDirectoryName(path);
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        using (FileStream fileStream = CreateEmpty(path))
        {
            ConvertAndWrite(fileStream, clip);
            WriteHeader(fileStream, clip);
        }
    }

    private static void ConvertAndWrite(FileStream fileStream, AudioClip clip)
    {
        float[] samples = new float[clip.samples];

        clip.GetData(samples, 0);

        Int16[] intData = new Int16[samples.Length];

        Byte[] bytesData = new Byte[samples.Length * 2];

        int rescaleFactor = 32767; //to convert float to Int16

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }
        fileStream.Write(bytesData, 0, bytesData.Length);
    }

    private static FileStream CreateEmpty(string filepath)
    {
        FileStream fileStream = new FileStream(filepath, FileMode.Create);
        byte emptyByte = new byte();

        for (int i = 0; i < 44; i++) //preparing the header  
        {
            fileStream.WriteByte(emptyByte);
        }

        return fileStream;
    }

    private static void WriteHeader(FileStream stream, AudioClip clip)
    {
        int hz = clip.frequency;
        int channels = clip.channels;
        int samples = clip.samples;

        stream.Seek(0, SeekOrigin.Begin);

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        stream.Write(riff, 0, 4);

        Byte[] chunkSize = BitConverter.GetBytes(stream.Length - 8);
        stream.Write(chunkSize, 0, 4);

        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        stream.Write(wave, 0, 4);

        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        stream.Write(fmt, 0, 4);

        Byte[] subChunk1 = BitConverter.GetBytes(16);
        stream.Write(subChunk1, 0, 4);

        UInt16 one = 1;

        Byte[] audioFormat = BitConverter.GetBytes(one);
        stream.Write(audioFormat, 0, 2);

        Byte[] numChannels = BitConverter.GetBytes(channels);
        stream.Write(numChannels, 0, 2);

        Byte[] sampleRate = BitConverter.GetBytes(hz);
        stream.Write(sampleRate, 0, 4);

        Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2  
        stream.Write(byteRate, 0, 4);

        UInt16 blockAlign = (ushort)(channels * 2);
        stream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        stream.Write(bitsPerSample, 0, 2);

        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        stream.Write(datastring, 0, 4);

        Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        stream.Write(subChunk2, 0, 4);


    }
}
