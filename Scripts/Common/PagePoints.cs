using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PagePoints : DirtyNode
{
    public Transform transBgRoot;
    public GameObject goBgPrefab, goCurPoint;

    int nPageCount = 1;
    int nCurIndex = 0;

    public void SetPageCount(int nPageCount)
    {
        this.nPageCount = nPageCount;

        for (int i = 0; i < nPageCount; i++)
        {
            if (i < transBgRoot.childCount)
            {
                transBgRoot.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                Instantiate(goBgPrefab, transBgRoot).SetActive(true);
            }
        }
        for (int i = nPageCount; i < transBgRoot.childCount; i++)
        {
            transBgRoot.GetChild(i).gameObject.SetActive(false);
        }

        SetCurPage(0);
    }

    public void SetCurPage(int nCurIndex)
    {
        Observable.TimerFrame(1, FrameCountType.EndOfFrame).Subscribe(_ =>
         {
             this.nCurIndex = nCurIndex;
             Dirty(true);
         }).AddTo(this);
    }

    protected override void OnReset()
    {
        if (nPageCount <= 0)
        {
            goCurPoint.gameObject.SetActive(false);
        }
        else
        {
            if (nCurIndex < 0)
                nCurIndex = 0;
            if (nCurIndex > nPageCount - 1)
                nCurIndex = nPageCount - 1;

            goCurPoint.transform.position = transBgRoot.GetChild(nCurIndex).position;
        }
    }
}
