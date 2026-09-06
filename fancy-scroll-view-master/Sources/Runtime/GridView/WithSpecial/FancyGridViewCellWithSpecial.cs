using UnityEngine;

namespace FancyScrollView
{
    public abstract class FancyGridViewCellWithSpecial<TItemData, TContext> : 
        FancyScrollRectCellWithSpecial<TItemData, TContext>
        where TContext : class, IFancyGridViewContext, new()
    {
        /// <inheritdoc/>
        protected override void UpdatePosition(float normalizedPosition, float localPosition)
        {
            var cellSize = Context.GetCellSize();
            var spacing = Context.GetStartAxisSpacing();
            var groupCount = Context.GetGroupCount();

            var indexInGroup = Index % groupCount;
            var positionInGroup = (cellSize + spacing) * (indexInGroup - (groupCount - 1) * 0.5f);

            transform.localPosition = Context.ScrollDirection == ScrollDirection.Horizontal
                ? new Vector2(-localPosition, -positionInGroup)
                : new Vector2(positionInGroup, localPosition);
        }
    }
    
    public abstract class FancyGridViewCellWithSpecial<TItemData> : 
        FancyGridViewCellWithSpecial<TItemData, FancyGridViewContext>
    {
        /// <inheritdoc/>
        public sealed override void SetContext(FancyGridViewContext context) => base.SetContext(context);
    }
}