/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: star
 * Created On: 2024-08-29
 * Description: controller of example one unfixed scrollView
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace FancyScrollView.Example01WithUnfixedSize
{
    internal class ExampleOneUnfixedScrollViewController : MonoBehaviour
    {
        [SerializeField]
        private ExampleOneUnfixedSizeScrollView exampleOneUnfixedSizeScrollView;

        [SerializeField]
        private int subCellMaxCount = 50;

        [SerializeField]
        private int cellCount = 1;

        [SerializeField]
        private int addCount = 20;

        [SerializeField]
        private bool holding = true;

        [SerializeField]
        private int cellIndex;

        [SerializeField]
        private float viewPortOffset;

        [SerializeField]
        private float cellOffset;

        private void Awake()
        {
            exampleOneUnfixedSizeScrollView.Init();
            InitExampleOneItemData();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                AddOrRemoveExampleOneItemData(10, false);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                AddOrRemoveExampleOneItemData(10, true);
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                exampleOneUnfixedSizeScrollView.ScrollToCell(cellIndex, 1f, viewPortOffset, cellOffset);
            }
        }

        private void AddOrRemoveExampleOneItemData(int count, bool addOrRemove)
        {
            var oldData = exampleOneUnfixedSizeScrollView.GetData();
            var newData = new List<ExampleOneUnfixedSizeItemData>(oldData);

            if (addOrRemove)
            {
                var lastItemCount = exampleOneUnfixedSizeScrollView.itemCount;

                var addData = CreateItemData(lastItemCount + count, lastItemCount);
                newData.AddRange(addData);
            }
            else
            {
                while (count > 0)
                {
                    if (newData.Count < 1)
                    {
                        break;
                    }

                    newData.RemoveAt(newData.Count - 1);
                    count--;
                }
            }

            exampleOneUnfixedSizeScrollView.UpdateData(newData, holding);
        }

        private void InitExampleOneItemData()
        {
            exampleOneUnfixedSizeScrollView.UpdateData(CreateItemData(cellCount, 0), false);
        }

        private IList<ExampleOneUnfixedSizeItemData> CreateItemData(int itemCount, int start)
        {
            var items = new List<ExampleOneUnfixedSizeItemData>();

            for (var i = start; i < itemCount; i++)
            {
                var itemData = new ExampleOneUnfixedSizeItemData();

                itemData.label = $"ScrollView {i}";

                var values = new List<int>();
                var count = Random.Range(1, subCellMaxCount);

                for (var j = 0; j < count; j++)
                {
                    var value = Random.Range(0, 99);
                    values.Add(value);
                }

                itemData.values = values.ToArray();

                items.Add(itemData);
            }

            return items;
        }
    }
}