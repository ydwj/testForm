using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;

public class FormLoading : Form
{
    public Transform transTitle;
    public float processValue;
    public Slider sliderProgress;
    public Text txt_Sld;

    protected override void OnShow()
    {
        sliderProgress.value = 0;
        processValue = 0;
        transTitle.localScale = Vector3.zero;
        goPanelPrivacyPolicy.SetActive(false);

        ToolsMgr.Timer(0.2f, () =>
        {
            PlayAnim(() =>
            {
                // bool bShowPrivacyPolicy = GameDataMgr.Ins.GetBathEnterCount(1) == 0;
                // if (bShowPrivacyPolicy)
                // {
                //     ShowPrivacyPolicy(() => StartCoroutine(StartLoadingScene((string)param[0], (LoadSceneMode)param[1], (Action<object>)param[2])));
                // }
                // else
                    StartCoroutine(StartLoadingScene((string)param[0], (LoadSceneMode)param[1], (Action<object>)param[2]));
            });
        }).AddTo(this);
    }
    
    void Update()
    {
        sliderProgress.value = processValue;
        txt_Sld.text = string.Format(Localization.Get("str_loadingprogress"), (int)(Mathf.Clamp((processValue * 100), 0, 100)) + "%");
    }
    
    IEnumerator StartLoadingScene(string strSceneName, LoadSceneMode loadSceneMode, Action<object> OnLoadingComplete)
    {
        float maxSpeed = 0.02f;
        AsyncOperation op = SceneManager.LoadSceneAsync(strSceneName, loadSceneMode);

        // 禁止载入后自己切换
        op.allowSceneActivation = false;
        // isDone后再加载最后的10%
        while (op.progress < 0.9f)
        {
            // 连续加载
            while (processValue < op.progress)
            {
                processValue += maxSpeed;
                yield return new WaitForEndOfFrame();
            }
        }
        while (processValue < 1)
        {
            processValue += maxSpeed;
            yield return new WaitForEndOfFrame();
        }
        op.allowSceneActivation = true;
        while (!op.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
                            
        CloseSelf();
        OnLoadingComplete?.Invoke(default);
        OnLoadingComplete = null;
    }
    
    void LoadScene(string strSceneName, LoadSceneMode loadSceneMode, Action<object> OnLoadingComplete)
    {
        var OnLoadAsync = SceneManager.LoadSceneAsync(strSceneName, loadSceneMode);

        AsyncOperation[] arrLoadingOps = { OnLoadAsync };

        //OnLoadingComplete = (Action<object>)param[1];
        object obj = null;

        if (arrLoadingOps != default)
        {
            IObservable<AsyncOperation> AllOps = null;
            var totalProgressObservable = new ScheduledNotifier<float>();
            
            foreach (var item in arrLoadingOps)
            {
                IObservable<AsyncOperation> obsOp = item.AsAsyncOperationObservable(totalProgressObservable);
                if (AllOps == null)
                    AllOps = obsOp;

                AllOps.Merge(obsOp);
            }

            totalProgressObservable.Subscribe(_ =>
            {
                sliderProgress.value = _;
            });

            AllOps.ObserveOnMainThread(MainThreadDispatchType.EndOfFrame)
                .Subscribe(_ =>
                {
                    CloseSelf();
                    
                    OnLoadingComplete?.Invoke(obj);
                    arrLoadingOps = null;
                    OnLoadingComplete = null;
                }).AddTo(this);
        }
        else
        {
            Observable.Timer(TimeSpan.FromSeconds(0.3f)).Subscribe(_ =>
            {
                OnLoadingComplete?.Invoke(obj);
                arrLoadingOps = null;
                OnLoadingComplete = null;
            }).AddTo(this);
        }
    }

    void PlayAnim(Action OnComplete)
    {
        transTitle.DOScale(1, 0.5f).SetEase(Ease.OutBack).OnComplete((() =>
        {
            OnComplete?.Invoke();
        }));
    }
    
    //==================PanelPrivacyPolicy
    public GameObject goPanelPrivacyPolicy;

    private Action OnAgree;
    void ShowPrivacyPolicy(Action OnAgree)
    {
        goPanelPrivacyPolicy.SetActive(true);
        this.OnAgree = OnAgree;
    }

    public void OnClickAgree()
    {
        goPanelPrivacyPolicy.SetActive(false);
        OnAgree?.Invoke();
    }
    
    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}