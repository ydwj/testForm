using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class GameObjectPool
{
    private Stack<GameObject> m_ItemStack;

    private readonly GameObject mTemp;
    Transform mRoot;

    public GameObjectPool(GameObject inTemp)
    {
        m_ItemStack = new Stack<GameObject>();
        mTemp = inTemp;
        //mTemp.SetActive(false);
        mRoot = mTemp.transform.parent;
    }
    public GameObjectPool(GameObject inTemp,Transform inRoot)
    {
        m_ItemStack = new Stack<GameObject>();
        mTemp = inTemp;
        //mTemp.SetActive(false);
        mRoot = inRoot;
    }

    /// <summary>
    /// 从池子获取对象
    /// </summary>
    /// <returns></returns>
    public GameObject GetPool(Transform parent = null)
    {
        GameObject item;
        if (m_ItemStack.Count == 0)
        {
            item = CreateObj();
            item.SetActive(true);
        }
        else
        {
            item = m_ItemStack.Pop();
            while (item == null && m_ItemStack.Count > 0)
            {
                item = m_ItemStack.Pop();
            }

            if (item == null)
                item = CreateObj();
            item.SetActive(true);
        }
        if (parent != null)
            item.transform.SetParent(parent);
        else
        {
            item.transform.SetParent(mRoot);
        }
        
        return item;
    }

    /// <summary>
    /// 创建对象
    /// </summary>
    /// <returns></returns>
    private GameObject CreateObj()
    {
        var go = GameObject.Instantiate(mTemp, mRoot);
        go.name = mTemp.name;
        return go;
    }

    /// <summary>
    /// 回收指定对象
    /// </summary>
    /// <param name="obj"></param>
    public void Recycle(GameObject obj)
    {
        //if (!obj.activeInHierarchy) return;
        if (m_ItemStack.Contains(obj))
            return;
        obj.SetActive(false);
        m_ItemStack.Push(obj);
    }

    /// <summary>
    /// 回收所有对象
    /// </summary>
    public void RecycleAll()
    {
        foreach (var item in m_ItemStack)
        {
            Recycle(item);
        }
    }

    /// <summary>
    /// 释放数据
    /// </summary>
    public void Release()
    {
        foreach (var item in m_ItemStack)
        {
            GameObject.Destroy(item);
        }
        
        m_ItemStack.Clear();
    }

    /// <summary>
    /// 移除所有实例物体
    /// </summary>
    public void DestoryAll()
    {
        foreach (var item in m_ItemStack)
        {
            GameObject.Destroy(item);
        }
    }
}
