using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public enum eObjTimerType
{ 
    Normal,
    Loop
}
//和场景中物体Update周期绑定的Mgr(对象池物体需要手动清除定时器)
public class ObjRelateTimerMgr : UnitySingleton<ObjRelateTimerMgr>
{

    private List<ObjTimer> listObjTimer = new List<ObjTimer>();

    private void Update()
    {
        for (int i = 0; i < listObjTimer.Count; i++)
        {
            listObjTimer[i].Update();
        }
    }

    public void RecycleTimer(ObjTimer objTimer )
    {
        listObjTimer.Remove(objTimer);
        objTimer?.Dispose();
    }


    public ObjTimer AddTimer(float interval, MonoBehaviour mono, Action action )
    {

        ObjTimer objTimer = new ObjTimer(interval, mono, action, eObjTimerType.Normal);
        listObjTimer.Add(objTimer);
        return objTimer;
    }

    public ObjTimer AddloopTimer(float interval, MonoBehaviour mono, Action action)
    {

        ObjTimer objTimer = new ObjTimer(interval, mono, action, eObjTimerType.Loop);
        listObjTimer.Add(objTimer);
        return objTimer;
    }

    //对象池清理Timer专用,手动调用

    public void ClearRelateObjTimer(MonoBehaviour target)
    {
        for (int i = listObjTimer.Count-1; i >= 0; i--)
        {
            var timer = listObjTimer[i];
            if (timer.GetRelate() == target)
            {
                listObjTimer.Remove(timer);
                timer?.Dispose();
            }
        }
    }
}


public class ObjTimer : IDisposable
{
    private float _interval , loopTime ;
    private MonoBehaviour  _mono;
    private Action callBack;
    private eObjTimerType eObjTimerType;
    public ObjTimer(float interval, MonoBehaviour mono , Action action , eObjTimerType oType)
    {
        _interval = interval;
        _mono = mono;
        callBack = action;
        eObjTimerType = oType;
        if (oType == eObjTimerType.Loop)
            loopTime = interval;
    }

    public void Update()
    {
        if (_mono == null)
        {
            RecycleSelf();
            return;
        }

        if (_interval < 0)
        {
            if (eObjTimerType == eObjTimerType.Loop)
            {
                callBack?.Invoke();
                ReAssignInterval(loopTime);
                return;
            }

            callBack?.Invoke();
            RecycleSelf();
            return;
        }

        if (_mono.isActiveAndEnabled)
        {
            _interval -= Time.deltaTime;
        }


    }

    //重置时间
    public void ReAssignInterval(float time,bool isAdd =false )
    {
        if (!isAdd)
        { 
            _interval = time;
        }
        else
        {
            _interval += time;
        }
    }

    //改变回调方法
    public void ReAssignCallback(Action newAction )
    {
        callBack = null ;
        callBack += newAction;
    }

    public MonoBehaviour GetRelate()
    {
        return _mono;
    }

     void RecycleSelf()
    {
        ObjRelateTimerMgr.Ins.RecycleTimer(this);
    }

    public void Dispose()
    {
        _mono = null;
        callBack = null;
    }
}

