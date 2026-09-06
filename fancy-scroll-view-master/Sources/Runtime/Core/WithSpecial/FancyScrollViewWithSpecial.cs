using UnityEngine;

namespace FancyScrollView
{
    public abstract class FancyScrollViewWithSpecial<TItemData, TContext> : 
        FancyScrollView<TItemData, TContext, FancyCellWithSpecial<TItemData,TContext>> where TContext : class, new()
    {
        protected ISpecialCells specialCells;

        /// <summary>
        /// 初期化を行います.
        /// </summary>
        /// <remarks>
        /// 最初にセルが生成される直前に呼び出されます.
        /// </remarks>
        protected override void Initialize()
        {
            base.Initialize();
            TryGetComponent(out specialCells);
            specialCells?.Initialize();
        }

        protected override void ResizePool(float firstPosition)
        {
            Debug.Assert(CellPrefab != null);
            Debug.Assert(cellContainer != null);

            var addCount = Mathf.CeilToInt((1f - firstPosition) / cellInterval) - pool.Count;
            for (var i = 0; i < addCount; i++)
            {
                var cell = Instantiate(CellPrefab, cellContainer).GetComponent<FancyCellWithSpecial<TItemData, TContext>>();
                if (cell == null)
                {
                    
                    throw new MissingComponentException(string.Format(
                        "FancyCell<{0}, {1}> component not found in {2}.",
                        typeof(TItemData).FullName, typeof(TContext).FullName, CellPrefab.name));
                }

                cell.SetContext(Context);
                cell.SetSpecialController(specialCells);
                cell.Initialize();
                cell.SetVisible(false);
                pool.Add(cell);
            }
        }
    }
    
    public abstract class FancyScrollViewWithSpecial<TItemData> : FancyScrollViewWithSpecial<TItemData, NullContext> { }
}
