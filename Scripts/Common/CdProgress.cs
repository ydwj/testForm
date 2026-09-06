using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CdProgress : MonoBehaviour
{
    public Image imgProgressFill;
    public Text txtProgress;

    public void SetProgress(float remainSeconds, float totalTime)
    {
        float fValue = (totalTime - remainSeconds) / totalTime;
        float fValTmp = Mathf.Clamp(fValue, 0, 1);
        imgProgressFill.fillAmount = fValTmp;
        TimeSpan timeSpan = new TimeSpan(0, 0, (int)(remainSeconds));
        txtProgress.text = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }

    public float GetProgress()
    {
        return imgProgressFill.fillAmount;
    }
}
