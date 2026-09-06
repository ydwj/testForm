/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Linq;
using EasingCore;
using UnityEngine;

namespace FancyScrollView.Example01Special
{
    class Example01Special : MonoBehaviour
    {
        [SerializeField] ScrollView scrollView = default;

        void Start()
        {
            scrollView.RefreshScrollView();

            var items = Enumerable.Range(0, 20)
                .Select(i => new ItemData($"Cell {i}"))
                .ToArray();

            scrollView.UpdateData(items);

            scrollView.ScrollTo(10, 0.5f, Ease.InOutSine);
        }
    }
}