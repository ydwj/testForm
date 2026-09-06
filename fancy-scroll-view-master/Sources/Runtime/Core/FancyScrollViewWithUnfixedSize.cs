/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: star
 * Created On: 2024-08-29
 * Description: Fancy Scroll View With Unfixed Size
 */

using System;
using System.Collections.Generic;
using System.Linq;
using EasingCore;
using UnityEditor;
using UnityEngine;

namespace FancyScrollView
{
    /// <summary>
    /// 非固定尺寸FancyScrollView
    /// </summary>
    /// <typeparam name="TItemData"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TFancyCell"></typeparam>
    public abstract class FancyScrollViewWithUnfixedSize<TItemData, TContext, TFancyCell> :
        MonoBehaviour where TContext : class, new() where TFancyCell : FancyCellWithUnfixedSize<TItemData, TContext>
    {
        #region Fields & Property

        protected TContext context { get; } = new TContext();

        #region Public & SerializeField

        public Scroller scroller { get; protected set; }

        /// <summary>
        /// 是否ScrollRect
        /// </summary>
        [SerializeField]
        private FancyViewMode m_FancyViewMode = FancyViewMode.ScrollView;

        /// <summary>
        /// 是否ScrollRect
        /// </summary>
        protected FancyViewMode fancyViewMode
        {
            get => m_FancyViewMode;
            set
            {
                m_FancyViewMode = value;

                InitLoading();

                UpdateUnfixedCells(currentScrollerPosition);
            }
        }

        [SerializeField, Delayed, InspectorName("scrollerDragSensitivityAdjuster")]
        private ScrollerDragSensitivityAdjuster m_ScrollerDragSensitivityAdjuster = new ScrollerDragSensitivityAdjuster
        {
            enable = true,
            speed = 1,
        };

        [SerializeField, Delayed, InspectorName("Cell Gap")]
        private float m_CellGap;

        [SerializeField, Delayed, InspectorName("Init Cell Count")]
        private int m_InitCellCount;

        [SerializeField, Delayed, InspectorName("Add Cell Count")]
        private int m_AdditionCellCount;

        /// <summary>
        /// cell间隔
        /// </summary>
        public float cellGap
        {
            get => m_CellGap;
            set
            {
                value = Mathf.Max(0, value);

                m_CellGap = value;

                UpdateContents(itemDataContainer, true);
            }
        }

        /// <summary>
        /// 初始化加载cell时的数量
        /// </summary>
        protected int initCellCount
        {
            get => m_InitCellCount;
            set
            {
                value = Mathf.Max(1, value);

                m_InitCellCount = value;
            }
        }

        /// <summary>
        /// 增量加载cell时的数量
        /// </summary>
        protected int additionCellCount
        {
            get => m_AdditionCellCount;
            set
            {
                value = Mathf.Max(1, value);

                m_AdditionCellCount = value;
            }
        }

        #endregion

        #region Abstract

        /// <summary>
        /// cell的预制体
        /// </summary>
        protected abstract GameObject cellPrefab { get; set; }

        /// <summary>
        /// cell的根节点
        /// </summary>
        protected abstract RectTransform cellRoot { get; set; }

        #endregion

        #region Pool

        /// <summary>
        /// cell复用池
        /// </summary>
        protected List<TFancyCell> pool { get; set; }

        #endregion

        #region Size & Position

        /// <summary>
        /// 非固定尺寸cell的总size
        /// </summary>
        protected float unfixedSizeTotalSize { get; set; }

        /// <summary>
        /// 每个cell的position
        /// </summary>
        protected List<float> cellPositions = new List<float>();

        /// <summary>
        /// Scroller当前的position
        /// </summary>
        protected float currentScrollerPosition { get; set; }

        /// <summary>
        /// 当前cell加载到的索引
        /// </summary>
        protected int currentCellLoadingIndex { get; set; }

        /// <summary>
        /// 所有cell的size
        /// </summary>
        protected List<float> cellSizes { get; set; }

        #endregion

        #region Scroll View

        /// <summary>
        /// ScrollView宽度
        /// </summary>
        protected float scrollViewRectWidth { get; set; }

        /// <summary>
        /// ScrollView高度
        /// </summary>
        protected float scrollViewRectHeight { get; set; }

        /// <summary>
        /// ScrollView前边界position
        /// </summary>
        protected float scrollViewRectPrePosition { get; set; }

        /// <summary>
        /// ScrollView后边界position
        /// </summary>
        protected float scrollViewRectPostPosition { get; set; }

        /// <summary>
        /// cell的总尺寸小于Rect
        /// </summary>
        protected bool isCellTotalSizeLessThanRect { get; set; }

        #endregion

        /// <summary>
        /// ItemData容器
        /// </summary>
        protected List<TItemData> itemDataContainer { get; set; }

        protected TFancyCell templateCell { get; set; }

        protected RectTransform templateRectTransform { get; set; }

        private bool m_Initialized { get; set; } = false;

        #endregion

        #region Initialize

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize()
        {
            pool = new List<TFancyCell>();

            InitScroller();

            m_Initialized = true;
        }

        protected virtual void InitScroller()
        {
            scroller = gameObject.GetComponent<Scroller>();

            if (scroller == null)
            {
                scroller = gameObject.AddComponent<Scroller>();
            }

            if (scroller.MovementType != MovementType.Clamped)
            {
                scroller.MovementType = MovementType.Clamped;

                Debug.LogWarning("FSV:  UnfixedSizeScrollView Scroller only supports Clamped MovementType temporarily.");
            }

            if (scroller.SnapEnabled)
            {
                scroller.SnapEnabled = false;

                Debug.LogWarning("FSV:  UnfixedSizeScrollView Scroller doesn't support Snap temporarily");
            }
        }

        #endregion

        #region Resize

        /// <summary>
        /// 重新计算ScrollView Rect
        /// </summary>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected virtual bool ResizeScrollViewRect(bool forceRefresh = false)
        {
            var rect = cellRoot.rect;

            var width = rect.width;
            var height = rect.height;

            var rectResized = false;

            if (!Mathf.Approximately(width, scrollViewRectWidth))
            {
                scrollViewRectWidth = width;
                rectResized = true;
            }

            if (!Mathf.Approximately(height, scrollViewRectHeight))
            {
                scrollViewRectHeight = height;
                rectResized = true;
            }

            if (!rectResized && !forceRefresh)
            {
                return false;
            }

            var totalPosition = scroller.GetTotalCount() - 1;

            var scrollViewLength = 0f;
            switch (scroller.ScrollDirection)
            {
                case ScrollDirection.Vertical:
                    scrollViewLength = height;
                    break;
                case ScrollDirection.Horizontal:
                    scrollViewLength = width;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(scroller.ScrollDirection)}: {scroller.ScrollDirection}");
            }

            // 计算ScrollView的前后边界的Position
            var realTotalSize = unfixedSizeTotalSize;
            var scrollViewPosition = scrollViewLength / realTotalSize * totalPosition;

            switch (fancyViewMode)
            {
                case FancyViewMode.ScrollView:
                    scrollViewRectPrePosition = -scrollViewPosition * 0.5f;
                    scrollViewRectPostPosition = scrollViewPosition * 0.5f;
                    break;
                case FancyViewMode.ScrollRect:
                    scrollViewRectPrePosition = 0;
                    scrollViewRectPostPosition = scrollViewPosition;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(fancyViewMode)}: {fancyViewMode}");
            }

