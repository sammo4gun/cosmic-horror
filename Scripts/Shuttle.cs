using Godot;
using System;

public partial class Shuttle : Node2D
{
    private Camera _camera;
    private Console _console;

    public override void _Ready()
    {
        base._Ready();
        _camera = GetNode<Camera>("Camera");
        _console = GetNode<Console>("Console");
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
        }
    }
}
