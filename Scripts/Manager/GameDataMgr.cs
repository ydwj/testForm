using Newtonsoft.Json;
using System;
using System.Collections;
using UniRx;
using UnityEngine;
// using Tool.Database; // 如果底层工具库还需要，请取消注释

public class GameDataMgr : Singleton<GameDataMgr>
{
    private PlayerData playerData;

    public GameDataMgr()
    {
    }

    #region 基础属性 (Basic Properties)

    public bool IsGameBGMOpen
    {
        get { return playerData == null ? true : playerData.bIsGameBGMOpen; }
        set { playerData.bIsGameBGMOpen = value; }
    }

    public bool IsGameSoundOpen
    {
        get { return playerData == null ? true : playerData.bIsGameSoundOpen; }
        set { playerData.bIsGameSoundOpen = value; }
    }

    public bool IsGameVibrateOpen
    {
        get { return playerData == null ? true : playerData.bIsVibrateOpen; }
        set { playerData.bIsVibrateOpen = value; }
    }

    public string PlayerName
    {
        get => playerData.mPlayerName;
        set { playerData.mPlayerName = value; }
    }

    public int AccountLvl
    {
        get { return playerData.nAccountLvl; }
        set { playerData.nAccountLvl = value; }
    }

    public DateTime DtOffline
    {
        get { return playerData.dtOfflineTime; }
        set { if (playerData != null) playerData.dtOfflineTime = value; }
    }

    public bool IsRemoveAd => playerData.bRemoveAd;

    #endregion

    #region 初始化逻辑 (Initialization)

    public static bool IsInited { get; private set; }
    public static Subject<Unit> OnInitCompleteMsg = new Subject<Unit>();

    public void Init(Action OnInitComplete)
    {
        if (IsInited)
            return;

        Observable.FromMicroCoroutine<PlayerData>(ob => LoadGameData(ob))
            .LastOrDefault()
            .CatchIgnore()
            .Subscribe(_ =>
                {
                    try
                    {
                        playerData = _;

                        // 判空或根据设置清理数据
                        if (playerData == null /* || GameMgr.GameSetting.bClearDataWhenStart */)
                        {
                            InitPlayerData();
                        }
                        else
                        {
                            // 校验版本号，如果版本不匹配可以强制初始化
                            if (playerData.nVersion != DataVersion.VERSION_CURRENT)
                                InitPlayerData();
                        }

                        // 初始化多语言设定
                        InitLanguage();

                        IsInited = true;
                        OnInitComplete?.Invoke();
                        OnInitCompleteMsg?.OnNext(default);
                    }
                    catch (Exception ex)
                    {
                        Debuger.Log(ex);
                    }
                },
                _ => Debuger.Log(_));
    }

    /// <summary>
    /// 初始化全新的玩家数据
    /// </summary>
    private void InitPlayerData()
    {
        playerData = new PlayerData();
        
        playerData.nVersion = DataVersion.VERSION_CURRENT;
        playerData.bIsGameBGMOpen = true;
        playerData.bIsGameSoundOpen = true;
        playerData.lPlayerID = ToolsMgr.GenerateId();
        
        // playerData.mPlayerName = Localization.Get("str_initialName");
        playerData.nAccountLvl = 1;
        DtOffline = TimeHandler.Now;

        // 根据系统语言自动设置
        playerData.currentLanguage = Application.systemLanguage;
        InitLanguage();

        // 强制保存一次初始数据
        SaveGameData(true);
    }

    private void InitLanguage()
    {
        if (playerData.currentLanguage == SystemLanguage.Chinese ||
            playerData.currentLanguage == SystemLanguage.ChineseSimplified)
            Localization.language = "Chinese";
        else if (playerData.currentLanguage == SystemLanguage.ChineseTraditional)
            Localization.language = "ChineseTraditional";
        else
            Localization.language = "English";
    }

    public void ChangeLanguage(SystemLanguage language)
    {
        playerData.currentLanguage = language;
    }

    #endregion

    #region 数据持久化 (Save & Load & Sync)

    private DateTime saveDataTime = DateTime.MinValue;

    /// <summary>
    /// 保存玩家数据 (带简单的节流功能，避免频繁IO)
    /// </summary>
    public void SaveGameData(bool isForce = false)
    {
        // 过滤引导阶段或无效数据
        // if (FormGuide.IsInGuide || playerData == null) return; 
        if (playerData == null) return;

        if (!isForce && saveDataTime > TimeHandler.Now)
            return;

        playerData.lSaveVersion++;

        // 写入本地文件
        FileMgr.Ins.WriteFile(playerData, ConstHandler.GameDataFileName);

        // CD 1秒
        saveDataTime = TimeHandler.Now.AddSeconds(1);
    }

    /// <summary>
    /// 从本地读取数据
    /// </summary>
    private IEnumerator LoadGameData(IObserver<PlayerData> observer)
    {
        yield return null;
        PlayerData localData = new PlayerData();
        PlayerData syncCloudData = new PlayerData();
        
        FileMgr.Ins.TryReadFile<PlayerData>("gameData.data", out localData);
        bool bHaveSyncCloudData = FileMgr.Ins.TryReadFile<PlayerData>("gameData_cloud.data", out syncCloudData);

        // 如果存在云端拉取的同步数据，优先使用云端数据
        if (bHaveSyncCloudData)
        {
            FileMgr.Ins.DeleteFile("gameData_cloud.data"); // 读取后清理云数据缓存
            observer.OnNext(syncCloudData);
            observer.OnCompleted();
        }
        else
        {
            observer.OnNext(localData);
            observer.OnCompleted();
        }
    }

    /// <summary>
    /// 将 JSON 字符串反序列化为 PlayerData
    /// </summary>
    public PlayerData GetPlayerDataFromJson(string strJson)
    {
        try
        {
            return JsonConvert.DeserializeObject<PlayerData>(strJson);
        }
        catch (Exception ex)
        {
            Debuger.Log(ex);
            return null;
        }
    }

    /// <summary>
    /// 获取当前 PlayerData 的 JSON 字符串
    /// </summary>
    public string GetJsonFromPlayerData()
    {
        try
        {
            return JsonConvert.SerializeObject(playerData);
        }
        catch (Exception ex)
        {
            Debuger.Log(ex);
            return "";
        }
    }



    /// <summary>
    /// 数据版本比对
    /// </summary>
    public bool IsNewerVersionData(PlayerData _playerData)
    {
        if (_playerData == null)
            return false;

        return playerData.lSaveVersion < _playerData.lSaveVersion;
    }

    #endregion
}