using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TimerMgr : UnitySingleton<TimerMgr>
{
    private Dictionary<int, Timer> timer_list; //时间管理器
    private List<int> remove_list = new List<int>();

    protected override void Awake()
    {
        base.Awake();
        if (timer_list == null)
            timer_list = new Dictionary<int, Timer>();
    }

    /// <summary>
    /// 创建延迟计时器
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="action"></param>
    public void CreateUnityTimer(float seconds, Callback action)
    {
        StartCoroutine(Delay(seconds, action));
    }

    private IEnumerator Delay(float seconds, Callback action)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }

    /// <summary>
    /// 创建计时器
    /// </summary>
    /// <param name="name">timer标示名字</param>
    /// <param name="time">延迟执行timer时间</param>
    /// <param name="intervalTime">间隔执行timer时间</param>
    /// <param name="callBack">timer执行的回调函数</param>
    public Timer CreateTimer(eTimerName name, float time, Timer.TimerHandler callBack, bool bDontKillWhenReach = false)
    {
        return Create(name, callBack, null, time, 0);
    }

    ///创建带参数的Timer
    public Timer CreateTimer(eTimerName name, float time, Timer.TimerArgsHandler callBack,
        params System.Object[] args)
    {
        return Create(name, null, callBack, time, 0, false, args);
    }

    /// <summary>
    /// 创建固定间隔timer
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fIntervalTime"></param>
    /// <param name="callBack"></param>
    /// <returns></returns>
    public Timer CreateIntervalTimer(eTimerName name, float fIntervalTime, Timer.TimerHandler callBack)
    {
        return Create(name, callBack, null, 0, fIntervalTime);
    }

    /// <summary>
    /// 暂停timer
    /// </summary>
    public void PauseTimer(eTimerName name, bool bIsPause)
    {
        timer_list.TryGetValue((int) name, out Timer timer);
        if (timer == null)
        {
            Debuger.Log(string.Format("没有名字为 {0} 的timer", name));
            return;
        }

        timer.IsPause = bIsPause;
    }

    private Timer Create(eTimerName name, Timer.TimerHandler callBack, Timer.TimerArgsHandler callBackArgs, float time,
        float intervalTime, bool bDontKillWhenReach = false, params System.Object[] args)
    {
        if (timer_list.ContainsKey((int) name))
        {
            DestroyTimer(name);
            
            //Debuger.LogWarning("创建的timer名字重复，需要换个名字～～～");
            //return null;
        }

        Timer timer = new Timer(name, callBack, callBackArgs, time, intervalTime, args, bDontKillWhenReach);
        timer_list.Add((int) name, timer);
        return timer;
    }

    /// <summary>
    /// 添加要销毁的timer
    /// </summary>
    void AddTimerToRemove(eTimerName removeName)
    {
        remove_list.Add((int) removeName);
    }

    /// <summary>
    /// 销毁timer
    /// </summary>
    public void DestroyTimer(eTimerName timerName)
    {
        if (timer_list.TryGetValue((int) timerName, out Timer timer))
        {
            timer_list.Remove((int)timerName);
            remove_list.Remove((int) timerName);
            timer.Dispose();
            timer = null;
        }
    }

    public void ReleaseTimerManager()
    {
        foreach (int timerName in timer_list.Keys)
        {
            AddTimerToRemove((eTimerName)timerName);
        }
    }

    /// <summary>
    /// 固定更新timer事件
    /// </summary>
    void Update()
    {
        if (timer_list.Count != 0)
        {
            Timer[] timers = timer_list.Values.ToArray();
            for (int i = 0; i < timers.Length; i++)
            {
                Timer timer = timers[i];
                if (!timer.IsPause)
                    timer.CurrentTime -= Time.deltaTime;
                if (timer.IntervalTime > 0)
                {
                    if ((timer.DelayTime - timer.CurrentTime) >= timer.IntervalTime)
                    {
                        timer.Notify();
                        timer.CurrentTime = timer.DelayTime;
                        continue;
                    }
                }
                else
                {
                    if (timer.CurrentTime <= 0)
                    {
                        if (!timer.IsPause)
                            timer.Notify();
                        timer.CurrentTime = 0;
                        timer.IsPause = true;
                        
                        if (!timer.bDontKillWhenReach)
                            AddTimerToRemove(timer.TimerName);
                    }
                }
            }
        }
    }

    void LateUpdate()
    {
        for (int i = 0; i < remove_list.Count; i++)
        {
            DestroyTimer((eTimerName) remove_list[i]);
        }
    }

    /// <summary>
    /// 获取timer还有多长时间到达
    /// </summary>
    /// <param name="timeName"></param>
    /// <returns></returns>
    public float GetTimerRemainTime(eTimerName timeName)
    {
        if (timer_list.TryGetValue((int) timeName, out Timer timer))
        {
            float fRemainTime = timer.CurrentTime;
            if (fRemainTime <= 0)
            {
                fRemainTime = 0;
            }

            return fRemainTime;
        }

        return 0;
    }

    /// <summary>
    /// 获取timer信息
    /// </summary>
    /// <param name="timerName"></param>
    /// <returns></returns>
    public Timer GetTimer(eTimerName timerName)
    {
        if (timer_list.TryGetValue((int) timerName, out Timer timer))
        {
            return timer;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 增加timer时间
    /// </summary>
    /// <param name="timerName"></param>
    /// <param name="fTime"></param>
    public void AddTimerTime(eTimerName timerName, float fTime, bool bDoStart = true)
    {
        if (timer_list.TryGetValue((int) timerName, out Timer timer))
        {
            timer.CurrentTime += fTime;
        }

        if (bDoStart)
        {
            timer.IsPause = false;
        }
    }

    /// <summary>
    /// 开始计算游戏开始时间
    /// </summary>
    private float spendTime = 0;

    private int currentMinute = 0;
    private int lastMinute = 0;
    private int gameTotalTime = 0;

    /// <summary>
    /// 开始游戏计时
    /// </summary>
    public void StartGameTime()
    {
        string gameTime = FileMgr.Ins.GetValue("GameTotalTime");
        if (gameTime != null)
            gameTotalTime = int.Parse(gameTime);
    }

    /// <summary>
    /// 更新游戏时间
    /// </summary>
    public void UpdateGameTime()
    {
        spendTime = Time.realtimeSinceStartup;
        currentMinute = (int) spendTime / 60;
        if (currentMinute != lastMinute)
        {
            gameTotalTime++;
            lastMinute = currentMinute;
            FileMgr.Ins.AddValue("GameTotalTime", gameTotalTime.ToString());
            FileMgr.Ins.Save();
        }
    }
}