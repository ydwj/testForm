using System.Linq;
using UnityEngine;

namespace FancyScrollView
{
    public abstract class FancyCellGroupWithSpecial<TItemData, TContext> : 
        FancyCellWithSpecial<TItemData[], TContext>
        where TContext : class, IFancyCellGroupContext, new()
    {
        /// <summary>
        /// このグループで表示するセルの配列.
        /// </summary>
        protected virtual FancyCellWithSpecial<TItemData, TContext>[] Cells { get; private set; }

        /// <summary>
        /// このグループで表示するセルの配列をインスタンス化します.
        /// </summary>
        /// <returns>このグループで表示するセルの配列.</returns>
        protected virtual FancyCellWithSpecial<TItemData, TContext>[] InstantiateCells()
        {
            return Enumerable.Range(0, Context.GetGroupCount())
                .Select(_ => Instantiate(Context.CellTemplate, transform))
                .Select(x => x.GetComponent<FancyCellWithSpecial<TItemData, TContext>>())
                .ToArray();
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            Cells = InstantiateCells();
            Debug.Assert(Cells.Length == Context.GetGroupCount());

            for (var i = 0; i < Cells.Length; i++)
            {
                Cells[i].SetContext(Context);
                Cells[i].SetSpecialController(specialCells);
                Cells[i].Initialize();
            }
        }

        /// <inheritdoc/>
        public override void UpdateContent(TItemData[] contents)
        {
            var firstCellIndex = Index * Context.GetGroupCount();

            for (var i = 0; i < Cells.Length; i++)
            {
                Cells[i].Index = i + firstCellIndex;
                Cells[i].SetVisible(i < contents.Length);

                if (Cells[i].IsVisible)
                {
                    Cells[i].UpdateContent(contents[i]);
                }
            }
        }

        /// <inheritdoc/>
        public override void UpdatePosition(float position)
        {
            for (var i = 0; i < Cells.Length; i++)
            {
                Cells[i].UpdatePosition(position);
            }
        }
    }
}
