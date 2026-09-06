using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "GameSetting")]
public class GameSetting : ScriptableObject
{
    [LabelText("游戏开始进入场景")]
    public SceneType startSceneType = SceneType.MainScene;
    
    [Header("打包必开", order = 0)]
    [LabelText("新手引导开关")]
    public bool bShowGuide = false;

    [LabelText("想要测试的新手引导ID")]
    public int nGuideID = 0;


    [Space(20, order = 0)]
    [Header("打包必关", order = 1)]
    [LabelText("使用本地时间")]
    public bool bUseLocalTime = false;
    [LabelText("GM")]
    public bool bGMTest = false;
    [LabelText("启动时清除数据")]
    public bool bClearDataWhenStart = false;
    [LabelText("解锁所有功能")]
    public bool bUnlockAllFunc = false;
    [LabelText("解锁新手引导所需资源")]
    public bool bUnlockGuideResource = false;

    [LabelText("提升建筑放置上限")]
    public bool bUnlockBuildingLimit= false;
    [LabelText("提升英雄等级上限")]
    public bool bUnlockHeroLvlMaxLimit = false;




    [LabelText("战斗测试英雄")]
    public bool bTestHero = false;
    [LabelText("战斗测试英雄ID")]
    [ShowIf(nameof(bTestHero))]
    public int bTestHeroID = 0;
    
    [LabelText("战斗测试士兵")]
    public bool bTestSoldier = false;
    [LabelText("战斗测试士兵ID")]
    [ShowIf(nameof(bTestSoldier))]
    public int bTestSoldierID = 0;
    
    [LabelText("战斗测试宠物")]
    public bool bTestPet = false;
    [LabelText("战斗测试宠物ID")]
    [ShowIf(nameof(bTestPet))]
    public int bTestPetID = 0;
    
    [LabelText("开启战斗Log")]
    public bool bEnableFightLogs;

    [LabelText("开启顾客求购概率测试")]
    public bool bTestCustomerBuy;

    [Space(20, order = 0)]
    [Header("渠道设置", order = 1)]
    [Header("内购开关", order = 2)]
    public bool bHaveInpurchase = false;
    [Header("是否是海外版")]
    public bool bOversea = false;
}
