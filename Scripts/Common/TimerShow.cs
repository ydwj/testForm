using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TimerShow : MonoBehaviour
{
    public float fTimer = 2f;
    bool bIsInTimer = false;

    private void OnEnable()
    {
        if (bIsInTimer)
            return;
        
        bIsInTimer = true;
        gameObject.SetActive(false);
        ToolsMgr.Timer(fTimer, ()=> {
            gameObject.SetActive(true);
            transform.localScale = Vector3.zero;
            transform.DOScale(1, 0.2f);
            bIsInTimer = false;
        }).AddTo(this);
    }
}
