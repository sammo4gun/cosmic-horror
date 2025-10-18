using Godot;
using System;
using System.Collections.Generic;

// SCENE_ID: Departing_Earth
// Window: The player sees earth moving away very slowly, starting out very close to the ground
// Get used to the basics: the launch code, and the french integrity check.
// Blast forwards 1 week!
public partial class Shuttle : Node2D
{
    protected Camera _camera;
    protected Console _console;
    protected TimeHandler _timeHandler;
    protected SpaceHandler _spaceHandler;
    protected HibernationHandler _hibernationHandler;
    protected SoundScapeHandler _soundScapeHandler;
    protected RecordPlayer _recordPlayer;

    public DateTime CurrentTime => _timeHandler.CurrentTime;
    public float DistanceFromEarth => _spaceHandler.DistanceFromEarth;
    public bool Hibernating => _hibernationHandler.IsHibernating;

    public float Speed = 15.0f; // in km/s (this may need to be updated)

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
        _console.LaunchCodesEntered += LaunchCodesEnteredHandler;
        _console.InputReceived += InputReceivedHandler;
    }

    public override void _Input(InputEvent @event)
    {
        HandleTurning(@event);
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

    public virtual void LaunchCodesEnteredHandler(bool correct, bool shuffled) { }

    public virtual void InputReceivedHandler(string question, string input) { }

    public virtual void RecordStarted() { }

    public virtual void RecordDone() { }

    public virtual void ButtonPressed(string buttonName, bool toggled) { }
}
