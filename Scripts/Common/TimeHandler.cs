using UnityEngine;
using UniRx;
using System;
using System.Net;
using System.Net.Cache;
using System.Linq;
using UnityEngine.Networking;

public class TimeHandler
{
    private static DateTime GameStartTime = DateTime.MinValue;
    private static DateTime GameStartTime_Net = DateTime.MinValue;
    public static Subject<eLoadTimeResultType> OnLoadTime = new Subject<eLoadTimeResultType>();

    public static bool bNetTime = false;

    public static void Init(Action OnCompleted)
    {
        if (GameMgr.Ins.gameSetting.bUseLocalTime)
        {
            GameStartTime = DateTime.Now.AddSeconds(-Time.realtimeSinceStartup);
            bNetTime = false;
            Debuger.Log("GameStartTime(Local):" + GameStartTime);
            OnCompleted?.Invoke();
            Observable.EveryUpdate().Subscribe(t => now = GameStartTime.AddSeconds(Time.realtimeSinceStartup));
            return;
        }

        OnLoadTime.Subscribe(_ =>
        {
            switch (_)
            {
                case eLoadTimeResultType.Success:
                    OnCompleted?.Invoke();
                    OnCompleted = null;
                    Observable.EveryUpdate().Subscribe(t => now = GameStartTime.AddSeconds(Time.realtimeSinceStartup));
                    break;
                case eLoadTimeResultType.Reload:
                    LoadTime().Subscribe();
                    break;
                default:
                    break;
            }
        });

        LoadTime().Subscribe();
    }

    private static IObservable<Unit> LoadTime()
    {
        return GetTimeMethod1().Merge(GetTimeMethod2()).Merge(GetTimeMethod3())
            .Where(t => t != null && t != DateTime.MinValue)
            .Take(1).ObserveOnMainThread()
            .DoOnError(_ =>
            {
                Debuger.Log(_.ToString());
            })
            .DoOnCompleted(() =>
             {
                 if (GameStartTime_Net == DateTime.MinValue)
                 {
                     GameStartTime = DateTime.Now.AddSeconds(-Time.realtimeSinceStartup);
                     bNetTime = false;
                     Debuger.Log("GameStartTime(Local):" + GameStartTime);
                     OnLoadTime.OnNext(eLoadTimeResultType.Success);
                     //ToolsMgr.ShowTipsByKey("str_net_connectfail");

                     //重试
                     Observable.Timer(TimeSpan.FromSeconds(10)).Subscribe(_ =>
                     {
                         OnLoadTime.OnNext(eLoadTimeResultType.Reload);
                     });
                 }
             })
            .Do(_ =>
             {
                 GameStartTime = _.AddSeconds(-Time.realtimeSinceStartup);
                 if (GameMgr.GameSetting.bUseLocalTime)
                     GameStartTime = DateTime.Now.AddSeconds(-Time.realtimeSinceStartup);

                 GameStartTime_Net = GameStartTime;
                 bNetTime = true;
                 Debuger.Log("GameStartTime(Net):" + GameStartTime);
                 OnLoadTime.OnNext(eLoadTimeResultType.Success);
             })
             .AsUnitObservable();
    }

    private static IObservable<DateTime> GetTimeMethod1()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.microsoft.com");
        request.Method = "GET";
        request.Accept = "text/html, application/xhtml+xml, */*";
        request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
        request.ContentType = "application/x-www-form-urlencoded";
        request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
        return request.GetResponseAsObservable().Select(_ =>
        {
            try
            {
                return DateTime.ParseExact(_.Headers["date"], "ddd, dd MMM yyyy HH:mm:ss 'GMT'", System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat, System.Globalization.DateTimeStyles.AssumeUniversal);
            }
            catch
            {
                return DateTime.MinValue;
            }
        })
        .Timeout(TimeSpan.FromSeconds(5))
        .CatchIgnore();
    }

    private static IObservable<DateTime> GetTimeMethod2()
    {
        return ObservableWWW.Get("https://www.hko.gov.hk/cgi-bin/gts/time5a.pr?a=1")
        .Select(_ =>
        {
            try
            {
                return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(Convert.ToDouble(_.Substring(2)));
            }
            catch
            {
                return DateTime.MinValue;
            }
        })
        .Timeout(TimeSpan.FromSeconds(5))
        .CatchIgnore();
    }

    private static IObservable<DateTime> GetTimeMethod3()
    {
        var obRequest1 = UnityWebRequest.Get("time.windows.com").SendWebRequest().AsAsyncOperationObservable();
        var obRequest2 = UnityWebRequest.Get("time1.google.com").SendWebRequest().AsAsyncOperationObservable();
        return obRequest1.Merge(obRequest2)
            .Select(_ =>
            {
                try
                {
                    var headers = _.webRequest.GetResponseHeaders();
                    return DateTime.ParseExact(headers["date"], "ddd, dd MMM yyyy HH:mm:ss 'GMT'", System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat, System.Globalization.DateTimeStyles.AssumeUniversal);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            })
        .Timeout(TimeSpan.FromSeconds(5))
        .CatchIgnore();
    }

    private static DateTime now;
    public static DateTime Now => now;
}

public enum eLoadTimeResultType
{
    Success,
    Reload,
}