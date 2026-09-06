using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FancyScrollView;
using UnityEngine.UI;

public class UISlotFancy : FancyScrollRectCell<UIItemDataFancy, UIContext>
{
    public override void Initialize()
    {
    }

    public override void UpdateContent(UIItemDataFancy itemData)
    {
    }

    protected override void UpdatePosition(float normalizedPosition, float localPosition)
    {
        base.UpdatePosition(normalizedPosition, localPosition);
    }
}

[System.Serializable]
public class UIItemDataFancy
{
};