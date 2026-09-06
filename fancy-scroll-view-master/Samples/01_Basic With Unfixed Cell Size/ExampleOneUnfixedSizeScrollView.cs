/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: star
 * Created On: 2024-08-29
 * Description: unfixed size scrollView for example 1
 */

using System.Collections.Generic;
using UnityEngine;

namespace FancyScrollView.Example01WithUnfixedSize
{
    public class ExampleOneUnfixedSizeScrollView : FancyScrollViewWithUnfixedSize<ExampleOneUnfixedSizeItemData>
    {
        #region Fields & Properties

        public int itemCount => itemDataContainer.Count;

        [SerializeField]
        private RectTransform m_CellContainer;

        protected override RectTransform cellRoot
        {
            get => m_CellContainer;
            set => m_CellContainer = value;
        }

        [SerializeField]
        private GameObject m_CellPrefab;

        protected override GameObject cellPrefab
        {
            get => m_CellPrefab;
            set => m_CellPrefab = value;
        }

        #endregion

        public virtual void UpdateData(IList<ExampleOneUnfixedSizeItemData> itemsSource, bool holding)
        {
            UpdateContents(itemsSource, holding);
        }

        public IList<ExampleOneUnfixedSizeItemData> GetData()
        {
            return itemDataContainer;
        }

        public virtual void Init()
        {
            Initialize();

            scroller.OnValueChanged(UpdatePosition);
        }
    }
}