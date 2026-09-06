/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.UI;

namespace FancyScrollView.Example08WithSpecial
{
    class Cell : FancyGridViewCellWithSpecial<ItemData, Context>
    {
        [SerializeField] Text message = default;
        [SerializeField] Image image = default;
        [SerializeField] Button button = default;

        public override void Initialize()
        {
            button.onClick.AddListener(() => Context.OnCellClicked?.Invoke(Index));
        }

        protected override void UpdatePosition(float normalizedPosition, float localPosition)
        {
            base.UpdatePosition(normalizedPosition, localPosition);

            var wave = Mathf.Sin(normalizedPosition * Mathf.PI * 2) * 65;
            transform.localPosition += Vector3.right * wave;
        }

        protected override void UpdateSpecialCellContent()
        {
            
        }

        protected override void UpdateNormalCellContent(ItemData itemData)
        {
            message.text = itemData.Index.ToString();

            var selected = Context.SelectedIndex == Index;
            image.color = selected
                ? new Color32(0, 255, 255, 100)
                : new Color32(255, 255, 255, 77);
        }

        protected override void HiddenNormalCellContent()
        {
            message.text = string.Empty;
            image.enabled = false;
            button.enabled = false;
        }

        protected override void ResumeNormalCellContent()
        {
            image.enabled = true;
            button.enabled = true;
        }
    }
}
