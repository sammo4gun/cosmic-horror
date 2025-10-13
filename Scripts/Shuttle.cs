using Godot;
using System;
using System.Collections.Generic;

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
            _console.OutputLine("THRUS1 - Operational.");
            _console.OutputLine("THRUS2 - Operational.");
            _console.OutputLine("RENDEZVOUS SAT1 - 0.34u87. Nominal trajectory.");
            _console.OutputLine("Estimed time to Mars orbit: 7 months.");
            _console.OutputLine("Boot code E41A.");
            _console.OutputLine("Engage thrusters and activate system? (Y/N)");
            _console.RequestInput();
            // _hibernationHandler.EnterHibernation(1, "years", 315_600_000); // 1 year
        }
        if (@event.IsActionPressed("text_test"))
        {
            _console.ToggleRaiseText();
        }
    }

    public void ReceiveInput(string question, string input)
    {
        if (question == "Engage thrusters and activate system? (Y/N)")
        {
            if (input.ToLower() == "y" || input.ToLower() == "yes")
            {
                if (_console.IsButtonPressed("A") && _console.IsButtonPressed("E") && _console.IsButtonPressed("1") && _console.IsButtonPressed("4"))
                {
                    _console.OutputLine("Affermative. All systems active.", noquestion: true);
                    return;
                }
                _console.OutputLine("Incorrect Launch sequence.", noquestion: true);
                _console.RequestInput();
            }
            else if (input.ToLower() == "n" || input.ToLower() == "no")
            {
                _console.OutputLine("Negative. Awaiting feedback.");
            }
            else
            {
                _console.OutputLine("Input not recognized. Please respond with Y or N.", noquestion: true);
                _console.RequestInput();
                return;
            }
        }
        else
        {
            _console.OutputLine($"Random Input received: {input}");
        }
    }
}
