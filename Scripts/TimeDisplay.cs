using Godot;
using System;

public partial class TimeDisplay : Node2D
{
    private RichTextLabel _mainText;

    public bool TimerRunning = true;

    public override void _Ready()
    {
        base._Ready();
        _mainText = GetNode<RichTextLabel>("ScreenContainer/SubViewport/Text");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!TimerRunning) return;
        _mainText.Text = ((Shuttle)GetParent().GetParent()).CurrentTime.ToString("dd-MM-yyyy HH:mm:ss.FFF");
    }
}
