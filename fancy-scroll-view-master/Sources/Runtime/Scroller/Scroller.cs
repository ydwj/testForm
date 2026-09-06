/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EasingCore;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace FancyScrollView
{
    /// <summary>
    /// スクロール位置の制御を行うコンポーネント.
    /// </summary>
    public class Scroller : UIBehaviour, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler
    {
        [SerializeField] RectTransform viewport = default;

        /// <summary>
        /// ビューポートのサイズ.
        /// </summary>
        public float ViewportSize => scrollDirection == ScrollDirection.Horizontal
            ? viewport.rect.size.x
            : viewport.rect.size.y;

        [SerializeField] ScrollDirection scrollDirection = ScrollDirection.Vertical;

        /// <summary>
        /// スクロール方向.
        /// </summary>
        public ScrollDirection ScrollDirection => scrollDirection;

        [SerializeField] MovementType movementType = MovementType.Elastic;

        /// <summary>
        /// コンテンツがスクロール範囲を越えて移動するときに使用する挙動.
        /// </summary>
        public MovementType MovementType
        {
            get => movementType;
            set => movementType = value;
        }

        [SerializeField] float elasticity = 0.1f;

        /// <summary>
        /// コンテンツがスクロール範囲を越えて移動するときに使用する弾力性の量.
        /// </summary>
        public float Elasticity
        {
            get => elasticity;
            set => elasticity = value;
        }

        [SerializeField] private float m_DragSensitivity = 1f;

        /// <summary>
        /// <see cref="ViewportSize"/> の端から端まで Drag したときのスクロール位置の変化量.
        /// </summary>
        public float dragSensitivity
        {
            get => m_DragSensitivity;
            set => m_DragSensitivity = value;
        }

        [SerializeField] private float m_MouseWheelSensitivity = 1f;

        /// <summary>
        /// 鼠标滚轮强度
        /// </summary>
        public float mouseWheelSensitivity
        {
            get => m_MouseWheelSensitivity;
            set => m_MouseWheelSensitivity = value;
        }

        [SerializeField] bool inertia = true;

        /// <summary>
        /// 慣性を使用するかどうか. <c>true</c> を指定すると慣性が有効に, <c>false</c> を指定すると慣性が無効になります.
        /// </summary>
        public bool Inertia
        {
            get => inertia;
            set => inertia = value;
        }

        [SerializeField] float decelerationRate = 0.03f;

        /// <summary>
        /// スクロールの減速率. <see cref="Inertia"/> が <c>true</c> の場合のみ有効です.
        /// </summary>
        public float DecelerationRate
        {
            get => decelerationRate;
            set => decelerationRate = value;
        }

        [SerializeField] Snap snap = new Snap {
            Enable = true,
            VelocityThreshold = 0.5f,
            Duration = 0.3f,
            Easing = Ease.InOutCubic
        };

        /// <summary>
        /// <c>true</c> ならスナップし, <c>false</c>ならスナップしません.
        /// </summary>
        /// <remarks>
        /// スナップを有効にすると, 慣性でスクロールが止まる直前に最寄りのセルへ移動します.
        /// </remarks>
        public bool SnapEnabled
        {
            get => snap.Enable;
            set => snap.Enable = value;
        }

        [SerializeField] bool draggable = true;

        /// <summary>
        /// Drag 入力を受付けるかどうか.
        /// </summary>
        public bool Draggable
        {
            get => draggable;
            set => draggable = value;
        }

        [SerializeField] private FancyScrollbar m_FancyScrollbar = default;

        /// <summary>
        /// スクロールバーのオブジェクト.
        /// </summary>
        public FancyScrollbar fancyScrollbar => m_FancyScrollbar;

        /// <summary>
        /// 現在のスクロール位置.
        /// </summary>
        /// <value></value>
        public float Position
        {
            get => currentPosition;
            set
            {
                autoScrollState.Reset();
                velocity = 0f;
                dragging = false;

                UpdatePosition(value);
            }
        }

        readonly AutoScrollState autoScrollState = new AutoScrollState();

        Action<float> onValueChanged;
        Action<int> onSelectionChanged;

        private Action onAutoScrollStart;

        private Action onAutoScrollEnd;

        Vector2 beginDragPointerPosition;
        float scrollStartPosition;
        float prevPosition;
        float currentPosition;

        int totalCount;

        bool hold;
        bool scrolling;
        bool dragging;
        float velocity;

        private bool m_IsLoop;

        [Serializable]
        class Snap
        {
            public bool Enable;
            public float VelocityThreshold;
            public float Duration;
            public Ease Easing;
        }

        static readonly EasingFunction DefaultEasingFunction = Easing.Get(Ease.OutCubic);

        class AutoScrollState
        {
            public bool isStarted;

            public bool Enable;
            public bool Elastic;
            public float Duration;
            public EasingFunction EasingFunction;
            public float StartTime;
            public float EndPosition;

            public Action OnStart;

            public Action OnComplete;

            public void Reset()
            {
                Enable = false;
                Elastic = false;
                Duration = 0f;
                StartTime = 0f;
                EasingFunction = DefaultEasingFunction;
                EndPosition = 0f;
                OnComplete = null;
            }

            public void Complete()
            {
                OnComplete?.Invoke();
                Reset();
            }

            public void Start()
            {
                OnStart?.Invoke();
                isStarted = false;
            }
        }
        
        /// <summary>
        /// Scroller获得用户按下Scrollbar的Position
        /// Position 介于 [0,1] 之间
        /// </summary>
        public UnityEvent<float> onScrollerGetPointerDownPosition = new Scrollbar.ScrollEvent();

        protected override void Start()
        {
            base.Start();

            if (m_FancyScrollbar)
            {
                m_FancyScrollbar.onValueChanged.AddListener(x => UpdatePosition(x * (totalCount - 1f), false));
                m_FancyScrollbar.onPointerDown.AddListener(OnScrollbarPointerDown);
            }
        }

        /// <summary>
        /// スクロール位置が変化したときのコールバックを設定します.
        /// </summary>
        /// <param name="callback">スクロール位置が変化したときのコールバック.</param>
        public void OnValueChanged(Action<float> callback) => onValueChanged = callback;

        /// <summary>
        /// 選択位置が変化したときのコールバックを設定します.
        /// </summary>
        /// <param name="callback">選択位置が変化したときのコールバック.</param>
        public void OnSelectionChanged(Action<int> callback) => onSelectionChanged = callback;

        /// <summary>
        /// 当 滚动开始
        /// </summary>
        /// <param name="callback"></param>
        public void OnAutoScrollStart(Action callback) => onAutoScrollStart = callback;

        /// <summary>
        /// 当 滚动结束
        /// </summary>
        /// <param name="callback"></param>
        public void OnAutoScrollEnd(Action callback) => onAutoScrollEnd = callback;

        /// <summary>
        /// アイテムの総数を設定します.
        /// </summary>
        /// <remarks>
        /// <paramref name="totalCount"/> を元に最大スクロール位置を計算します.
        /// </remarks>
        /// <param name="totalCount">アイテムの総数.</param>
        public void SetTotalCount(int totalCount) => this.totalCount = totalCount;

        /// <summary>
        /// 获取TotalCount
        /// </summary>
        /// <returns></returns>
        public int GetTotalCount() => this.totalCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isLoop"></param>
        public void SetIsLoop(bool isLoop) => this.m_IsLoop = isLoop;
        
        /// <summary>
        /// 指定した位置まで移動します.
        /// </summary>
        /// <param name="position">スクロール位置. <c>0f</c> ~ <c>totalCount - 1f</c> の範囲.</param>
        /// <param name="duration">移動にかける秒数.</param>
        /// <param name="onComplete">移動が完了した際に呼び出されるコールバック.</param>
        public void ScrollTo(float position, float duration, Action onComplete = null) => ScrollTo(position, duration, Ease.OutCubic, onComplete);

        /// <summary>
        /// 指定した位置まで移動します.
        /// </summary>
        /// <param name="position">スクロール位置. <c>0f</c> ~ <c>totalCount - 1f</c> の範囲.</param>
        /// <param name="duration">移動にかける秒数.</param>
        /// <param name="easing">移動に使用するイージング.</param>
        /// <param name="onComplete">移動が完了した際に呼び出されるコールバック.</param>
        public void ScrollTo(float position, float duration, Ease easing, Action onComplete = null) => ScrollTo(position, duration, Easing.Get(easing), onComplete);

        /// <summary>
        /// 滚动到指定的位置
        /// </summary>
        /// <param name="position"></param>
        /// <param name="duration"></param>
        /// <param name="easing"></param>
        /// <param name="onStart"></param>
        /// <param name="onComplete"></param>
        public void ScrollTo(float position, float duration, Ease easing, Action onStart, Action onComplete) => ScrollTo(position, duration, Easing.Get(easing), onStart, onComplete);

        public void ScrollTo(float position, float duration, EasingFunction easingFunction, Action onStart, Action onComplete)
        {
            if (duration <= 0f)
            {
                Position = CircularPosition(position, totalCount);
                onComplete?.Invoke();
                return;
            }

            var preIsEnable = autoScrollState.Enable;
            if (preIsEnable)
            {
                autoScrollState.StartTime = Time.unscaledTime;
                var preEndPos = autoScrollState.EndPosition;
                autoScrollState.EndPosition += CalculateMovementAmount(preEndPos, position);
                ;
                scrollStartPosition = currentPosition;

                UpdateSelection(Mathf.RoundToInt(CircularPosition(autoScrollState.EndPosition, totalCount)));
                return;
            }

            autoScrollState.Reset();
            autoScrollState.Enable = true;
            autoScrollState.isStarted = true;
            autoScrollState.Duration = duration;
            autoScrollState.EasingFunction = easingFunction ?? DefaultEasingFunction;
            autoScrollState.StartTime = Time.unscaledTime;

            var movement = CalculateMovementAmount(currentPosition, position);
            autoScrollState.EndPosition = currentPosition + movement;

            autoScrollState.OnStart = onStart;
            autoScrollState.OnComplete = onComplete;

            velocity = 0f;
            scrollStartPosition = currentPosition;

            var select = Mathf.RoundToInt(CircularPosition(autoScrollState.EndPosition, totalCount));
            UpdateSelection(select);
        }

        /// <summary>
        /// 指定した位置まで移動します.
        /// </summary>
        /// <param name="position">スクロール位置. <c>0f</c> ~ <c>totalCount - 1f</c> の範囲.</param>
        /// <param name="duration">移動にかける秒数.</param>
        /// <param name="easingFunction">移動に使用するイージング関数.</param>
        /// <param name="onComplete">移動が完了した際に呼び出されるコールバック.</param>
        public void ScrollTo(float position, float duration, EasingFunction easingFunction, Action onComplete = null)
        {
            if (duration <= 0f)
            {
                Position = CircularPosition(position, totalCount);
                onComplete?.Invoke();
                return;
            }

            var preIsEnable = autoScrollState.Enable;
            if (preIsEnable)
            {
                autoScrollState.StartTime = Time.unscaledTime;
                var preEndPos = autoScrollState.EndPosition;
                autoScrollState.EndPosition += CalculateMovementAmount(preEndPos, position);
                scrollStartPosition = currentPosition;
                
                UpdateSelection(Mathf.RoundToInt(CircularPosition(autoScrollState.EndPosition, totalCount)));
                return;
            }

            autoScrollState.Reset();
            autoScrollState.Enable = true;
            autoScrollState.isStarted = true;
            autoScrollState.Duration = duration;
            autoScrollState.EasingFunction = easingFunction ?? DefaultEasingFunction;
            autoScrollState.StartTime = Time.unscaledTime;

            var movement = CalculateMovementAmount(currentPosition, position);
            autoScrollState.EndPosition = currentPosition + movement;

            autoScrollState.OnComplete = onComplete;

            velocity = 0f;
            scrollStartPosition = currentPosition;

            var select = Mathf.RoundToInt(CircularPosition(autoScrollState.EndPosition, totalCount));
            UpdateSelection(select);
        }

        /// <summary>
        /// 指定したインデックスの位置までジャンプします.
        /// </summary>
        /// <param name="index">アイテムのインデックス.</param>
        public void JumpTo(int index)
        {
            if (index < 0 || index > totalCount - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            UpdateSelection(index);
            Position = index;
        }

        /// <summary>
        /// <paramref name="sourceIndex"/> から <paramref name="destIndex"/> に移動する際の移動方向を返します.
        /// スクロール範囲が無制限に設定されている場合は, 最短距離の移動方向を返します.
        /// </summary>
        /// <param name="sourceIndex">移動元のインデックス.</param>
        /// <param name="destIndex">移動先のインデックス.</param>
        /// <returns></returns>
        public MovementDirection GetMovementDirection(int sourceIndex, int destIndex)
        {
            var movementAmount = CalculateMovementAmount(sourceIndex, destIndex);
            return scrollDirection == ScrollDirection.Horizontal
                ? movementAmount > 0
                    ? MovementDirection.Left
                    : MovementDirection.Right
                : movementAmount > 0
                    ? MovementDirection.Up
                    : MovementDirection.Down;
        }

        /// <inheritdoc/>
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (!draggable || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            hold = true;
            velocity = 0f;
            autoScrollState.Reset();
        }

        /// <inheritdoc/>
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (!draggable || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (hold && snap.Enable)
            {
                UpdateSelection(Mathf.Clamp(Mathf.RoundToInt(currentPosition), 0, totalCount - 1));
                ScrollTo(Mathf.RoundToInt(currentPosition), snap.Duration, snap.Easing);
            }

            hold = false;
        }

        /// <inheritdoc/>
        void IScrollHandler.OnScroll(PointerEventData eventData)
        {
            if (!draggable)
            {
                return;
            }

            var delta = eventData.scrollDelta;

            // Down is positive for scroll events, while in UI system up is positive.
            delta.y *= -1;
            var scrollDelta = scrollDirection == ScrollDirection.Horizontal
                ? Mathf.Abs(delta.y) > Mathf.Abs(delta.x)
                        ? delta.y
                        : delta.x
                : Mathf.Abs(delta.x) > Mathf.Abs(delta.y)
                        ? delta.x
                        : delta.y;

            if (eventData.IsScrolling())
            {
                scrolling = true;
            }

            var position = currentPosition + 
                           (scrollDelta / ViewportSize * m_MouseWheelSensitivity);
            if (movementType == MovementType.Clamped)
            {
                position += CalculateOffset(position);
            }

            if (autoScrollState.Enable)
            {
                autoScrollState.Reset();
            }

            UpdatePosition(position);
        }

        /// <inheritdoc/>
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (!draggable || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            hold = false;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                viewport,
                eventData.position,
                eventData.pressEventCamera,
                out beginDragPointerPosition);

            scrollStartPosition = currentPosition;
            dragging = true;
            autoScrollState.Reset();
        }

        /// <inheritdoc/>
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (!draggable || eventData.button != PointerEventData.InputButton.Left || !dragging)
            {
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                viewport,
                eventData.position,
                eventData.pressEventCamera,
                out var dragPointerPosition))
            {
                return;
            }

            var pointerDelta = dragPointerPosition - beginDragPointerPosition;
            var position = (scrollDirection == ScrollDirection.Horizontal ? -pointerDelta.x : pointerDelta.y)
                           / ViewportSize
                           * m_DragSensitivity
                           + scrollStartPosition;

            var offset = CalculateOffset(position);
            position += offset;

            if (movementType == MovementType.Elastic)
            {
                if (offset != 0f)
                {
                    position -= RubberDelta(offset, m_DragSensitivity);
                }
            }

            UpdatePosition(position);
        }

        /// <inheritdoc/>
        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (!draggable || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            dragging = false;
        }

        float CalculateOffset(float position)
        {
            if (movementType == MovementType.Unrestricted)
            {
                return 0f;
            }

            if (position < 0f)
            {
                return -position;
            }

            var count = Mathf.Max(1, totalCount - 1);

            if (position > count)
            {
                return count - position;
            }

            return 0f;
        }

        private void UpdatePosition(float position, bool updateScrollbar = true)
        {
            onValueChanged?.Invoke(currentPosition = position);

            if (m_FancyScrollbar && updateScrollbar)
            {
                position = m_IsLoop ? CircularPosition(position, totalCount) : position;
                var value = Mathf.Clamp01(position / Mathf.Max(totalCount - 1f, 1e-4f));
                
                m_FancyScrollbar.SetValueWithoutNotify(value);
            }
        }

        void UpdateSelection(int index) => onSelectionChanged?.Invoke(index);

        float RubberDelta(float overStretching, float viewSize) =>
            (1 - 1 / (Mathf.Abs(overStretching) * 0.55f / viewSize + 1)) * viewSize * Mathf.Sign(overStretching);

        void Update()
        {
            var deltaTime = Time.unscaledDeltaTime;
            var offset = CalculateOffset(currentPosition);

            if (autoScrollState.Enable)
            {
                if (autoScrollState.isStarted)
                {
                    autoScrollState.Start();
                }

                var position = 0f;

                if (autoScrollState.Elastic)
                {
                    position = Mathf.SmoothDamp(currentPosition, currentPosition + offset, ref velocity,
                        elasticity, Mathf.Infinity, deltaTime);

                    if (Mathf.Abs(velocity) < 0.01f)
                    {
                        position = Mathf.Clamp(Mathf.RoundToInt(position), 0, totalCount - 1);
                        velocity = 0f;
                        autoScrollState.Complete();
                    }
                }
                else
                {
                    var alpha = Mathf.Clamp01((Time.unscaledTime - autoScrollState.StartTime) /
                                               Mathf.Max(autoScrollState.Duration, float.Epsilon));
                    position = Mathf.LerpUnclamped(scrollStartPosition, autoScrollState.EndPosition,
                        autoScrollState.EasingFunction(alpha));

                    if (Mathf.Approximately(alpha, 1f))
                    {
                        autoScrollState.Complete();
                    }
                }

                UpdatePosition(position);
            }
            else if (!(dragging || scrolling) && (!Mathf.Approximately(offset, 0f) || !Mathf.Approximately(velocity, 0f)))
            {
                var position = currentPosition;

                if (movementType == MovementType.Elastic && !Mathf.Approximately(offset, 0f))
                {
                    autoScrollState.Reset();
                    autoScrollState.Enable = true;
                    autoScrollState.Elastic = true;

                    UpdateSelection(Mathf.Clamp(Mathf.RoundToInt(position), 0, totalCount - 1));
                }
                else if (inertia)
                {
                    velocity *= Mathf.Pow(decelerationRate, deltaTime);

                    if (Mathf.Abs(velocity) < 0.001f)
                    {
                        velocity = 0f;
                    }

                    position += velocity * deltaTime;

                    if (snap.Enable && Mathf.Abs(velocity) < snap.VelocityThreshold)
                    {
                        ScrollTo(Mathf.RoundToInt(currentPosition), snap.Duration, snap.Easing, onAutoScrollStart, onAutoScrollEnd);
                    }
                }
                else
                {
                    velocity = 0f;
                }

                if (!Mathf.Approximately(velocity, 0f))
                {
                    if (movementType == MovementType.Clamped)
                    {
                        offset = CalculateOffset(position);
                        position += offset;

                        if (Mathf.Approximately(position, 0f) || Mathf.Approximately(position, totalCount - 1f))
                        {
                            velocity = 0f;
                            UpdateSelection(Mathf.RoundToInt(position));
                        }
                    }

                    UpdatePosition(position);
                }
            }

            if (!autoScrollState.Enable && (dragging || scrolling) && inertia)
            {
                var newVelocity = (currentPosition - prevPosition) / deltaTime;
                velocity = Mathf.Lerp(velocity, newVelocity, deltaTime * 10f);
            }

            prevPosition = currentPosition;
            scrolling = false;
        }

        /// <summary>
        /// 当用户按下Scrollbar
        /// </summary>
        /// <param name="scrollbarValue">介于[0,1]</param>
        private void OnScrollbarPointerDown(float scrollbarValue)
        {
            onScrollerGetPointerDownPosition?.Invoke(scrollbarValue);
        }

        float CalculateMovementAmount(float sourcePosition, float destPosition)
        {
            if (movementType != MovementType.Unrestricted)
            {
                var logicCount = Mathf.Max(1, totalCount - 1);
                return Mathf.Clamp(destPosition, 0, logicCount) - sourcePosition;
            }

            var sourceCircularPos = CircularPosition(sourcePosition, totalCount);
            var destCircularPos = CircularPosition(destPosition, totalCount);
            var amount = destCircularPos - sourceCircularPos;

            var absAmount = Mathf.Abs(amount);

            var halfCount = totalCount * 0.5f;

            if (amount < 0 && (absAmount > halfCount || Mathf.Approximately(absAmount, halfCount)))
            {
                amount = Mathf.Sign(-amount) * (totalCount - Mathf.Abs(amount));
            }

            return amount;
        }

        float CircularPosition(float p, int size) => size < 1 ? 0 : p < 0 ? size - 1 + (p + 1) % size : p % size;
    }
}
