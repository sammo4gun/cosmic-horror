using Godot;
using System;

public partial class DistDisplay : Node2D
{
    private RichTextLabel _mainText;

    public bool DisplayRunning = true;

    public override void _Ready()
    {
        base._Ready();
        _mainText = GetNode<RichTextLabel>("ScreenContainer/SubViewport/Text");
        _mainText.Text = ((Shuttle)GetParent().GetParent()).DistanceFromEarth.ToString("F0") + " km";
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!DisplayRunning) return;
        _mainText.Text = ((Shuttle)GetParent().GetParent()).DistanceFromEarth.ToString("F0") + " km";
    }
}
