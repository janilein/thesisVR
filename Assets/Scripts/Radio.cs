using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using Un4seen.Bass;

public class Radio : MonoBehaviour
{
    public string url = "http://icecast.vrtcdn.be/stubru-high.mp3"; //"http://www.radiomaria.be:8000/RadioMaria-96";

    private int stream;

    private bool playing;

    // Use this for initialization
    void Start()
    {
        playing = false;
        Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_NET_PLAYLIST, 0);

        Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

        stream = Bass.BASS_StreamCreateURL(url, 0, BASSFlag.BASS_DEFAULT, null, IntPtr.Zero);

        //PlayStream(url);
    }

    void Awake()
    {
        BassNet.Registration("jan.hardy@student.kuleuven.be", "2X1933429281823");
    }

    public void PlayStream()
    {
        if (stream != 0)
        {
            Bass.BASS_ChannelPlay(stream, false);
        }
        else
        {
            Debug.Log("BASS Error Code = " + Bass.BASS_ErrorGetCode());
        }
    }

    public void StopStream()
    {
        Bass.BASS_ChannelStop(stream);
    }

    // Get the Channel Information
    public string GetChannelInfo()
    {
        BASS_CHANNELINFO info = new BASS_CHANNELINFO();
        Bass.BASS_ChannelGetInfo(stream, info);
        return info.ToString();
    }

    public void SetVolume(float value)
    {
        Bass.BASS_SetVolume(value);
    }

    void OnApplicationQuit()
    {
        // free the stream
        Bass.BASS_StreamFree(stream);
        // free BASS
        Bass.BASS_Free();
    }

    public void ToggleRadio()
    {
            if (playing)
            {
                StopStream();
            }
            else
            {
                PlayStream();
            }
            playing = !playing;
    }
}