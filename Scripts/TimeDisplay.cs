using Godot;
using System;

public partial class TimeDisplay : Node2D
{
    private RichTextLabel _mainText;
    private DateTime _startTime;
    private DateTime _startTimerTime;

    public bool TimerRunning = false;

    public override void _Ready()
    {
        base._Ready();
        _mainText = GetNode<RichTextLabel>("ScreenContainer/SubViewport/Text");
        StartTimer();
    }

    private void StartTimer()
    {
        _startTime = DateTime.Now;
        _startTimerTime = DateTime.ParseExact(_mainText.Text, "dd-MM-yyyy HH:mm:ss.FFF", null);
        TimerRunning = true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!TimerRunning) return;
        TimeSpan elapsed = DateTime.Now - _startTime;
        DateTime newTimerTime = _startTimerTime.Add(elapsed);
        _mainText.Text = newTimerTime.ToString("dd-MM-yyyy HH:mm:ss.FFF");
    }

    // add time regurlarly


    // function for adding n amount of time to the display
}
