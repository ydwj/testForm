
////////////////////////////////////////////////////////////////////////////////////////////////
//Form 所有UI界面的基类
//创建者：LXR
////////////////////////////////////////////////////////////////////////////////////////////////

using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class Form : DirtyNode
{
    public eFormOpenAnim animType;
    public GameObject goPanelBg;
    public List<Form> mListChildForm;
    public List<ScrollRect> childScrollRects;
    private bool mIsPopForm = false;

    [NonSerialized]
    public object[] param;                          //参数
    //UI上的监听事件列表
    protected Dictionary<int, Callback<object[]>> eventDic = null;
    public virtual Dictionary<int, Callback<object[]>> CtorEvent()
    {
        if (eventDic == null)
            eventDic = new Dictionary<int, Callback<object[]>>();
        return eventDic;
    }
    public bool isPopForm
    {
        set
        {
            mIsPopForm = value;
        }
        get
        {
            return mIsPopForm;
        }
    }

    private static string ANIMCLIP_POP = "FormPop";
    private AnimationClip ac_Pop;
    private Animation anim;
    
    public virtual void Show(bool ishsow)
    {
        if (ishsow == IsShow())
            return;

        if (ishsow)
        {
            gameObject.SetActive(true);
            OnShow();
            //GameSoundMgr.Ins.PlaySound(4);
            Dirty(true);
            ResetScroolRects();

            switch (animType)
            {
                case eFormOpenAnim.Scale:
                    {
                        if (goPanelBg != null)
                        {
                            if (ac_Pop == default)
                                ac_Pop = ResMgr.Ins.GetAnimationClip(ANIMCLIP_POP);

                            if (anim == default)
                            {
                                anim = goPanelBg.GetOrAddComponent<Animation>();
                                anim.AddClip(ac_Pop, ac_Pop.name);
                                anim.clip = ac_Pop;
                            }
                            
                            anim.Rewind();
                            anim.Play();
                            //goPanelBg.transform.localScale = new Vector3(0.8f,0.8f,0.8f);
                            //Tween tween = goPanelBg.transform.DOScale(1, 0.6f).SetEase(Ease.OutElastic);
                            
                            //缩放scale时调整scrollrect pos不起作用，用此方法可解决
                            //tween.ObserveEveryValueChanged(_ => _.fullPosition)
                            //    .Where(_ => _ > 0.1f && _ < 0.15f)
                            //    .Subscribe(_ =>
                            //    {
                            //        childScrollRects.ForEach(sr =>
                            //        {
                            //            if (sr != null && sr.vertical)
                            //                sr.verticalNormalizedPosition = 1;
                            //        });
                            //    });
                            Image imgMask = GetComponent<Image>();
                            if (imgMask != null)
                            {
                                imgMask.color = new Color(imgMask.color.r, imgMask.color.g, imgMask.color.b, 0);
                                imgMask.DOFade(0.85f, 0.3f);
                                //imgMask.DOFade(name.Contains("FormMap") ? 0.9f : 0.8f, 0.3f);
                            }
                            //var canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
                            //canvasGroup.alpha = 0;
                            //canvasGroup.DOFade(1, 0.1f);
                        }
                        else
                            Debuger.Log(name + " goBg is null!");
                    }
                    break;
                case eFormOpenAnim.Move:
                    if (goPanelBg != null)
                    {
                        goPanelBg.transform.localPosition = new Vector3(0, 1800f, 0);
                        goPanelBg.transform.DOLocalMoveY(0f, 0.38f).SetEase(Ease.OutBack);
                        Image imgMask = GetComponent<Image>();
                        if (imgMask != null)
                        {
                            imgMask.color = new Color(imgMask.color.r, imgMask.color.g, imgMask.color.b, 0);
                            imgMask.DOFade(0.6f, 0.38f);
                        }
                    }
                    else
                        Debuger.Log(name + " goBg is null!");
                    break;
                case eFormOpenAnim.Half:
                    if (goPanelBg != null)
                    {
                        goPanelBg.transform.localPosition = new Vector3(0, -1800f, 0);
                        goPanelBg.transform.DOLocalMoveY(0f, 0.38f).SetEase(Ease.OutBack);
                        Image imgMask = GetComponent<Image>();
                        if (imgMask != null)
                        {
                            imgMask.color =  new Color(imgMask.color.r, imgMask.color.g, imgMask.color.b, 0);
                            imgMask.DOFade(0.01f, 0.38f);
                        }
                    }
                    else
                        Debuger.Log(name + " goBg is null!");
                    break;
                default:
                    break;
            }
        }
        else
        {
            OnClose();
            gameObject.SetActive(false);
        }
    }

    protected void ResetScroolRects()
    {
        childScrollRects.ForEach(_ =>
        {
            if (_ != null && _.vertical)
                _.verticalNormalizedPosition = 1;
        });
    }

    public virtual bool IsShow()
    {
        return /*enabled && */gameObject.activeSelf;
    }

    protected virtual void OnShow()
    {
        
    }

    protected virtual void OnClose()
    {
        
    }

    Tween tweenClose;
    public virtual void OnClickClose()
    {
        GameSoundMgr.Ins.PlaySound("sound_ui_click3");

        if (tweenClose != null && tweenClose.IsPlaying())
            return;

        switch (animType)
        {
            case eFormOpenAnim.Scale:
                {
                    if (goPanelBg != null)
                    {
                        tweenClose = goPanelBg.transform.DOScale(0.8f, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
                        {
                            CloseSelf();
                        });
                        Image imgMask = GetComponent<Image>();
                        if (imgMask != null)
                            imgMask.DOFade(0, 0.2f).SetDelay(0.1f);
                        //gameObject.GetOrAddComponent<CanvasGroup>().DOFade(0, 0.1f).SetDelay(0.1f);
                    }
                    else
                        Debuger.Log(name + " goBg is null!");
                }
                break;
            case eFormOpenAnim.Move:
                if (goPanelBg != null)
                {
                    tweenClose = goPanelBg.transform.DOLocalMoveY(1800, 0.38f).SetEase(Ease.InBack).OnComplete(()=> 
                    {
                        CloseSelf();
                    });
                    Image imgMask = GetComponent<Image>();
                    if (imgMask != null)
                        imgMask.DOFade(0, 0.38f).SetDelay(0.1f);
                }
                else
                    Debuger.Log(name + " goBg is null!");
                break;
            case eFormOpenAnim.Half:
                if (goPanelBg != null)
                {
                    tweenClose = goPanelBg.transform.DOLocalMoveY(-1800, 0.38f).SetEase(Ease.InBack).OnComplete(() =>
                    {
                        CloseSelf();
                    });
                    Image imgMask = GetComponent<Image>();
                    if (imgMask != null)
                        imgMask.DOFade(0, 0.38f).SetDelay(0.1f);
                }
                else
                    Debuger.Log(name + " goBg is null!");
                break;
            default:
                CloseSelf();
                break;
        }
    }

    protected void CloseSelf()
    {
        if (isPopForm)
            FormMgr.Ins.PopForm(GetType());
        else
            FormMgr.Ins.PopForm();
    }
}

public enum eFormOpenAnim
{
    None,
    Scale,
    Move,
    Half,
}
