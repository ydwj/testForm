using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FancyScrollView
{
    public class FancyScrollbar : Scrollbar
    {
        private RectTransform m_RectTransform;

        private RectTransform rectTransform
        {
            get
            {
                if (m_RectTransform == null)
                {
                    m_RectTransform = GetComponent<RectTransform>();
                }

                return m_RectTransform;
            }
        }

        private bool horizontal => direction == Direction.LeftToRight || direction == Direction.RightToLeft;

        private bool isReverse => direction == Direction.RightToLeft || direction == Direction.TopToBottom;

        /// <summary>
        /// 用户按下Scrollbar的事件
        /// 参数float介于[0,1]
        /// </summary>
        public UnityEvent<float> onPointerDown = new ScrollEvent();

        public override void OnPointerDown(PointerEventData eventData)
        {
            var screenPosition = eventData.pointerPressRaycast.screenPosition;
            var camera = eventData.enterEventCamera;

            if (!RectTransformUtility.RectangleContainsScreenPoint(handleRect, screenPosition, camera))
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        rectTransform, screenPosition, camera,
                        out var localMousePos))
                {
                    var rect = rectTransform.rect;
                    
                    var point = horizontal ? localMousePos.x : localMousePos.y;
                    var halfHandle = horizontal ? rect.width : rect.height;
                    halfHandle = isReverse ? -halfHandle : halfHandle;
                    
                    var originalValue = (point + (halfHandle * 0.5f)) / halfHandle;
                    originalValue = Mathf.Clamp01(originalValue);
                    originalValue = Mathf.Round(originalValue * 10000f) / 10000f;
                    
                    onPointerDown?.Invoke(originalValue);
                }
            }
        }
    }
}