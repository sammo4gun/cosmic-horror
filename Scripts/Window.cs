using Godot;
using System;

public partial class Window : Node2D
{
    private Stars _stars;
    public override void _Ready()
    {
        base._Ready();
    }

    public void SetWindow(string windowName)
    {
        GetNode<SubViewportContainer>(windowName).Visible = true;
        _stars = GetNode<Stars>($"{windowName}/SubViewport/Stars");
    }

    public void SetAsteroidsVisible(bool toggle, float interval = 2.0f)
    {
        GetNode<AsteroidHandler>("AsteroidHandler").Visible = toggle;
        GetNode<AsteroidHandler>("AsteroidHandler").SpawnInterval = interval;
    }

    public void DeleteAllAsteroids()
    {
        foreach (var child in GetTree().CurrentScene.GetChildren())
        {
            if (child is Asteroid)
            {
                child.QueueFree();
            }
        }
    }

    public void SetSpinning(float spinStrength)
    {
        _stars.spinningStrength = spinStrength;
    }
}