            return true;
        }

        /// <summary>
        /// 重计算复用池
        /// </summary>
        protected virtual void ResizePool()
        {
            var maxCellCount = GetScrollViewMaxCellCount();

            var offset = maxCellCount - pool.Count;

            // 复用池扩容
            if (offset > 0)
            {
                IncreasePool(offset);
            }
            else
            {
                ReducePool(-offset);
            }
        }

        /// <summary>
        /// 增加复用池
        /// </summary>
        /// <param name="count"></param>
        /// <exception cref="MissingComponentException"></exception>
        protected virtual void IncreasePool(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var cellGameObj = Instantiate(cellPrefab, cellRoot);
                var cell = cellGameObj.GetComponent<TFancyCell>();

                if (cellGameObj == null || cell == null)
                {
                    throw new MissingComponentException(string.Format(
                        "FancyCell<{0}, {1}> component not found in {2}.",
                        typeof(TItemData).FullName, typeof(TContext).FullName, cellPrefab.name));
                }

                cellGameObj.name = $"UnfixedSize Cell {i}";

                AfterCellCreated(cell);

                cell.SetVisible(false);

                pool.Add(cell);
            }
        }

        /// <summary>
        /// 减少复用池
        /// </summary>
        /// <param name="count"></param>
        protected virtual void ReducePool(int count)
        {
            var target = pool.Count - 1 - count;

            for (var i = pool.Count - 1; i > target; i--)
            {
                var cell = pool[i];

                BeforeCellDestroyed(cell);

                Destroy(cell.gameObject);

                pool.RemoveAt(i);
            }
        }

        /// <summary>
        /// cell销毁前
        /// </summary>
        /// <param name="cell"></param>
        protected virtual void BeforeCellDestroyed(TFancyCell cell)
        {
        }

        /// <summary>
        /// cell创建后
        /// </summary>
        /// <param name="cell"></param>
        protected virtual void AfterCellCreated(TFancyCell cell)
        {
            cell.SetContext(context);
            cell.Initialize();
        }

        #endregion

        #region Loading

        /// <summary>
        /// 初始化加载cells
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected virtual void InitLoadingCells()
        {
            var itemCount = itemDataContainer.Count;

            // 如果cell数量小于等于initCellCount, 就加载cell数量个cells;
            // 如果cell数量大于initCellCount, 就加载initCellCount个cells.
            var initCount = itemCount <= initCellCount ? itemCount : initCellCount;

            GetCellSize(0, initCount);
            CalculateSizeAndPosition();

            scroller.SetTotalCount(initCount);
        }

        /// <summary>
        /// 增量加载cells
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected virtual void AddLoadingCells()
        {
            var itemCount = itemDataContainer.Count;

            // 相比当前已经加载的cell数量, 剩余的总的cell数量.
            var countOffset = itemCount - currentCellLoadingIndex;

            // 如果剩余的总的cell数量大于additionCellCount, 就加载additionCellCount个cells;
            // 如果剩余的总的cell数量小于additionCellCount, 就加载剩余数量个cells.
            var addCount = countOffset > additionCellCount ? additionCellCount : countOffset;

            var startIndex = currentCellLoadingIndex;

            var targetIndex = currentCellLoadingIndex + addCount;

            GetCellSize(startIndex, targetIndex);
            CalculateSizeAndPosition();

            var preCount = scroller.GetTotalCount();

            scroller.SetTotalCount(preCount + addCount);
        }

        /// <summary>
        /// 获取cell的size
        /// </summary>
        /// <param name="start">在itemDataContainer中的起始索引</param>
        /// <param name="end">在itemDataContainer中的结束索引</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected virtual void GetCellSize(int start, int end)
        {
            // 如果临时cell不存在, 则生成它
            if (templateCell == null)
            {
                var tmpCellObj = Instantiate(cellPrefab, cellRoot);

                templateRectTransform = tmpCellObj.GetComponent<RectTransform>();

                templateCell = tmpCellObj.GetComponent<TFancyCell>();

                templateCell.Initialize();
            }

            templateCell.SetVisible(true);

            for (var i = start; i < end; i++)
            {
                templateCell.UpdateContent(itemDataContainer[i]);

                var rect = templateRectTransform.sizeDelta;

                float size;
                switch (scroller.ScrollDirection)
                {
                    case ScrollDirection.Vertical:
                        size = rect.y;
                        break;
                    case ScrollDirection.Horizontal:
                        size = rect.x;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"{nameof(scroller.ScrollDirection)}: {scroller.ScrollDirection}");
                }

                // 获取尺寸
                cellSizes.Add(size);

                currentCellLoadingIndex++;
            }

            templateCell.SetVisible(false);
        }

        /// <summary>
        /// 计算size和position
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected virtual void CalculateSizeAndPosition()
        {
            var unfixedSizeCompletedTotalSize = 0f;
            unfixedSizeTotalSize = 0f;

            cellPositions.Clear();

            // 计算总尺寸
            var prePosition = 0f;
            for (var i = 0; i < cellSizes.Count; i++)
            {
                var size = cellSizes[i];

                var cellHalfLength = i > 0 ? size * 0.5f : 0f;
                var cellPos = prePosition + cellHalfLength;

                // 计算每个cell的position
                cellPositions.Add(cellPos);

                unfixedSizeCompletedTotalSize += size;

                // 最后一个cell不加上cellGap
                if (i != itemDataContainer.Count - 1)
                {
                    unfixedSizeCompletedTotalSize += cellGap;
                }

                var cellSizePos = i > 0 ? size : size * 0.5f;
                prePosition += (cellSizePos + cellGap);
            }

            unfixedSizeTotalSize = TotalSizePostProcessing(unfixedSizeCompletedTotalSize);
        }

        protected virtual float TotalSizePostProcessing(float unfixedSizeCompletedTotalSize)
        {
            isCellTotalSizeLessThanRect = false;

            var totalSize = 0f;

            switch (fancyViewMode)
            {
                case FancyViewMode.ScrollView:
                    totalSize = unfixedSizeCompletedTotalSize;

                    // 总size减去首尾cell的size的一半
                    var firstHalfSize = cellSizes[0] * 0.5f;
                    var endHalfSize = cellSizes[cellSizes.Count - 1] * 0.5f;

                    totalSize -= (firstHalfSize + endHalfSize);

                    break;
                case FancyViewMode.ScrollRect:
                    // 总size减去总size和ScrollView长度的差值
                    switch (scroller.ScrollDirection)
                    {
                        case ScrollDirection.Vertical:
                            totalSize = unfixedSizeCompletedTotalSize - cellRoot.rect.height;
                            if (totalSize < 0)
                            {
                                totalSize = unfixedSizeCompletedTotalSize;
                                isCellTotalSizeLessThanRect = true;
                            }

                            break;
                        case ScrollDirection.Horizontal:
                            totalSize = unfixedSizeCompletedTotalSize - cellRoot.rect.width;
                            if (totalSize < 0)
                            {
                                totalSize = unfixedSizeCompletedTotalSize;
                                isCellTotalSizeLessThanRect = true;
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"{nameof(scroller.ScrollDirection)}: {scroller.ScrollDirection}");
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return totalSize;
        }

        #endregion

        #region Udate

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Editor下, Inspector面板调整序列化字段时, 进行刷新操作
            OnSerializedFieldsChanged();
        }

        private void OnSerializedFieldsChanged()
        {
            if (!EditorApplication.isPlaying || !m_Initialized)
            {
                return;
            }

            UpdateContents(itemDataContainer, true);
        }

#endif

        #region Load

        protected virtual void InitLoading()
        {
            InitLoadingCells();
            AfterLoading();
        }

        protected virtual void AddLoading()
        {
            AddLoadingCells();
            AfterLoading();
        }

        protected virtual void AfterLoading()
        {
            ResizeScrollViewRect(true);
            AdjustScrollerDragSensitivity();
            ResizePool();
        }

        #endregion

        /// <summary>
        /// 更新数据内容
        /// </summary>
        /// <param name="itemsSource"></param>
        /// <param name="holdingPosition">是否保持位置</param>
        protected virtual void UpdateContents(IList<TItemData> itemsSource, bool holdingPosition)
        {
            if (itemsSource == null || itemsSource.Count < 1)
            {
                return;
            }

            var oldItemCount = itemDataContainer != null ? itemDataContainer.Count : 0;
            var oldPosition = currentScrollerPosition;
            var oldTotalPosition = scroller.GetTotalCount() - 1;
            var oldTotalSize = unfixedSizeTotalSize;

            itemDataContainer = itemsSource.ToList();

            cellSizes = new List<float>(itemsSource.Count);
            cellPositions = new List<float>(itemDataContainer.Count);

            currentCellLoadingIndex = 0;

            InitLoading();

            // 非保持旧位置, 则直接将位置置0
            var newPosition = holdingPosition ? HoldingOldPosition(oldPosition, oldTotalPosition, oldTotalSize, oldItemCount) : 0f;

            scroller.Position = newPosition;
        }

        /// <summary>
        /// 调整Scroller的DragSensitivity
        /// </summary>
        protected virtual void AdjustScrollerDragSensitivity()
        {
            if (!m_ScrollerDragSensitivityAdjuster.enable)
            {
                return;
            }

            var totalPosition = Mathf.Max(1, scroller.GetTotalCount() - 1);
            var scrollViewSize = scroller.ScrollDirection == ScrollDirection.Vertical ? scrollViewRectHeight : scrollViewRectWidth;
            var totalSize = unfixedSizeTotalSize;

            var speed = m_ScrollerDragSensitivityAdjuster.speed;

            var sensitivity = (totalPosition / totalSize) * scrollViewSize * speed;

            scroller.dragSensitivity = sensitivity;
        }

        /// <summary>
        /// 保持旧位置
        /// </summary>
        /// <param name="oldPosition"></param>
        /// <param name="oldTotalPosition"></param>
        /// <param name="oldTotalSize"></param>
        /// <param name="preItemCount"></param>
        /// <returns></returns>
        protected float HoldingOldPosition(float oldPosition, float oldTotalPosition, float oldTotalSize, int preItemCount)
        {
            var newPosition = 0f;

            var newTotalPosition = scroller.GetTotalCount() - 1;
            var newTotalSize = unfixedSizeTotalSize;
            var newItemCount = itemDataContainer.Count;

            var itemCountOffset = newItemCount - preItemCount;
            if (itemCountOffset > 0)
            {
                newPosition = CalculateHoldingNewPosition(oldPosition, oldTotalPosition, oldTotalSize, newTotalPosition, newTotalSize);
            }
            else if (itemCountOffset < 0)
            {
                newPosition = oldPosition > newTotalPosition ? newTotalPosition : CalculateHoldingNewPosition(oldPosition, oldTotalPosition, oldTotalSize, newTotalPosition, newTotalSize);
            }
            else
            {
                newPosition = oldPosition;
            }

            return newPosition;
        }

        /// <summary>
        /// 计算新position
        /// </summary>
        /// <param name="oldPosition"></param>
        /// <param name="oldTotalPosition"></param>
        /// <param name="oldTotalSize"></param>
        /// <param name="newTotalPosition"></param>
        /// <param name="newTotalSize"></param>
        /// <returns></returns>
        private float CalculateHoldingNewPosition(
            float oldPosition, float oldTotalPosition, float oldTotalSize,
            float newTotalPosition, float newTotalSize)
        {
            // 保持位置情况下, 有等式成立:
            // (oldPosition / oldTotalPosition) * oldTotalSize = (newPosition / newTotalPosition) * newTotalSize
            // 变换得到以下算式:
            return (oldPosition * oldTotalSize * newTotalPosition) / (oldTotalPosition * newTotalSize);
        }

        protected virtual void UpdatePosition(float position) => UpdatePosition(position, false);

        protected virtual void UpdatePosition(float position, bool forceRefresh)
        {
            UpdateUnfixedCells(position);
        }

        /// <summary>
        /// 更新Cells
        /// </summary>
        /// <param name="scrollerPosition"></param>
        protected virtual void UpdateUnfixedCells(float scrollerPosition)
        {
            var totalPosition = Mathf.Max(1, scroller.GetTotalCount() - 1);

            scrollerPosition = Mathf.Clamp(scrollerPosition, 0f, totalPosition);

            // ScrollView模式下, itemCount < 2 时不进行滚动
            if (fancyViewMode == FancyViewMode.ScrollView && itemDataContainer.Count < 2)
            {
                scrollerPosition = 0;
            }

            // cell总尺寸小于scrollView
            if (isCellTotalSizeLessThanRect)
            {
                scrollerPosition = 0;
            }

            // 当scrollerPosition到达最大, 并且仍有数据未加载
            if (Mathf.Approximately(scrollerPosition, totalPosition) && itemDataContainer.Count > scroller.GetTotalCount())
            {
                // 增量加载
                AddLoading();

                return;
            }

            currentScrollerPosition = scrollerPosition;

            var realPosition = scrollerPosition;

            var realTotalSize = unfixedSizeTotalSize;

            var size = 0f;

            // 将position加到scrollView的前后position, 逻辑上模拟成scrollView在滚动, 而cells不滚动
            var svPrePos = scrollViewRectPrePosition + realPosition;
            var svPostPos = scrollViewRectPostPosition + realPosition;

            var scrollDirection = scroller.ScrollDirection;

            var poolIndex = 0;
            for (var i = 0; i < currentCellLoadingIndex; i++)
            {
                var cellSize = cellSizes[i];

                var itemSize = i == 0 && fancyViewMode == FancyViewMode.ScrollView ? cellSize * 0.5f : cellSize;

                // 计算cell的前后position
                var prePosition = size / realTotalSize * totalPosition;
                var postPosition = (size + itemSize) / realTotalSize * totalPosition;

                if (poolIndex > pool.Count - 1)
                {
                    ResizeScrollViewRect(true);
                    ResizePool();

                    return;
                }

                // 是否在scrollView中
                var isInScrollView =
                    CheckIsCellPositionInScrollView(prePosition, postPosition, svPrePos, svPostPos);

                // 如果在scrollView中, 则绘制该cell
                if (isInScrollView)
                {
                    var poolCell = pool[poolIndex];

                    poolCell.SetVisible(true);

                    var pos = GetUnfixedSizeCellRealPos(i, scrollerPosition);

                    UpdateCellRealPosition(poolCell, pos, scrollDirection);

                    poolCell.UpdateContent(itemDataContainer[i]);

                    poolIndex++;
                }

                size += (itemSize + cellGap);
            }

            // 非使用到的pool的item隐藏
            for (var i = 0; i < pool.Count; i++)
            {
                if (i >= poolIndex)
                {
                    pool[i].SetVisible(false);
                }
            }
        }

        /// <summary>
        /// 更新cell的真实位置
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="position"></param>
        /// <param name="scrollDirection"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected virtual void UpdateCellRealPosition(TFancyCell cell, float position, ScrollDirection scrollDirection)
        {
            Vector3 localPosition;
            switch (scrollDirection)
            {
                case ScrollDirection.Vertical:
                    localPosition = new Vector3()
                    {
                        z = 0,
                        y = position,
                        x = 0
                    };

                    break;
                case ScrollDirection.Horizontal:
                    localPosition = new Vector3()
                    {
                        z = 0,
                        y = 0,
                        x = position
                    };

                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(scrollDirection)}: {scrollDirection}");
            }

            cell.rectTransform.localPosition = localPosition;
        }

        #endregion

        #region Scroll

        /// <summary>
        /// 滚动到指定的索引的cell
        /// </summary>
        /// <param name="cellIndex">cell的索引</param>
        /// <param name="duration">过程时间</param>
        /// <param name="viewPortOffset">相对ScrollViewPort内的偏移, 范围在[0, 1], 非IsScrollRect下不生效</param>
        /// <param name="cellOffset">相对cell内部的偏移, 范围在[0, 1], 非IsScrollRect下不生效</param>
        /// <param name="easing">缓动函数</param>
        /// <param name="onStart">滚动开始</param>
        /// <param name="onEnd">滚动结束</param>
        public virtual void ScrollToCell(
            int cellIndex, float duration,
            float viewPortOffset = 0.5f, float cellOffset = 0.5f,
            Ease easing = Ease.Linear, Action onStart = null, Action onEnd = null)
        {
            var position = CellIndexToPosition(cellIndex, viewPortOffset, cellOffset);

            scroller.ScrollTo(position, duration, easing, onStart, onEnd);
        }

        /// <summary>
        /// Cell索引 to Position
        /// </summary>
        /// <param name="cellIndex">cell的索引</param>
        /// <param name="viewPortOffset">相对ScrollViewPort内的偏移, 范围在[0, 1], 非IsScrollRect下不生效</param>
        /// <param name="cellOffset">相对cell内部的偏移, 范围在[0, 1], 非IsScrollRect下不生效</param>
        /// <returns></returns>
        protected virtual float CellIndexToPosition(int cellIndex, float viewPortOffset = 0.5f, float cellOffset = 0f)
        {
            if ((cellIndex < 0 || cellIndex > itemDataContainer.Count - 1) ||
                (fancyViewMode == FancyViewMode.ScrollView && itemDataContainer.Count == 1))
            {
                return currentScrollerPosition;
            }

            // 如果目标index超过当前已经加载到的index
            while (cellIndex > currentCellLoadingIndex && cellIndex < itemDataContainer.Count - 1)
            {
                // 增量加载
                AddLoading();
            }

            // 取0到1之间
            viewPortOffset = Mathf.Clamp(viewPortOffset, 0f, 1f);
            cellOffset = Mathf.Clamp(cellOffset, 0f, 1f);

            // 如果是ScrollView模式
            if (fancyViewMode == FancyViewMode.ScrollView)
            {
                // 偏移统一修改为0
                viewPortOffset = 0f;
                cellOffset = 0f;
            }

            // viewPort偏移值
            var viewPortOffsetValue = 0f;
            switch (scroller.ScrollDirection)
            {
                case ScrollDirection.Vertical:
                    viewPortOffsetValue = -viewPortOffset * scrollViewRectHeight;
                    break;
                case ScrollDirection.Horizontal:
                    viewPortOffsetValue = -viewPortOffset * scrollViewRectWidth;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(scroller.ScrollDirection)}: {scroller.ScrollDirection}");
            }

            var position = 0f;

            var totalLogicSize = 0f;

            for (var i = 0; i < cellSizes.Count; i++)
            {
                var cellSize = cellSizes[i];

                // 当ScrollView模式时,
                // 需要添加前置的Size
                if (fancyViewMode == FancyViewMode.ScrollView && i > 0)
                {
                    var preLogicSize = cellSize * 0.5f;

                    totalLogicSize += preLogicSize;
                }

                // 找到了指定的index的cell
                if (cellIndex == i)
                {
                    // cell偏移值
                    var cellOffsetValue = cellOffset * cellSize;

                    // 应用偏移值
                    totalLogicSize += cellOffsetValue;
                    totalLogicSize += viewPortOffsetValue;

                    position = totalLogicSize / unfixedSizeTotalSize;

                    break;
                }

                // 后置的size
                var postLogicSize = 0f;

                switch (fancyViewMode)
                {
                    case FancyViewMode.ScrollView:
                        postLogicSize = cellSize * 0.5f;
                        break;
                    case FancyViewMode.ScrollRect:
                        postLogicSize = cellSize;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"{nameof(fancyViewMode)}: {fancyViewMode}");
                }

                postLogicSize += cellGap;

                // 添加后置size
                totalLogicSize += postLogicSize;
            }

            position = Mathf.Clamp(position, 0, 1);

            var totalPosition = Mathf.Max(1f, scroller.GetTotalCount() - 1);
            position *= totalPosition;

            return position;
        }

        #endregion

        #region Tool Method

        /// <summary>
        /// 获取ScrollView中能显示的最大cell数量
        /// </summary>
        /// <returns></returns>
        protected virtual int GetScrollViewMaxCellCount()
        {
            var scrollViewLength = 0f;
            switch (scroller.ScrollDirection)
            {
                case ScrollDirection.Vertical:
                    scrollViewLength = scrollViewRectHeight;
                    break;
                case ScrollDirection.Horizontal:
                    scrollViewLength = scrollViewRectWidth;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(scroller.ScrollDirection)}: {scroller.ScrollDirection}");
            }

            var maxCount = 1;

            // 是否找到最大maxCount
            var find = false;

            for (var i = 0; i < cellSizes.Count; i++)
            {
                if (i == cellSizes.Count - 1)
                {
                    continue;
                }

                var subWidth = 0f;

                // 从i为起点出发, 是否有足够的cell的长度大于ScrollView Rect
                for (var j = i + 1; j < cellSizes.Count; j++)
                {
                    var subCount = j - i + 1;

                    if (subWidth > scrollViewLength && subCount > maxCount)
                    {
                        maxCount = subCount - 1;

                        find = true;

                        break;
                    }

                    var postCell = cellSizes[j];

                    subWidth += postCell;
                }
            }

            if (!find)
            {
                maxCount = scroller.GetTotalCount();
            }

            return maxCount;
        }

        /// <summary>
        /// 检查cell的位置是否在ScrollView的显示区域内部
        /// </summary>
        /// <param name="cellPrePosition">cell的前位置</param>
        /// <param name="cellPostPosition">cell的后位置</param>
        /// <param name="svPrePos">scrollView的前位置</param>
        /// <param name="svPostPos">scrollView的后位置</param>
        /// <returns></returns>
        protected virtual bool CheckIsCellPositionInScrollView(
            float cellPrePosition, float cellPostPosition,
            float svPrePos, float svPostPos)
        {
            //  cell超过sv的前边界
            if (cellPostPosition < svPrePos)
            {
                return false;
            }

            // cell超过sv的后边界
            if (cellPrePosition > svPostPos)
            {
                return false;
            }

            // 其他情况均表示在sv边界内部
            return true;
        }

        /// <summary>
        /// 获取非固定尺寸cell的真实位置
        /// </summary>
        /// <param name="index"></param>
        /// <param name="scrollerPosition"></param>
        /// <returns></returns>
        protected virtual float GetUnfixedSizeCellRealPos(int index, float scrollerPosition)
        {
            // 计算第0个cell的位置
            var firstPositionOffset = 0f;

            var count = Mathf.Max(1, scroller.GetTotalCount() - 1);

            var percent = scrollerPosition / count;
            var realTotalSize = isCellTotalSizeLessThanRect ? 0 : unfixedSizeTotalSize;

            firstPositionOffset = percent * realTotalSize;

            if (firstPositionOffset > 0)
            {
                firstPositionOffset = -firstPositionOffset;
            }

            if (fancyViewMode == FancyViewMode.ScrollRect)
            {
                var firstCellHalfSize = cellSizes[0] * 0.5f;

                var scrollViewRectLength = 0f;
                switch (scroller.ScrollDirection)
                {
                    case ScrollDirection.Vertical:
                        scrollViewRectLength = scrollViewRectHeight;
                        break;
                    case ScrollDirection.Horizontal:
                        scrollViewRectLength = scrollViewRectWidth;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"{nameof(scroller.ScrollDirection)}: {scroller.ScrollDirection}");
                }

                var scrollViewRectHalfLength = scrollViewRectLength * 0.5f;

                firstPositionOffset += (firstCellHalfSize - scrollViewRectHalfLength);
            }

            var pos = cellPositions[index] + firstPositionOffset;

            var realPos = 0f;
            switch (scroller.ScrollDirection)
            {
                case ScrollDirection.Vertical:
                    realPos = -pos;
                    break;
                case ScrollDirection.Horizontal:
                    realPos = pos;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(scroller.ScrollDirection)}: {scroller.ScrollDirection}");
            }

            return realPos;
        }

        #endregion
    }

    public abstract class FancyScrollViewWithUnfixedSize<TItemData> : FancyScrollViewWithUnfixedSize<TItemData, NullContext, FancyCellWithUnfixedSize<TItemData>>
    {
    }

    public abstract class FancyScrollViewWithUnfixedSize<TItemData, TContent> :
        FancyScrollViewWithUnfixedSize<TItemData, TContent, FancyCellWithUnfixedSize<TItemData, TContent>> where TContent : class, new()
    {
    }
}