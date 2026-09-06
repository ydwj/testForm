using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SimpleProgress : MonoBehaviour
{
    public Image imgSlider;
    //public Slider slider;
    public Text txtProgress;

    [HideInInspector]
    public bool IsGoing;

    float fTimerTarget;
    float fTimer;
    Action callBack;

    public void InitProgress(float _fTimer, Action _callBack = null)
    {
        gameObject.SetActive(true);
        SetProgress(0);
        IsGoing = false;

        fTimerTarget = _fTimer;
        callBack = _callBack;
        fTimer = 0;

        IsGoing = true;
        
        return;

        //Observable.Timer(TimeSpan.FromSeconds(_fDelay)).Subscribe(_ => IsGoing = true);
    }

    public void Stop()
    {
        gameObject.SetActive(false);
        SetProgress(0);
        IsGoing = false;
        callBack = default;
        fTimer = 0;
    }

    public void Update()
    {
        if (IsGoing)
        {
            fTimer += Time.deltaTime;
            float fProgress = fTimer / fTimerTarget;
            
            SetProgress(fProgress);
            if (txtProgress != null)
                txtProgress.text = string.Format("{0:f1}%", fProgress * 100);

            if (fProgress >= 1)
            {
                IsGoing = false;
                Observable.TimerFrame(1, FrameCountType.EndOfFrame)
                    .Subscribe(_ =>
                    {
                        callBack?.Invoke();
                        gameObject.SetActive(false);
                    }).AddTo(this);
            }
        }
    }

    public void SetProgress(float fValue)
    {
        float fValTmp = Mathf.Clamp(fValue, 0, 1);
        //slider.value = fValTmp;
        imgSlider.fillAmount = fValTmp;
    }
}
