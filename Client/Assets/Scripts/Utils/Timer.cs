using System;

public class Timer
{
    private float time;
    private Action OnComplete;
    private float value;

    public float Value
    {
        get => value;
    }

    public bool IsRunning { get; private set; }
    public bool IsFinished { get; private set; }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="time">计时器计时的时间</param>
    /// <param name="OnComplete">计时结束时的回调</param>
    /// <param name="active">是否激活计时器</param>
    public Timer(float time, Action OnComplete = null, bool active = false)
    {
        this.time = time;
        this.OnComplete = OnComplete;
        value = 0;
        IsRunning = active;
    }

    public void ResetTimerAndRun(float time, Action OnComplete = null)
    {
        value = 0;
        this.time = time;
        SetCallback(OnComplete);
        IsFinished = false;
        IsRunning = true;
    }


    private void SetCallback(Action OnComplete)
    {
        this.OnComplete = OnComplete ?? this.OnComplete;
    }

    public void Tick(float dt)
    {
        if (!IsRunning|| IsFinished)
        {
            return;
        }
        
        value += dt;
        if (value >= time)
        {
            End();
        }
    }

    private void End()
    {
        IsRunning = false;
        IsFinished = true;
        OnComplete?.Invoke();
    }

    public void ReRun()
    {
        value = 0;
        IsRunning = true;
        IsFinished = false;
    }

    public void Stop()
    {
        IsRunning = false;
    }

    public void Run()
    {
        IsRunning = true;
    }
}