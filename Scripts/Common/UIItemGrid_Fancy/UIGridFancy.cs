using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using FancyScrollView;
using EasingCore;

public class UIGridFancy : FancyScrollRect<UIItemDataFancy, UIContext>
{
    [SerializeField] GameObject cellPrefab = default;

    protected override float CellSize => Scroller.ScrollDirection == ScrollDirection.Vertical ? (cellPrefab.transform as RectTransform).sizeDelta.y : (cellPrefab.transform as RectTransform).sizeDelta.x;
    protected override GameObject CellPrefab => cellPrefab;
    public int DataCount => ItemsSource.Count;
    protected override void Initialize()
    {
        base.Initialize();

        cellPrefab.SetActive(false);
    }

    public float PaddingTop
    {
        get => paddingHead;
        set
        {
            paddingHead = value;
            Relayout();
        }
    }

    public float PaddingBottom
    {
        get => paddingTail;
        set
        {
            paddingTail = value;
            Relayout();
        }
    }

    public float Spacing
    {
        get => spacing;
        set
        {
            spacing = value;
            Relayout();
        }
    }

    public void OnCellClicked(Action<int> callback)
    {
        Context.OnCellClicked = callback;
    }
    
    public void OnCellClicked_Data(Action<UIItemDataFancy> callback)
    {
        Context.OnCellClicked_Data = callback;
    }

    public void UpdateData(IList<UIItemDataFancy> items)
    {
        UpdateContents(items);
    }

    public void ScrollTo(int index, float duration, Ease easing, UIAlignment alignment = UIAlignment.Middle)
    {
        UpdateSelection(index);
        ScrollTo(index, duration, easing, GetAlignment(alignment));
    }

    public void JumpTo(int index, UIAlignment alignment = UIAlignment.Middle)
    {
        UpdateSelection(index);
        JumpTo(index, GetAlignment(alignment));
    }


    float GetAlignment(UIAlignment alignment)
    {
        switch (alignment)
        {
            case UIAlignment.Upper: return 0.0f;
            case UIAlignment.Middle: return 0.5f;
            case UIAlignment.Lower: return 1.0f;
            default: return GetAlignment(UIAlignment.Middle);
        }
    }

    void UpdateSelection(int index)
    {
        if (Context.SelectedIndex == index)
        {
            return;
        }

        Context.SelectedIndex = index;
        Refresh();
    }
}

public class UIContext : FancyScrollRectContext
{
    public int SelectedIndex = -1;
    public Action<int> OnCellClicked;
    public Action<UIItemDataFancy> OnCellClicked_Data;
}

public enum UIAlignment
{
    Upper,
    Middle,
    Lower,
}