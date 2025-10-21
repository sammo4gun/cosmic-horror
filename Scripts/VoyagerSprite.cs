using Godot;
using System;

public partial class VoyagerSprite : Sprite2D
{
    [Export] public float Amplitude = 4f;     // pixels
    [Export] public float Frequency = 0.1f;     // cycles per second

    private float _baseY;
    private double _t;

    public override void _Ready()
    {
        _baseY = Position.Y;
    }

    public override void _Process(double delta)
    {
        _t += delta;
        var yOffset = Amplitude * Mathf.Sin((float)(_t * Frequency * Mathf.Tau));
        Position = new Vector2(Position.X, _baseY + yOffset);
    }
}
