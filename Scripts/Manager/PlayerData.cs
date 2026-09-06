using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DataVersion
{
    // 当前存档版本号，用于升级旧存档时做校验
    public const int VERSION_CURRENT = 100;
}

[Serializable]
public class PlayerData
{
    #region 版本与存档信息 (Version & Save Info)
    
    public int nVersion = 0;
    public long lSaveVersion = 0;
    public long lPlayerID = 0;
    
    #endregion

    #region 基础设置 (Basic Settings)
    
    public SystemLanguage currentLanguage = SystemLanguage.English;
    public bool bIsGameBGMOpen = true;
    public bool bIsGameSoundOpen = true;
    public bool bIsVibrateOpen = true;
    public bool bRemoveAd = false;
    
    #endregion

    #region 玩家基础信息 (Player Basic Info)
    
    public string mPlayerName;
    public int nAccountLvl = 1;
    
    #endregion

    #region 时间记录 (Time Records)
    
    /// <summary>
    /// 上次离线时间
    /// </summary>
    public DateTime dtOfflineTime = DateTime.MinValue;

    /// <summary>
    /// 上次登录的日期 (用于计算每日签到/每日刷新)
    /// </summary>
    public DateTime dateTimeLastSignin = DateTime.MinValue;
    
    #endregion

    #region 基础框架数据 (Base Framework Data - 按需保留)

    /// <summary>
    /// 新手引导进度记录 <引导ID, 是否完成>
    /// </summary>
    public Dictionary<int, bool> mDicGuideData = new Dictionary<int, bool>();

    /// <summary>
    /// 游戏中已解锁的通用功能记录集合
    /// </summary>
    public HashSet<int> unlockFuncs = new HashSet<int>();

    #endregion

    public PlayerData()
    {
    }
}