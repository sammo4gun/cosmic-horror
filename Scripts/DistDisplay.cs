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
    }

    private static String GetStringDistance(float distance)
    {
        if (distance < 1_000.0f) return distance.ToString("F1");
        if (distance < 1_000_000.0f) return distance.ToString("F0");
        if (distance < 10_000_000.0f) return (distance / 1_000_000.0f).ToString("F1") + " mil";
        if (distance < 1_000_000_000.0f) return (distance / 1_000_000.0f).ToString("F0") + " mil";
        if (distance < 10_000_000_000.0f) return (distance / 1_000_000_000.0f).ToString("F1") + " bil";
        if (distance < 1000_000_000_000.0f) return (distance / 1_000_000_000.0f).ToString("F0") + " bil";
        return distance.ToString("F0");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!DisplayRunning) return;
        string distance = GetStringDistance(((Shuttle)GetParent().GetParent()).DistanceFromEarth);
        _mainText.Text = "distance " + distance + " km";
    }
}
