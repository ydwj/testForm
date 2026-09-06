using System;
using UnityEngine;
//计时器
public class Timer : IDisposable
{
    public delegate void TimerHandler();
    public delegate void TimerArgsHandler(System.Object[] args);

    public TimerHandler Handler;         //无参回调
    public TimerArgsHandler ArgsHandler; //带参回调
    public float DelayTime;              //时间延迟
    public float IntervalTime;           //一定时间间隔触发，否则为0
    public System.Object[] Args;         //参数
    public float CurrentTime = 0;        //当前时间
    public eTimerName TimerName;             //计时器标示
    public bool IsPause = false;         //是否暂停中
    public bool bDontKillWhenReach = false; //到达后是否清除

    public Timer()
    {

    }

    /// <summary>
    /// 创建一个时间事件对象
    /// </summary>
    /// <param name="Handler">回调函数</param>
    /// <param name="ArgsHandler">带参数的回调函数</param>
    /// <param name="intervalTime">时间内执行</param>
    /// <param name="repeats">重复次数</param>
    /// <param name="Args">参数  可以任意的传不定数量，类型的参数</param>
    public Timer(eTimerName name, TimerHandler Handler, TimerArgsHandler ArgsHandler, float delayTime,
        float intervalTime, System.Object[] Args, bool bDontKillWhenReach = false)
    {
        this.TimerName = name;
        this.Handler = Handler;
        this.ArgsHandler = ArgsHandler;
        this.DelayTime = delayTime;
        this.CurrentTime = delayTime;
        this.IntervalTime = intervalTime;
        this.Args = Args;
        this.bDontKillWhenReach = bDontKillWhenReach;
    }
    ///执行函数
    public void Notify()
    {
        if (Handler != null)
            Handler();
        if (ArgsHandler != null)
            ArgsHandler(Args);
    }
    //清楚timer数据
    public void CleanUp()
    {
        TimerName = eTimerName.None;
        Handler = null;
        ArgsHandler = null;
        DelayTime = 0;
        IntervalTime = 0;
        CurrentTime = DelayTime;
        IsPause = false;
    }

    public void Reset()
    {
        CurrentTime = DelayTime;
        IsPause = false;
    }

    public void Dispose()
    {
        CleanUp();
    }
}

public enum eTimerName
{
    None,
    Cursor,
    MoodTip,
    Event_BrainWash,
    LostItemTip,
}
