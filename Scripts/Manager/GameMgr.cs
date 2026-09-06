using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameMgr : UnitySingleton<GameMgr>
{
    public GameSetting gameSetting;
    public static GameSetting GameSetting => Ins.gameSetting;
    public Camera uiCamera;
    [HideInInspector] public Camera mainCamera;
    public Canvas uiCanvas;

    public AppStart appStart;

    public static float UIScreenHeight { get; private set; }

    SceneType mType = SceneType.Count;

    private bool bFirstEnterMain = false;


    //当前战斗相关数据

    private void Start()
    {
        UIScreenHeight = (uiCanvas.transform as RectTransform).rect.height;
    }

    public void switchScene(SceneType type, params object[] param)
    {
        Debug.Log(
            "7777777");
        mType = type;
  
        ResMgr.Ins.ClearPools();
        switch (mType)
        {
            case SceneType.Logo:
                SceneManager.LoadScene(0);
                break;
            case SceneType.Enter:
                //{
                //GameSoundMgr.Ins.PlayBgMusic("music_main");
                LoadScene(SceneManager.GetSceneByBuildIndex((int)param[0]).name, LoadSceneMode.Single, obj =>
                {
                    var objScene = SceneManager.GetSceneByBuildIndex((int)param[0]);
                    SceneManager.SetActiveScene(objScene);
           
                });
                //}
                break;
            case SceneType.MainScene:
                LoadScene("MainScene", LoadSceneMode.Single, _ =>
                {
                    var a = ResMgr.Ins.GetResourceInstantiate("TimelineMgr",
                       transform, ResouceType.PrefabItem);

                    // a.transform.position= Vector3.zero;
                    GameSoundMgr.Ins.PlayBgMusic("music_main");
                    FormMgr.Ins.OpenForm_Replace<FormMain>();


                    mainCamera = Camera.main;

                });
                break;
     
        }
    }



    void LoadScene(string strSceneName, LoadSceneMode loadSceneMode, Action<object> OnLoadingComplete)
    {
        FormMgr.Ins.OpenForm_Replace<FormLoading>(strSceneName, loadSceneMode, OnLoadingComplete);
    }

    /// <summary>
    /// 禁止游戏中所有点击事件
    /// </summary>
    /// <param name="bEnable"></param>
    public void EnableTouch(bool bEnable)
    {
        MsgMgr.Ins.Publish(GameMsg.Main_EnableTouch, bEnable);
    }

    //public string GetItemTypeIcon(eItemType type)
    //{
    //    string name = "";
    //    switch (type)
    //    {
    //        case eItemType.Gold:
    //            name = "icon_money";
    //            break;
    //        case eItemType.Diamond:
    //            name = "icon_Diamond";
    //            break;
    //    }
    //    return name;
    //}

    
}