using UnityEngine;

namespace FancyScrollView
{
    public abstract class FancyCellWithSpecial<TItemData, TContext> : FancyCell<TItemData,TContext> where TContext : class, new()
    {
        protected GameObject specialObj = null;

        protected bool isSpecial { get; set; } = false;

        protected bool prevIsSpecial { get; set; } = false;

        protected int preSpecialIndex { get; set; }
        
        protected ISpecialCells specialCells;
        
        public void SetSpecialController(ISpecialCells cells)
        {
            specialCells = cells;
        }

        /// <summary>
        /// アイテムデータに基づいてこのセルの表示内容を更新します.
        /// </summary>
        /// <param name="itemData">アイテムデータ.</param>
        public override void UpdateContent(TItemData itemData)
        {
            CheckPreCellIsSpecial();

            SetIsCurrentCellSpecial();

            switch (isSpecial)
            {
                case true:
                    WhileIsSpecialCell();
                    break;
                case false:
                    WhileIsNormalCell(itemData);
                    break;
            }
        }

        protected void CheckPreCellIsSpecial()
        {
            if (prevIsSpecial)
            {
                specialCells?.PushSpecialCellObject((uint)preSpecialIndex);
                prevIsSpecial = false;
            }
        }

        protected void SetIsCurrentCellSpecial()
        {
            if (specialCells == null ||
                specialCells.GetSpecialCellIndexAndObject() == null)
            {
                isSpecial = false;
                prevIsSpecial = false;
                preSpecialIndex = -1;
                return;
            }

            var specialGameObjects = specialCells.GetSpecialCellIndexAndObject();
            isSpecial = specialGameObjects.ContainsKey((uint)Index);
            
            prevIsSpecial = true;
            preSpecialIndex = isSpecial ? Index : -1;
        }


        protected void WhileIsSpecialCell()
        {
            HiddenNormalCellContent();
            GetSpecialCellObj();
            UpdateSpecialCellContent();
        }

        protected void GetSpecialCellObj()
        {
            var specialCellObject = specialCells.GetSpecialCellObject((uint)Index);

            if (specialCellObject == null)
            {
                return;
            }
            
            specialObj = specialCellObject;
            var specialTrans = specialObj.transform; 
            specialTrans.SetParent(transform,false);
            specialTrans.localPosition = Vector3.zero;
            specialTrans.localRotation = Quaternion.identity;
            specialCellObject.SetActive(true);
        }

        protected void ReturnSpecialCellObj()
        {
            specialObj = null;
        }

        protected void WhileIsNormalCell(TItemData itemData)
        {
            ReturnSpecialCellObj();
            ResumeNormalCellContent();
            UpdateNormalCellContent(itemData);
        }

        protected virtual void UpdateSpecialCellContent() { }
        
        protected virtual void UpdateNormalCellContent(TItemData itemData) { }

        protected abstract void HiddenNormalCellContent();

        protected abstract void ResumeNormalCellContent();
    }
    
    /// <summary>
    /// <see cref="FancyScrollView{TItemData}"/> のセルを実装するための抽象基底クラス.
    /// </summary>
    /// <typeparam name="TItemData">アイテムのデータ型.</typeparam>
    /// <seealso cref="FancyCell{TItemData, TContext}"/>
    public abstract class FancyCellWithSpecial<TItemData> : FancyCellWithSpecial<TItemData, NullContext>
    {
        /// <inheritdoc/>
        public sealed override void SetContext(NullContext context) => base.SetContext(context);
    }
}