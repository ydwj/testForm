using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;

public class AppStart : MonoBehaviour
{
    private bool isLoginning;

    private static AppStart ins;

    ///// <summary>
    ///// 初始化
    ///// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        SceneManager.LoadScene("PottingLogo");
    }

    void Start()
    {
        if (ins != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        ins = this;
        DontDestroyOnLoad(gameObject);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
#if !UNITY_EDITOR
        //Application.targetFrameRate = 60;
        //Application.targetFrameRate = Screen.currentResolution.refreshRate;
        Debug.Log($"Current FrameRate:{Application.targetFrameRate}");
        if (!ISSupportASTC())
            Debug.Log("Device Not Support ASTC!");
#endif

        TimeHandler.Init(() => { Login(); });

        
    }

    public bool ISSupportASTC()
    {
        bool isSupport = false;

        for (TextureFormat i = TextureFormat.ASTC_4x4; i <= TextureFormat.ASTC_12x12; i++)
        {
            isSupport = SystemInfo.SupportsTextureFormat(i);
            if (!isSupport)
                return isSupport;
        }

        return isSupport;
    }

    // Update is called once per frame
    void Update()
    {
        GameDataMgr.Ins.SaveGameData();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            //Application.targetFrameRate = 60;
            if (FormMgr.Ins.IsTopFormInStack<FormMain>())
            {
                //GameDataMgr.Ins.CheckOfflineReward();
            }
        }
        else
        {
            GameDataMgr.Ins.SaveGameData(true);

            GameDataMgr.Ins.DtOffline = TimeHandler.Now;
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            GameDataMgr.Ins.DtOffline = TimeHandler.Now;
    }

    private void OnApplicationQuit()
    {
        GameDataMgr.Ins.DtOffline = TimeHandler.Now;
    }

    void Login()
    {
        BeginGame();
    }



    void BeginGame()
    {
        GameDataMgr.Ins.Init(() =>
        {
            GameSoundMgr.Ins.setBgMusic(GameDataMgr.Ins.IsGameBGMOpen);
            
        });
    }
}