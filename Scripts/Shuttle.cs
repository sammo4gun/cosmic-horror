using Godot;
using System;

public partial class Shuttle : Node2D
{
    private Camera _camera;
    private Console _console;
    private TimeHandler _timeHandler;
    private SpaceHandler _spaceHandler;

    public DateTime CurrentTime => _timeHandler.CurrentTime;
    public float DistanceFromEarth => _spaceHandler.DistanceFromEarth;

    public float Speed = 10.0f; // in km/s (this may need to be updated)
    public float SpinTilt = 0.0f; // degrees
    public float HeightTilt = 0.0f; // degrees

    public override void _Ready()
    {
        base._Ready();
        _camera = GetNode<Camera>("Camera");
        _console = GetNode<Console>("Console");
        _timeHandler = GetNode<TimeHandler>("TimeHandler");
        _spaceHandler = GetNode<SpaceHandler>("SpaceHandler");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("left"))
        {
            _camera.Turn("left");
        }
        if (@event.IsActionPressed("right"))
        {
            _camera.Turn("right");
        }
        if (@event.IsActionPressed("input_test"))
        {
            _console.OutputLine("0159879");
            _timeHandler.AddTime(1, "years");
            _spaceHandler.AddDistance(1, "years", SpinTilt, HeightTilt);
        }
    }
}
