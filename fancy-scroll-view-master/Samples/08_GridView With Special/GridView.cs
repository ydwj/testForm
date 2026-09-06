/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using EasingCore;

namespace FancyScrollView.Example08WithSpecial
{
    class GridView : FancyGridViewWithSpecial<ItemData, Context>
    {
        class CellGroup : DefaultCellGroup
        {
            protected override void HiddenNormalCellContent() { }

            protected override void ResumeNormalCellContent() { }
        }

        [SerializeField] Cell cellPrefab = default;

        protected override void SetupCellTemplate() => Setup<CellGroup>(cellPrefab);

        public Dictionary<uint, GameObject> resetSpecialCells = new Dictionary<uint, GameObject>();

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

        public float SpacingY
        {
            get => spacing;
            set
            {
                spacing = value;
                Relayout();
            }
        }

        public float SpacingX
        {
            get => startAxisSpacing;
            set
            {
                startAxisSpacing = value;
                Relayout();
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            Scroller.onScrollerGetPointerDownPosition?.AddListener(OnScrollerGetPointerDownPosition);
        }

        public void UpdateSelection(int index)
        {
            if (Context.SelectedIndex == index)
            {
                return;
            }

            Context.SelectedIndex = index;
            Refresh();
        }

        public void OnCellClicked(Action<int> callback)
        {
            Context.OnCellClicked = callback;
        }

        private void OnScrollerGetPointerDownPosition(float position)
        {
            var id = DataCount * position;
            id -= id % startAxisCellCount;
            ScrollTo((int)id,0.25f,Ease.InOutCirc);
        }

        public void ScrollTo(int index, float duration, Ease easing, Alignment alignment = Alignment.Middle)
        {
            UpdateSelection(index);
            ScrollTo(index, duration, easing, GetAlignment(alignment));
        }

        public void JumpTo(int index, Alignment alignment = Alignment.Middle)
        {
            UpdateSelection(index);
            JumpTo(index, GetAlignment(alignment));
        }

        float GetAlignment(Alignment alignment)
        {
            switch (alignment)
            {
                case Alignment.Upper: return 0.0f;
                case Alignment.Middle: return 0.5f;
                case Alignment.Lower: return 1.0f;
                default: return GetAlignment(Alignment.Middle);
            }
        }

        public void ResetSpecialCells()
        {
            specialCells.ResetSpecialCellIndexAndObject(resetSpecialCells);
            Refresh();
        }

        public void RefreshGridView()
        {
            Refresh();
        }
    }
}
