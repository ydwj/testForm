/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: star
 * Created On: 2024/03/11
 * Description: special cells
 */

using System.Collections.Generic;
using UnityEngine;

namespace FancyScrollView
{
    public interface ISpecialCells
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Initialize();

        /// <summary>
        /// 获取特殊cells
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<uint, GameObject> GetSpecialCellIndexAndObject();

        /// <summary>
        /// 获取特殊cell物体
        /// </summary>
        /// <param name="index">特殊cell的index</param>
        /// <returns></returns>
        GameObject GetSpecialCellObject(uint index);

        /// <summary>
        /// 返回特殊cell物体
        /// </summary>
        /// <param name="index">特殊cell的index</param>
        void PushSpecialCellObject(uint index);

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="newSpecialCells"></param>
        /// <returns></returns>
        bool ResetSpecialCellIndexAndObject(Dictionary<uint, GameObject> newSpecialCells);

        /// <summary>
        /// 添加特殊cell的物体
        /// </summary>
        /// <param name="index">cell的索引</param>
        /// <param name="gameObj">特殊物体</param>
        bool AddSpecialCellGameObject(uint index, GameObject gameObj);

        /// <summary>
        /// 删除特殊cell的物体
        /// </summary>
        /// <param name="index">cell的索引</param>
        bool RemoveSpecialCellGameObject(uint index);
    }
}