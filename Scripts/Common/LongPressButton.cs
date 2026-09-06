using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LongPressButton : Button
{
    [Serializable]
    public class LongButtonEvent : UnityEvent { }
    [SerializeField]
    private LongButtonEvent _onLongButtonClick = new LongButtonEvent();
    public LongButtonEvent OnLongButtonClick
    {
        get
        {
            return _onLongButtonClick;
        }
        set
        {
            _onLongButtonClick = value;
        }
    }

    public bool bEnableLongPress = true;

    bool bLongPressed = false;
    const float fStartTime = 0.3f;
    const float fInterval = 0.08f;
    float fStartTimer = 0;
    float fTimer = 0;

    private void Update()
    {
        if (IsPressed() && bEnableLongPress)
        {
            fStartTimer += Time.deltaTime;
            if (fStartTimer > fStartTime)
            {
                fTimer += Time.deltaTime;
                if (fTimer > fInterval)
                {
                    bLongPressed = true;
                    Press();
                }
            }
        }
    }

    void Press()
    {
        if (OnLongButtonClick != null)
            OnLongButtonClick.Invoke();
        fTimer = 0;
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (!bLongPressed)
            Press();

        fStartTimer = 0;
        bLongPressed = false;
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        fTimer = 0;
        fStartTimer = 0;
    }
}