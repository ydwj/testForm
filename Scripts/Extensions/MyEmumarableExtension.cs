using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class MyEmumarableExtension
{
    public static List<T> GetDistinctedSubList<T>(this List<T> listTarget, int nCount)
    {
        HashSet<int> set = new HashSet<int>();
        List<T> listNew = new List<T>();
        if (listTarget.Count < nCount)
        {
            return listTarget;
        }

        while (set.Count < nCount)
        {
            int index = Random.Range(0, listTarget.Count);
            if (!set.Contains(index))
            {
                set.Add(index);
                listNew.Add(listTarget[index]);
            }
        }
        return listNew;
    }

    public static T GetRandomChild<T>(this List<T> listTarget)
    {
        if (listTarget.Count == 0)
            return default;
        
        return listTarget[Random.Range(0, listTarget.Count)];
    }
    
    public static KeyValuePair<T1, T2> GetRandomChild<T1, T2>(this Dictionary<T1, T2> dicTarget)
    {
        if (dicTarget.Count == 0)
            return default;

        return dicTarget.ElementAtOrDefault(Random.Range(0, dicTarget.Count));
    }
    
    public static KeyValuePair<T1, T2> GetRandomChild<T1, T2>(this Dictionary<T1, T2> dicTarget, Func<KeyValuePair<T1, T2>, bool> predicate)
    {
        if (dicTarget.Count == 0)
            return default;

        var enumerable = dicTarget.Where(predicate);
        return enumerable.ElementAtOrDefault(Random.Range(0, enumerable.Count()));
    }

    public static T GetRandomChild<T>(this T[] arrTarget)
    {
        if (arrTarget.Length == 0)
            return default;
        
        return arrTarget[Random.Range(0, arrTarget.Length)];
    }

    public static T GetOrLast<T>(this List<T> listTarget, int nIndex)
    {
        if (nIndex > listTarget.Count - 1)
            return listTarget.LastOrDefault();
        
        return listTarget[nIndex];
    }

    public static T GetOrLast<T>(this T[] arrTarget, int nIndex)
    {
        if (nIndex > arrTarget.Length - 1)
            return arrTarget.LastOrDefault();
        
        return arrTarget[nIndex];
    }

    public static T GetOrLast<T>(this Queue<T> collection, int nIndex)
    {
        if (nIndex > collection.Count() - 1 || nIndex < 0)
            return collection.LastOrDefault();
        
        int nTmp = 0;

        foreach (var item in collection)
        {
            if (nTmp == nIndex)
                return item;
            nTmp++;
        }

        return collection.LastOrDefault();
    }
    
    public static KeyValuePair<T1, T2> GetAtIndex<T1, T2>(this Dictionary<T1, T2> collection, int nIndex)
    {
        if (nIndex > collection.Count() - 1 || nIndex < 0)
            return default;
        
        int nTmp = 0;

        foreach (var item in collection)
        {
            if (nTmp == nIndex)
                return item;
            nTmp++;
        }

        return default;
    }

    public static T GetOrFirst<T>(this List<T> listTarget, int nIndex)
    {
        if (nIndex > listTarget.Count - 1)
            return listTarget.FirstOrDefault();
        
        return listTarget[nIndex];
    }

    public static T GetOrFirst<T>(this T[] arrTarget, int nIndex)
    {
        if (nIndex > arrTarget.Length - 1)
            return arrTarget.FirstOrDefault();
        
        return arrTarget[nIndex];
    }

    public static T GetOrFirst<T>(this Queue<T> collection, int nIndex)
    {
        if (nIndex > collection.Count() - 1 || nIndex < 0)
            return collection.FirstOrDefault();
        
        int nTmp = 0;

        foreach (var item in collection)
        {
            if (nTmp == nIndex)
                return item;
            nTmp++;
        }

        return collection.FirstOrDefault();
    }

    public static T GetOrDefault<T>(this Queue<T> collection, int nIndex)
    {
        if (nIndex > collection.Count() - 1 || nIndex < 0)
            return default;
        
        int nTmp = 0;

        foreach (var item in collection)
        {
            if (nTmp == nIndex)
                return item;
            nTmp++;
        }

        return default;
    }

    public static int IndexOf<T>(this Queue<T> collection, T searchItem)
    {
        int i = 0;
        foreach (var item in collection)
        {
            if (EqualityComparer<T>.Default.Equals(item, searchItem))
                return i;
            i++;
        }

        return -1;
    }
    
    public static int IndexOf<T>(this T[] collection, T searchItem)
    {
        int i = 0;
        foreach (var item in collection)
        {
            if (EqualityComparer<T>.Default.Equals(item, searchItem))
                return i;
            i++;
        }

        return -1;
    }
}
