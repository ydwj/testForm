using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.EventSystems;

public class FingerMoveRotate : MonoBehaviour, IPointerDownHandler , IPointerUpHandler
{
    public Transform transRotateTarget;
    bool bIsDown = false;

    Vector3 preMousePos = Vector3.zero;

    protected void OnGUI()
    {
        ////没有触摸  
        //if (Input.touchCount <= 0)
        //{
        //    return;
        //}
        if (Event.current.type == EventType.MouseDrag)
        {
            if (bIsDown)
            {
                var deltaposition = Input.mousePosition - preMousePos;
                preMousePos = Input.mousePosition;
                transRotateTarget.DORotate(new Vector3(0, -deltaposition.x, 0), 0.1f).SetEase(Ease.Linear).SetRelative();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        bIsDown = true;
        preMousePos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        bIsDown = false;
    }
}
