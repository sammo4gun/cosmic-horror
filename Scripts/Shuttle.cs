using Godot;
using System;

public partial class Shuttle : Node2D
{
    private Camera _camera;
    private Console _console;
    private TimeHandler _timeHandler;
    private SpaceHandler _spaceHandler;
    private HibernationHandler _hibernationHandler;

    public DateTime CurrentTime => _timeHandler.CurrentTime;
    public float DistanceFromEarth => _spaceHandler.DistanceFromEarth;
    public bool Hibernating => _hibernationHandler.IsHibernating;

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
        _hibernationHandler = GetNode<HibernationHandler>("HibernationHandler");
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
            _console.OutputLine("VOY01 - Booting systems...");
            _console.OutputLine("Standby for further instructions.");
            // _hibernationHandler.EnterHibernation(1, "years", 315_600_000); // 1 year
        }
        if (@event.IsActionPressed("text_test"))
        {
            _console.ToggleRaiseText();
            // _console.OutputLine("01\n\n\n\n\n\n\n\n\n59879");
            // _hibernationHandler.EnterHibernation(1, "years", 315_600_000); // 1 year
        }
    }
}
