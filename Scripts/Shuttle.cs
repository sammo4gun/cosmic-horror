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
    private SoundScapeHandler _soundScapeHandler;

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
        _soundScapeHandler = GetNode<SoundScapeHandler>("SoundScapeHandler");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("left"))
        {
            _camera.Turn("left");
            _console.SetLightState("1", true); // Example of setting a light on
        }
        if (@event.IsActionPressed("right"))
        {
            _camera.Turn("right");
        }
        if (@event.IsActionPressed("input_test"))
        {
            // _hibernationHandler.EnterHibernation(1, "years", 315_600_000); // 1 year
        }
        if (@event.IsActionPressed("text_test"))
        {
            _console.ToggleRaiseText();
            if (!(_console.LaunchCodes is string)) 
            {
                _console.OutputLine("VOY01 - Booting systems...");
                _console.OutputLine("THRUS1 - Operational.");
                _console.OutputLine("THRUS2 - Operational.");
                _console.OutputLine("RENDEZVOUS SAT1 - 0.34u87. Nominal trajectory.");
                _console.OutputLine("Estimed time to Mars orbit: 7 months.");
                _console.OutputLine("Launch code E41A.");
                _console.LaunchCodes = "E41A";
            }
        }
    }

    public void LaunchCodesEntered(bool correct, bool shuffled)
    {
        if (correct) _console.OutputLine("Launch code received. Ready for takeoff. Psheeewwww!!!");
        else if (shuffled) _console.OutputLine("Incorrect ordering on launch codes. Holding off on launch.");
        else _console.OutputLine("Launch codes incorrect. Awaiting instruction.");
    }

    public void ReceiveInput(string question, string input)
    {
        if (question == "Engage thrusters and activate system? (Y/N)")
        {
            if (input.ToLower() == "y" || input.ToLower() == "yes")
            {
                if (_console.AreButtonsPressed("E41A", exact: true))
                {
                    _console.OutputLine("Affirmative. All systems active.", noquestion: true);
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
