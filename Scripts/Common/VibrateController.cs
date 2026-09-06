using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class VibrateController : MonoBehaviour
{
    private static AndroidJavaObject jo;
    public static void DoVibrate(long lMillionSec, int nAmplitude = 255)
    {
        if (!GameDataMgr.Ins.IsGameVibrateOpen)
            return;
#if !UNITY_EDITOR && UNITY_ANDROID
        try
        {
            if (jo == null)
            {
                var jc = new AndroidJavaClass("com.craftman.vibrate.VibratorClass");
                jo = jc.CallStatic<AndroidJavaObject>("GetInstance");
            }

            jo.Call("DoVibrator", lMillionSec, nAmplitude);
        }
        catch (System.Exception ex)
        {
            Debuger.Log(ex);
        }
#endif

#if !UNITY_EDITOR && UNITY_IOS
                //IOS_Shake.StartShake();
#endif
    }
}
