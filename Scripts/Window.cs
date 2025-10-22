using Godot;
using System;

public partial class Window : Node2D
{
    private Stars _stars;
    private Blot _blot;
    public override void _Ready()
    {
        base._Ready();
    }

    public void SetWindow(string windowName)
    {
        GetNode<SubViewportContainer>(windowName).Visible = true;
        _stars = GetNode<Stars>($"{windowName}/SubViewport/Stars");
        if (windowName == "Stars" || windowName == "SolarFlare")
        {
            _blot = GetNode<Blot>($"{windowName}/SubViewport/Blot");
        }
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

    public void ShowBlot(bool toggle)
    {
        if (_blot is not null)
        {
            _blot.Visible = toggle;
        }
    }

    public void MoveBlot(Vector2 newPos, float speed)
    {
        if (_blot is not null)
        {
            _blot.SetMoveTarget(newPos, speed);
        }
    }

    public void ScaleBlot(float newScale, float speed)
    {
        if (_blot is not null)
        {
            _blot.SetScaleTarget(newScale, speed);
        }
    }

    public void SetBlotPos(Vector2 newPos)
    {
        if (_blot is not null)
        {
            _blot.Position = newPos;
        }
    }

    public void ShowEye(bool toggle)
    {
        GetNode<Sprite2D>("Eye").Visible = toggle;
        GetNode<TextureRect>("OminousGlow").Visible = toggle;
    }
}
