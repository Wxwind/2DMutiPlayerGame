using System;

public class Timer
{
    private float m_timeMs;
    private Action? m_OnComplete;
    private float m_value;

    private bool m_isRunning;
    private bool m_isFinished;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="timeMs">计时器计时的时间</param>
    /// <param name="OnComplete">计时结束时的回调</param>
    /// <param name="active">是否激活计时器</param>
    public Timer(float timeMs, Action? OnComplete = null, bool active = false)
    {
        this.m_timeMs = timeMs;
        this.m_OnComplete = OnComplete;
        m_value = 0;
        m_isRunning = active;
    }

    public void ResetTimerAndRun(float time, Action? OnComplete = null)
    {
        m_value = 0;
        this.m_timeMs = time;
        SetCallback(OnComplete);
        m_isFinished = false;
        m_isRunning = true;
    }


    private void SetCallback(Action? OnComplete)
    {
        this.m_OnComplete = OnComplete ?? m_OnComplete;
    }

    public void Tick(float dt)
    {
        if (!m_isRunning || m_isFinished)
        {
            return;
        }

        m_value += dt;
        if (m_value >= m_timeMs)
        {
            End();
        }
    }

    private void End()
    {
        m_isRunning = false;
        m_isFinished = true;
        m_OnComplete?.Invoke();
    }

    public void ReRun()
    {
        m_value = 0;
        m_isRunning = true;
        m_isFinished = false;
    }

    public void Stop()
    {
        m_isRunning = false;
    }

    public void Run()
    {
        m_isRunning = true;
    }
}