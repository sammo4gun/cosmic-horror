using Godot;
using System;

public partial class TimeHandler : Node
{
    private DateTime _startTime;
    private DateTime _startTimerTime;

    public DateTime CurrentTime;

    public bool TimerRunning = false;

    public override void _Ready()
    {
        base._Ready();
        StartTimer();
    }

    private void StartTimer()
    {
        _startTime = DateTime.Now;
        _startTimerTime = DateTime.ParseExact("05-09-1977 12:56:01.000", "dd-MM-yyyy HH:mm:ss.FFF", null);
        TimerRunning = true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!TimerRunning) return;
        TimeSpan elapsed = DateTime.Now - _startTime;
        CurrentTime = _startTimerTime.Add(elapsed);
    }

    public void AddTime(int n, string unit)
    {
        switch (unit)
        {
            case "seconds":
                AddSeconds(n);
                break;
            case "minutes":
                AddSeconds(n * 60);
                break;
            case "hours":
                AddSeconds(n * 3600);
                break;
            case "days":
                AddSeconds(n * 86400);
                break;
            case "months":
                AddSeconds(n * 2592000);
                break;
            case "years":
                AddSeconds(n * 31536000);
                break;
            default:
                GD.PrintErr("Invalid time unit specified.");
                break;
        }
    }

    private void AddSeconds(int seconds)
    {
        _startTimerTime = _startTimerTime.AddSeconds(seconds);
        _startTime = DateTime.Now;
    }
}
