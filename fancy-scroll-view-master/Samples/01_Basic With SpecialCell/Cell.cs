/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.UI;

namespace FancyScrollView.Example01Special
{
    class Cell : FancyCellWithSpecial<ItemData>
    {
        [SerializeField] Animator animator = default;
        [SerializeField] Text message = default;
        [SerializeField] public Image image;

        static class AnimatorHash
        {
            public static readonly int scroll = Animator.StringToHash("scroll");
        }

        public override void UpdatePosition(float position)
        {
            currentPosition = position;

            if (animator.isActiveAndEnabled)
            {
                animator.Play(AnimatorHash.scroll, -1, position);
            }

            animator.speed = 0;
        }

        /// <summary>
        /// 当该cell为特殊物体时, 进行更新,
        /// 或者做你想对特殊物体做的其他逻辑
        /// </summary>
        protected override void UpdateSpecialCellContent()
        {
            var specialTrans = specialObj.transform;
            specialTrans.SetParent(transform.GetChild(0), false);
        }

        /// <summary>
        /// 当该cell为普通物体时,进行更新
        /// </summary>
        /// <param name="itemData"></param>
        protected override void UpdateNormalCellContent(ItemData itemData)
        {
            message.text = itemData.Message;
        }

        /// <summary>
        /// 隐藏普通物体
        /// </summary>
        protected override void HiddenNormalCellContent()
        {
            image.enabled = false;
            message.text = string.Empty;
        }

        /// <summary>
        /// 恢复普通物体
        /// </summary>
        protected override void ResumeNormalCellContent()
        {
            image.enabled = true;
        }

        // GameObject が非アクティブになると Animator がリセットされてしまうため
        // 現在位置を保持しておいて OnEnable のタイミングで現在位置を再設定します
        float currentPosition = 0;

        void OnEnable() => UpdatePosition(currentPosition);
    }
}
