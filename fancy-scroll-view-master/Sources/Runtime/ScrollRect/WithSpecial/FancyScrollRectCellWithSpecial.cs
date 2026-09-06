using UnityEngine;

namespace FancyScrollView
{
    public abstract class FancyScrollRectCellWithSpecial<TItemData, TContext>
        : FancyCellWithSpecial<TItemData, TContext> where TContext : class, IFancyScrollRectContext, new()
    {
        public override void UpdatePosition(float position)
        {
            var (scrollSize, reuseMargin) = Context.CalculateScrollSize();

            var normalizedPosition =
                (Mathf.Lerp(0f, scrollSize, position) - reuseMargin) / (scrollSize - reuseMargin * 2f);

            var start = 0.5f * scrollSize;
            var end = -start;

            UpdatePosition(normalizedPosition, Mathf.Lerp(start, end, position));
        }

        /// <summary>
        /// このセルの位置を更新します.
        /// </summary>
        /// <param name="normalizedPosition">
        /// ビューポートの範囲で正規化されたスクロール位置.
        /// <see cref="FancyScrollRect{TItemData, TContext}.reuseCellMarginCount"/> の値に基づいて
        ///  <c>0.0</c> ~ <c>1.0</c> の範囲を超えた値が渡されることがあります.
        /// </param>
        /// <param name="localPosition">ローカル位置.</param>
        protected virtual void UpdatePosition(float normalizedPosition, float localPosition)
        {
            transform.localPosition = Context.ScrollDirection == ScrollDirection.Horizontal
                ? new Vector2(-localPosition, 0)
                : new Vector2(0, localPosition);
        }
    }
    
    public abstract class FancyScrollRectCellWithSpecial<TItemData> : 
        FancyScrollRectCellWithSpecial<TItemData, FancyScrollRectContext>
    {
        /// <inheritdoc/>
        public sealed override void SetContext(FancyScrollRectContext context) => base.SetContext(context);
    }
}