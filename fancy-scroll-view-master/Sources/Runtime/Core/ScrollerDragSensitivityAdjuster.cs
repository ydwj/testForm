/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: star
 * Created On: 2024/10/12
 * Description: Adjuster of Scroller Drag Sensitivity
 */

using System;

namespace FancyScrollView
{
    /// <summary>
    /// Scroller拖拽速率调整器
    /// </summary>
    [Serializable]
    internal class ScrollerDragSensitivityAdjuster
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool enable;

        /// <summary>
        /// 速度倍率
        /// </summary>
        public int speed = 1;
    }
}