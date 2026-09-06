using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class FormMain : Form
{
    
    
    private void Awake()
    {
        // GameDataMgr.Ins.OnGetExp.Subscribe(_ =>
        // {
        // }).AddTo(this);
   
        MsgMgr.Ins.Subscribe(GameMsg.Update_Main, OnReset);
        

    }

    private void Start()
    {
  
    }

    protected override void OnShow()
    {
        base.OnShow();

   

    }

    protected override void OnReset()
    {
     
        
    
    }

    private void Update()
    {
    }

    

    #region Button




 

    #endregion

}