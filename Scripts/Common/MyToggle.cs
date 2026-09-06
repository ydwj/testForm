using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyToggle : DirtyNode
{
    public GameObject goOff, goOn;
    public Button btnClick;

    public bool IsOn
    {
        get
        {
            return bIsOn;
        }
        set
        {
            bIsOn = value;
            Dirty(true);
        }
    }
    bool bIsOn = false;

    private void Awake()
    {
        Dirty(true);
    }

    protected override void OnReset()
    {
        goOff.SetActive(!bIsOn);
        goOn.SetActive(bIsOn);
    }

    public void OnValueChangeListiner(Action<bool> OnChange)
    {
        btnClick.onClick.AddListener(() =>
        {
            bIsOn = !bIsOn;
            OnChange?.Invoke(bIsOn);
            Dirty(true);
        });
    }
}
