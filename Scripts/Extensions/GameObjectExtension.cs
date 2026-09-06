using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtension
{
    public static T GetOrAddComponent<T>(this GameObject gameObject)
    where T : Component
    {
        var component = gameObject.GetComponent<T>();
        if (component == null)
            component = gameObject.AddComponent<T>();

        return component;
    }

    public static void ClearChildren(this Transform transParent)
    {
        for (int i = transParent.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(transParent.GetChild(i).gameObject);
        }
    }
    
    public static void ClearChildrenImmediate(this Transform transParent)
    {
        for (int i = transParent.childCount - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(transParent.GetChild(i).gameObject);
        }
    }

    public static bool Overlaps(this RectTransform a, RectTransform b)
    {
        return a.WorldRect().Overlaps(b.WorldRect());
    }
    public static bool Overlaps(this RectTransform a, RectTransform b, bool allowInverse)
    {
        return a.WorldRect().Overlaps(b.WorldRect(), allowInverse);
    }

    public static Rect WorldRect(this RectTransform rectTransform)
    {
        Vector2 sizeDelta = rectTransform.sizeDelta;
        float rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
        float rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

        Vector3 position = rectTransform.position;
        return new Rect(position.x + rectTransformWidth * rectTransform.pivot.x, position.y - rectTransformHeight * rectTransform.pivot.y, rectTransformWidth, rectTransformHeight);
    }

    public static void ResetLocalPos(this Transform transTarget)
    {
        transTarget.localPosition = Vector3.zero;
    }

    public static void GetChildrenTransformWithOutSelf(this Transform transParent, out Transform[] transforms)
    {
        transforms = new Transform[transParent.childCount];
        int nIndex = 0;

        foreach (Transform transChild in transParent)
        {
            transforms[nIndex] = transChild;
            nIndex++;
        }
    }
}
