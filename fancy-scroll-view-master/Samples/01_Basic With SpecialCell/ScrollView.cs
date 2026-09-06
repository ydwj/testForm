/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections.Generic;
using EasingCore;
using UnityEditor;

namespace FancyScrollView.Example01Special
{
    class ScrollView : FancyScrollViewWithSpecial<ItemData>
    {
        [SerializeField] Scroller scroller = default;
        [SerializeField] GameObject cellPrefab = default;

        protected override GameObject CellPrefab => cellPrefab;

        protected override void Initialize()
        {
            base.Initialize();
            scroller.OnValueChanged(UpdatePosition);
        }

        public void UpdateData(IList<ItemData> items)
        {
            UpdateContents(items);
            scroller.SetTotalCount(items.Count);
        }

        public void ScrollTo(float position,float duration,Ease ease)
        {
            scroller.ScrollTo(position,duration,ease);
        }

        public void RefreshScrollView()
        {
            Refresh();
        }
    }
}
