/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: star
 * Created On: 2024/03/11
 * Description: impl of ISpecialCells
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace FancyScrollView
{
    public class SpecialCells : MonoBehaviour, ISpecialCells
    {
        [Serializable]
        private struct IndexObjPair
        {
            [SerializeField]
            public uint index;

            [SerializeField]
            public GameObject obj;
        }

        #region Fields

        private Transform m_Root;

        /// <summary>
        /// 特殊cell的id和GameObject
        /// </summary>
        private Dictionary<uint, GameObject> m_SpecialCellIndexAndGameObjectDict = new Dictionary<uint, GameObject>();

        /// <summary>
        /// 通过脚本指定的, 特殊cell的id和GameObject
        /// </summary>
        [Header("指定的Index的Cell将会显示指定的GameObject")]
        [SerializeField]
        [FormerlySerializedAs("specialCellIndexObjPairs")]
        private List<IndexObjPair> scriptSpecialCellIndexAndGameObjPairs = new List<IndexObjPair>();

        /// <summary>
        /// 检查特殊cell的id是否有重复用的hashset
        /// </summary>
        private readonly HashSet<uint> m_SpecialCellIndexHashSet = new HashSet<uint>();

        #endregion

        #region Life Event

        public void Initialize()
        {
            CreateSpecialRoot();
            CreateSpecialCells();
        }

        private void CreateSpecialRoot()
        {
            m_Root = new GameObject("SpecialCells").transform;
            m_Root.SetParent(transform, false);
        }

        private void CreateSpecialCells()
        {
            if (CheckScriptValid())
            {
                ScriptSpecialCellsCopyToPrivate();
            }
        }

        private bool CheckScriptValid()
        {
            if (scriptSpecialCellIndexAndGameObjPairs == null || scriptSpecialCellIndexAndGameObjPairs.Count < 1)
            {
                return false;
            }

            m_SpecialCellIndexHashSet.Clear();
            for (var i = scriptSpecialCellIndexAndGameObjPairs.Count - 1; i > -1; i--)
            {
                var index = scriptSpecialCellIndexAndGameObjPairs[i].index;
                if (!m_SpecialCellIndexHashSet.Add(index))
                {
                    scriptSpecialCellIndexAndGameObjPairs.RemoveAt(i);
                    Debug.LogWarning($"FancySpecialCells: Duplicate index, special cell index of {index} was removed.");
                    continue;
                }

                var gameObj = scriptSpecialCellIndexAndGameObjPairs[i].obj;
                if (gameObj == null)
                {
                    scriptSpecialCellIndexAndGameObjPairs.RemoveAt(i);
                    Debug.LogWarning($"FancySpecialCells: Null reference gameObject, special cell index of {index} was removed.");
                }
            }

            return true;
        }

        private void ScriptSpecialCellsCopyToPrivate()
        {
            if (m_SpecialCellIndexAndGameObjectDict != null)
            {
                DestroyPreSpecialCellGameObjects();
            }

            m_SpecialCellIndexAndGameObjectDict = new Dictionary<uint, GameObject>();
            for (var i = 0; i < scriptSpecialCellIndexAndGameObjPairs.Count; i++)
            {
                var id = scriptSpecialCellIndexAndGameObjPairs[i].index;
                var gameObj = scriptSpecialCellIndexAndGameObjPairs[i].obj;

                m_SpecialCellIndexAndGameObjectDict[id] = InstantiateSpecialObject(id, gameObj);
            }
        }

        #endregion

        #region Interface

        /// <summary>
        /// 添加特殊cell的IndexGameObj Pair
        /// </summary>
        /// <param name="index"></param>
        /// <param name="gameObj"></param>
        public bool AddSpecialCellGameObject(uint index, GameObject gameObj)
        {
            if (gameObj == null || m_SpecialCellIndexAndGameObjectDict.ContainsKey(index))
            {
                return false;
            }

            m_SpecialCellIndexAndGameObjectDict[index] = InstantiateSpecialObject(index, gameObj);
            return true;
        }

        public bool RemoveSpecialCellGameObject(uint index)
        {
            if (m_SpecialCellIndexAndGameObjectDict.ContainsKey(index))
            {
                Destroy(m_SpecialCellIndexAndGameObjectDict[index]);
                m_SpecialCellIndexAndGameObjectDict.Remove(index);
                return true;
            }

            return false;
        }

        public bool ResetSpecialCellIndexAndObject(Dictionary<uint, GameObject> newSpecialCells)
        {
            if (!RemoveInvalidNewSpecialCells(newSpecialCells))
            {
                return false;
            }

            DestroyPreSpecialCellGameObjects();

            m_SpecialCellIndexAndGameObjectDict = CreateSpecialCellIndexAndGameObjects(newSpecialCells);

            return true;
        }

        public IReadOnlyDictionary<uint, GameObject> GetSpecialCellIndexAndObject()
        {
            if (m_SpecialCellIndexAndGameObjectDict.Count == 0)
            {
                return null;
            }

            return m_SpecialCellIndexAndGameObjectDict;
        }

        public GameObject GetSpecialCellObject(uint index) => m_SpecialCellIndexAndGameObjectDict.TryGetValue(index, out var obj) ? obj : null;

        public void PushSpecialCellObject(uint index)
        {
            if (m_SpecialCellIndexAndGameObjectDict.TryGetValue(index, out var obj))
            {
                var specialTrans = obj.transform;
                specialTrans.SetParent(m_Root, false);
                specialTrans.localPosition = Vector3.zero;
                specialTrans.localRotation = Quaternion.identity;
                obj.SetActive(false);
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// 复制特殊GameObject
        /// </summary>
        /// <param name="index"></param>
        /// <param name="gameObj"></param>
        /// <returns></returns>
        private GameObject InstantiateSpecialObject(uint index, GameObject gameObj)
        {
            if (gameObj == null)
            {
                return null;
            }

            // 复制物体, 设定父级和名称等
            var specialObj = Instantiate(gameObj, m_Root);
            specialObj.transform.SetParent(m_Root, false);
            specialObj.name = $"SpecialCell {index} {specialObj.GetHashCode()}";
            specialObj.SetActive(false);

            return specialObj;
        }

        /// <summary>
        /// 检查新的特殊cells是否合法
        /// </summary>
        /// <param name="newSpecialCells"></param>
        /// <returns></returns>
        private bool RemoveInvalidNewSpecialCells(Dictionary<uint, GameObject> newSpecialCells)
        {
            if (newSpecialCells == null || newSpecialCells.Count < 1)
            {
                return true;
            }

            m_SpecialCellIndexHashSet.Clear();

            var indexes = newSpecialCells.Keys.ToArray();
            for (var i = 0; i < indexes.Length; i++)
            {
                var index = indexes[i];

                if (!m_SpecialCellIndexHashSet.Add(index))
                {
                    newSpecialCells.Remove(index);
                    Debug.LogWarning($"FancySpecialCells: Duplicate index, special cell index of {index} was removed.");
                    continue;
                }

                if (newSpecialCells[index] == null)
                {
                    newSpecialCells.Remove(index);
                    Debug.LogWarning($"FancySpecialCells: Null reference gameObject, special cell index of {index} was removed.");
                }
            }

            return true;
        }

        /// <summary>
        /// Destroy之前的所有specialCell的GameObject
        /// </summary>
        private void DestroyPreSpecialCellGameObjects()
        {
            foreach (var specialObj in m_SpecialCellIndexAndGameObjectDict.Values)
            {
                Destroy(specialObj);
            }
        }

        private Dictionary<uint, GameObject> CreateSpecialCellIndexAndGameObjects(Dictionary<uint, GameObject> indexAndGameObjectDict)
        {
            if (indexAndGameObjectDict == null || indexAndGameObjectDict.Count < 1)
            {
                return new Dictionary<uint, GameObject>();
            }

            var reCreateIndexAndObj = new Dictionary<uint, GameObject>();
            foreach (var index in indexAndGameObjectDict.Keys)
            {
                var specialObj = InstantiateSpecialObject(index, indexAndGameObjectDict[index]);

                reCreateIndexAndObj.Add(index, specialObj);
            }

            return reCreateIndexAndObj;
        }

        #endregion
    }
}