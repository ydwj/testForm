using System;
using UniRx;

public class MsgMgr : Singleton<MsgMgr>
{
    /// <summary>
    /// 发布不带参数的消息
    /// </summary>
    /// <param name="gameEvent"></param>
    public void Publish(GameMsg gameEvent)
    {
        Msg msg = new Msg(gameEvent);
        MessageBroker.Default.Publish(msg);
    }

    /// <summary>
    /// 发布带参数的消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameEvent"></param>
    /// <param name="data"></param>
    public void Publish<T>(GameMsg gameEvent, T data)
    {
        Msg<T> msg = new Msg<T>(gameEvent, data);
        MessageBroker.Default.Publish(msg);
    }

    /// <summary>
    /// 注册不带数据的消息
    /// </summary>
    /// <param name="gameEvent"></param>
    /// <param name="OnRecieve">消息数据</param>
    public IDisposable Subscribe(GameMsg gameEvent, Action OnRecieve)
    {
        return MessageBroker.Default.Receive<Msg>().Subscribe(_ =>
        {
            if (_.gameEvent == gameEvent)
                OnRecieve?.Invoke();
        });
    }

    /// <summary>
    /// 注册带参数的消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameEvent"></param>
    /// <param name="OnRecieve"></param>
    public IDisposable Subscribe<T>(GameMsg gameEvent, Action<T> OnRecieve)
    {
        return MessageBroker.Default.Receive<Msg<T>>().Subscribe(_ =>
        {
            if (_.gameEvent == gameEvent)
                OnRecieve?.Invoke(_.data);
        });
    }

  
}

public interface IMsg
{
    GameMsg gameEvent { get;}
}

public interface IMsg<T> : IMsg
{
    T data { get; }
}

public class Msg : IMsg
{
    public GameMsg gameEvent { get; private set; }

    public Msg(GameMsg gameEvent)
    {
        this.gameEvent = gameEvent;
    }
}

public class Msg<T> : IMsg<T>
{
    public GameMsg gameEvent { get; private set; }
    public T data { get; private set; }

    public Msg(GameMsg gameEvent, T data)
    {
        this.gameEvent = gameEvent;
        this.data = data;
    }
}
