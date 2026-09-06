/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: star
 * Created On: 2024-09-29
 * Description: Mode of Scroll View
 */

using System;

namespace FancyScrollView
{
    [Serializable]
    public enum FancyViewMode
    {
        // ScrollView模式,
        // 首尾Cell的位置位于ViewPort的中央
        ScrollView,

        // ScrollRect模式,
        // 首cell的起始位置位于ViewPort的起始边界,
        // 尾cell的结束位置位于ViewPort的结束边界
        ScrollRect
    }
}