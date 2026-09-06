using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using FancyScrollView;
using EasingCore;

public class UIGridAnimFancy : FancyScrollView<UIItemDataFancy>
{
    [SerializeField] Scroller scroller = default;
    [SerializeField] GameObject cellPrefab = default;

    protected override GameObject CellPrefab => cellPrefab;

    protected override void Initialize()
    {
        base.Initialize();
        scroller.OnValueChanged(UpdatePosition);
        cellPrefab.SetActive(false);
    }

    public void UpdateData(IList<UIItemDataFancy> items)
    {
        UpdateContents(items);
        scroller.SetTotalCount(items.Count);
    }
}