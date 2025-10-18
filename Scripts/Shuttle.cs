using Godot;
using System;
using System.Collections.Generic;

// SCENE_ID: Departing_Earth
// Window: The player sees earth moving away very slowly, starting out very close to the ground
// Get used to the basics: the launch code, and the french integrity check.
// Blast forwards 1 week!
public partial class Shuttle : Node2D
{
    private Camera _camera;
    private Console _console;
    private TimeHandler _timeHandler;
    private SpaceHandler _spaceHandler;
    private HibernationHandler _hibernationHandler;
    private SoundScapeHandler _soundScapeHandler;
    private RecordPlayer _recordPlayer;

    public DateTime CurrentTime => _timeHandler.CurrentTime;
    public float DistanceFromEarth => _spaceHandler.DistanceFromEarth;
    public bool Hibernating => _hibernationHandler.IsHibernating;

    public float Speed = 15.0f; // in km/s (this may need to be updated)
    // define starting time
    // define starting distance

    public bool TriggeredConsole = false;

    public override void _Ready()
    {
        base._Ready();
        _camera = GetNode<Camera>("Camera");
        _console = GetNode<Console>("Console");
        _timeHandler = GetNode<TimeHandler>("TimeHandler");
        _spaceHandler = GetNode<SpaceHandler>("SpaceHandler");
        _hibernationHandler = GetNode<HibernationHandler>("HibernationHandler");
        _soundScapeHandler = GetNode<SoundScapeHandler>("SoundScapeHandler");
        _recordPlayer = GetNode<RecordPlayer>("Window/RecordPlayer");
        _recordPlayer.MusicStarted += RecordStarted;
        _recordPlayer.MusicDone += RecordDone;
        _console.ButtonPressed += ButtonPressed;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_camera.FacingConsole)
        {
            if (!TriggeredConsole) TriggerConsole();
        }
    }

    public override void _Input(InputEvent @event)
    {
        HandleTurning(@event);
    }

    public async void TriggerConsole()
    {
        TriggeredConsole = true;

        await ToSignal(GetTree().CreateTimer(2f), "timeout");

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
            _recordPlayer.Disabled = false;
            _console.RadioAlert(true);
        }
    }

    public void HandleTurning(InputEvent @event)
    {
        if (@event.IsActionPressed("left"))
        {
            _camera.Turn("left");
        }
        if (@event.IsActionPressed("right"))
        {
            _camera.Turn("right");
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

    public void RecordStarted()
    {
        _console.RadioAlert(false);
    }

    public void RecordDone()
    {
        _console.OutputLine($"Validation complete.");
        _console.OutputLine($"Golden Drive Integrity at 100%.");
    }

    public void ButtonPressed(string buttonName, bool toggled)
    {
        if (buttonName == "Hibernation" && toggled)
        {
            _ = _hibernationHandler.EnterHibernation("Shuttle");
        }
    }
}
