using Godot;
using System;

// A simple Sprite2D that bobs up and down
public partial class BobbingSprite : Sprite2D
{
    [Export] public float BobbingAmplitude = 5.0f;
    [Export] public float BobbingSpeed = 2.0f;

    private float _initialY;
    private float _initialX;
    private float _time;

    public override void _Ready()
    {
        base._Ready();
        _initialY = Position.Y;
        _initialX = Position.X;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        _time += (float)delta * BobbingSpeed;
        Position = new Vector2(_initialX + Mathf.Sin(_time) * BobbingAmplitude, Position.Y);
    }
}
