using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameSoundMgr : UnitySingleton<GameSoundMgr>
{
    [Tooltip("IS")]
    AudioSource _backMusicSource = null;
    AudioSource _SoundSource = null;
    Dictionary<string, AudioClip> mAudioClipDict = new Dictionary<string, AudioClip>();
    Dictionary<string, float> mAudioVolum = new Dictionary<string, float>();
    Dictionary<string, bool> mDicOverlying = new Dictionary<string, bool>();

    protected override void Awake()
    {
        base.Awake();
        _backMusicSource = gameObject.AddComponent<AudioSource>();
        _backMusicSource.loop = true;
        _SoundSource = gameObject.AddComponent<AudioSource>();
        AddAudioClipResource();
        mDicOverlying.Clear();
    }
    private void AddAudioClipResource()
    {
        // var list = ConfigDataMgr.Ins.getAllSoundConfigData();
        // foreach (var item in list)
        // {
        //     if (!mAudioClipDict.ContainsKey(item.SoundName))
        //     {
        //         AudioClip audioClip = Resources.Load<AudioClip>(string.Format("Sound/{0}", item.SoundName));
        //         if (audioClip != null)
        //         {
        //             mAudioClipDict.Add(item.SoundName, audioClip);
        //             mAudioVolum.Add(item.SoundName, item.volume);
        //         }
        //     }
        // }
    }  
    private AudioClip getAudioClip(string strAudioName)
    {
        if (mAudioClipDict.ContainsKey(strAudioName))
        {
            return mAudioClipDict[strAudioName];
        }
        return null;
    }
    private float getAudioVolume(string strAudioName)
    {
        if (!mAudioVolum.ContainsKey(strAudioName))
            return 1.0f;
        return mAudioVolum[strAudioName];
    }

    public void PlayBgMusic(string strMusic)
    {
        StartCoroutine(PlayBgMusicInCoroutine(strMusic));
    }
    IEnumerator PlayBgMusicInCoroutine(string strMusic)
    {
        if (!GameDataMgr.Ins.IsGameBGMOpen)
           yield break;
        var clip = getAudioClip(strMusic);
        if (clip == null)
            yield break;

        yield return null;
        _backMusicSource.clip = clip;
        _backMusicSource.volume = getAudioVolume(strMusic);
        _backMusicSource.Play();
    }

    public void setBgMusic(bool isOn)
    {
        if (_backMusicSource.clip == null)
        {
            PlayBgMusic("music_main");
            return;
        }
        if (isOn)
        {
            _backMusicSource.UnPause();
        }
        else
        {
            if (_backMusicSource.isPlaying)
                _backMusicSource.Pause();
        }
    }

    public void PlaySound(string strName,bool isOverlying = true)
    {
        if (!GameDataMgr.Ins.IsGameSoundOpen)
            return;
        var clip = getAudioClip(strName);
        if (clip == null)
            return;
        var volume = getAudioVolume(strName);
        if (isOverlying == false && mDicOverlying.ContainsKey(strName))
        {
            return;
        }
        else
        {
            _SoundSource.PlayOneShot(clip, volume);
            if (isOverlying == false)
            {
                mDicOverlying[strName] = true;
                ToolsMgr.Timer(clip.length, () =>
                {
                    mDicOverlying.Remove(strName);
                });
            }
        }
    }
}

