using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class BtnAnimation : MonoBehaviour
{
    public bool bAnim = true;
    private float fOriginalScale;

    private void Awake()
    {
        fOriginalScale = transform.localScale.x;

        GetComponent<Button>().OnPointerDownAsObservable().Subscribe(_ =>
        {
            if (bAnim)
                transform.DOScale(fOriginalScale * 1.1f, 0.1f).SetEase(Ease.OutBack);
        }).AddTo(this);

        GetComponent<Button>().OnPointerUpAsObservable().Subscribe(_ =>
        {
            if (bAnim)
                transform.DOScale(fOriginalScale, 0.4f).SetEase(Ease.OutElastic);
        }).AddTo(this);
    }
}
