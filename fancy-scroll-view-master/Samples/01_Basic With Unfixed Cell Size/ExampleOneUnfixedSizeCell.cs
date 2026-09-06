/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: star
 * Created On: 2024-08-29
 * Description: Unfixed size cell of example 1
 */

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FancyScrollView.Example01WithUnfixedSize
{
    public class ExampleOneUnfixedSizeCell : FancyCellWithUnfixedSize<ExampleOneUnfixedSizeItemData>
    {
        public override RectTransform rectTransform => cellRectTransform;

        protected RectTransform cellRectTransform { get; set; }

        [SerializeField]
        private RectTransform m_Content;

        protected RectTransform content => m_Content;

        [SerializeField]
        private TMP_Text m_Label;

        protected TMP_Text label => m_Label;

        protected RectTransform labelRectTransform { get; set; }

        protected GridLayoutGroup gridLayoutGroup { get; set; }

        protected List<ExampleOneUnfixedSizeSubCell> subCells { get; set; }

        [SerializeField]
        private GameObject subCellPrefab;

        public override void Initialize()
        {
            base.Initialize();

            cellRectTransform = gameObject.GetComponent<RectTransform>();

            gridLayoutGroup = gameObject.GetComponentInChildren<GridLayoutGroup>();

            labelRectTransform = label.gameObject.GetComponent<RectTransform>();

            subCells = new List<ExampleOneUnfixedSizeSubCell>();
        }

        protected virtual void ExpandSubCells(int expandCount)
        {
            for (var i = 0; i < expandCount; i++)
            {
                var subCellObj = Instantiate(subCellPrefab, content);

                var subCell = subCellObj.GetComponent<ExampleOneUnfixedSizeSubCell>();

                subCell.Init();

                subCells.Add(subCell);
            }
        }

        public override void UpdateContent(ExampleOneUnfixedSizeItemData itemData)
        {
            var data = itemData;

            var labelStr = data.label;

            label.text = labelStr;

            if (itemData.values.Length > subCells.Count)
            {
                ExpandSubCells(itemData.values.Length - subCells.Count);
            }

            var values = data.values;

            for (var i = 0; i < subCells.Count; i++)
            {
                var subCell = subCells[i];

                var visible = i < values.Length;

                subCell.SetVisible(visible);

                if (visible)
                {
                    subCell.SetValue(values[i]);
                }
            }

            Canvas.ForceUpdateCanvases();

            var labelSize = labelRectTransform.sizeDelta;

            var contentHeight = content.sizeDelta.y;

            var totalSize = new Vector2(labelSize.x, labelSize.y + contentHeight);

            cellRectTransform.sizeDelta = totalSize;

            labelRectTransform.localPosition = new Vector3(0f, (totalSize.y - labelSize.y) * 0.5f, 0f);

            var contentLocalPos = new Vector3(0, -(totalSize.y - contentHeight) * 0.5f, 0);

            // 更新可变尺寸物体的位置
            content.localPosition = contentLocalPos;
        }
    }
}