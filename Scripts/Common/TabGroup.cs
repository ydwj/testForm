using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public bool bChangeSprite = true;
    [ShowIf(nameof(bChangeSprite))]
    public Sprite spSeleted, spUnSeleted;
    [ShowIf(nameof(bChangeSprite))]
    public bool bResetNativeSize = false;

    [Space(10)]
    public bool bMoveTab = false;
    [ShowIf(nameof(bMoveTab))]
    public float fMovefSeletedPosYOffset = 10f;

    [Space(10)]
    public bool bScaleTab = false;
    [ShowIf(nameof(bScaleTab))]
    public float fScaleSeletedScaleOffset = 1f;

    private Action<int> OnTabChange;

    [HideInInspector]
    public Tab[] listTabs;

    public int CurTabIndex { get; private set; } = -1;

    private void Awake()
    {
        listTabs = GetComponentsInChildren<Tab>().Where(_ => _.gameObject.activeSelf).ToArray();
        for (int i = 0; i < listTabs.Length; i++)
        {
            listTabs[i].Init(this, i);
        }

        UpdateTabs();
    }

    public void UpdateTabs()
    {
        int nIndex = CurTabIndex != -1 ? CurTabIndex : 0;
        if (nIndex != -1 && !listTabs[nIndex].gameObject.activeSelf)
            nIndex = listTabs.FirstOrDefault(_ => _.gameObject.activeSelf).nIndex;

        SwitchTab(nIndex);
    }

    public void SwitchTab(int nTabIndex)
    {
        if (nTabIndex == CurTabIndex)
            return;

        CurTabIndex = nTabIndex;

        for (int i = 0; i < listTabs.Length; i++)
        {
            listTabs[i].Select(i == nTabIndex);
        }

        OnTabChange?.Invoke(nTabIndex);
    }

    public void AddChangeListener(Action<int> _OnTabChange)
    {
        OnTabChange = _OnTabChange;
    }
}
