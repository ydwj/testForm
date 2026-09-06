/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: star
 * Created On: 2024-08-29
 * Description: Fancy Cell With Unfixed Size
 */

using UnityEngine;

namespace FancyScrollView
{
    /// <summary>
    /// 非固定尺寸FancyCell
    /// </summary>
    /// <typeparam name="TItemData"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public abstract class FancyCellWithUnfixedSize<TItemData, TContext> : FancyCell<TItemData, TContext> where TContext : class, new()
    {
        public virtual RectTransform rectTransform { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            if (rectTransform == null)
            {
                rectTransform = gameObject.GetComponent<RectTransform>();
            }
        }

        public override void UpdatePosition(float position) { }
    }

    /// <summary>
    /// <see cref="FancyScrollView{TItemData}"/> のセルを実装するための抽象基底クラス.
    /// </summary>
    /// <typeparam name="TItemData">アイテムのデータ型.</typeparam>
    /// <seealso cref="FancyCell{TItemData, TContext}"/>
    public abstract class FancyCellWithUnfixedSize<TItemData> : FancyCellWithUnfixedSize<TItemData, NullContext>
    {
        /// <inheritdoc/>
        public sealed override void SetContext(NullContext context) => base.SetContext(context);
    }

}