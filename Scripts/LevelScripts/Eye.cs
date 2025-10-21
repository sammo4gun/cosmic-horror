using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

// SCENE_ID: Eye
// 
public partial class Eye : Shuttle
{
    public bool TriggeredConsole = false;
    public bool RecordRunning = false;
    public bool finalCutSceneStarted = false;

    public override void _Ready()
    {
        base._Ready();

        _window.SetWindow("Stars");
        _window.SetSpinning(100f);

        _camera.setDarkness(0.4f);

        // starting time, distance, and speed
        _timeHandler.StartTimer(DateTime.ParseExact("06-11-1998 08:52:13.000", "dd-MM-yyyy HH:mm:ss.FFF", null));
        _spaceHandler.StartDistance(11_055_342_009f);
        Speed = 15f;

        _console.ToggleActivateButton("Hibernation", false); // so we can't hibernate right away.
        _console.ToggleButtonPressed("Hibernation", true, silent: true); // so the hibernation button is off
        _console.ToggleButtonPressed("BackupRight", true, silent: true); // to set the backup to being used
        _console.ToggleButtonPressed("BackupLeft", true, silent: true); // to set the backup to being used
        _console.ToggleActivateButton("Launch", false); // so we can't launch right away.
        _console.ToggleButtonPressed("Launch", true, silent: true); // to set the launch to being used
        _console.ToggleActivateButton("A", false); // so we can't launch right away.
        _console.ToggleActivateButton("B", false); // so we can't launch right away.
        _console.ToggleActivateButton("C", false); // so we can't launch right away.
        _console.ToggleActivateButton("D", false); // so we can't launch right away.
        _console.ToggleActivateButton("E", false); // so we can't launch right away.
        _console.ToggleActivateButton("1", false); // so we can't launch right away.
        _console.ToggleActivateButton("2", false); // so we can't launch right away.
        _console.ToggleActivateButton("3", false); // so we can't launch right away.
        _console.ToggleActivateButton("4", false); // so we can't launch right away.
        _console.ToggleActivateButton("5", false); // so we can't launch right away.
        _recordPlayer.LoadSong(8, repeated: false, loadBar: false);

        _ = _hibernationHandler.EndHibernation(delay: 1.5f, speedFactor: 6);
        // _ = _hibernationHandler.EndHibernation(delay: 0f, speedFactor: 1);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_camera.FacingConsole)
        {
            if (!TriggeredConsole) TriggerConsole();
        }
    }

    public async void TriggerConsole()
    {
        TriggeredConsole = true;

        await ToSignal(GetTree().CreateTimer(4f), "timeout");

        _console.SetTextDisplaySpeed(0.5f);
        _console.ToggleRaiseText();
        //Character line(ish)
        //                  |                                      |
        _console.OutputLine("✱{p=0.5}✱{p=0.5}E{p=0.5}R{p=0.5}R{p=0.5}O{p=0.5}R{p=0.5}✱{p=0.5}✱{p=2.5}");
        _console.OutputLine("✱{p=0.5}✱{p=0.5}E{p=0.5}R{p=0.5}R{p=0.5}O{p=0.5}R{p=0.5}✱{p=0.5}✱{p=2.5}");
        _console.OutputLine("G{p=0.5}o{p=0.5}l{p=0.5}d{p=0.5}e{p=0.5}n{p=0.5} {p=0.5} d{p=0.5}r{p=0.5}i{p=0.5}v{p=0.5}e{p=0.5}  {p=0.5}d{p=0.5}a{p=0.5}m{p=0.5}a{p=0.5}g{p=0.5}e{p=0.5}d{p=2.5}");
    
        await ToSignal(_console, "TextFinished");
        _recordPlayer.Disabled = false;
        while (!finalCutSceneStarted)
        {
            _console.OutputLine(" {p=0.5}");
            _console.OutputLine(" {p=0.5}");
            _console.OutputLine("✱{p=0.5}✱{p=0.5}H{p=0.5}E{p=0.5}L{p=0.5}P{p=0.5} {p=0.5}M{p=0.5}E{p=0.5}✱{p=0.5}✱{p=2.5}");
            await ToSignal(_console, "TextFinished");
        }
    }

    public override void RecordStarted()
    {
        if (!finalCutSceneStarted)
        {
            finalCutSceneStarted = true;
            runFinalCutscene();
        }
    }

    public async void runFinalCutscene()
    {
        _window.ShowBlot(true);
        _window.SetBlotPos(new Vector2(274, 272));
        _window.MoveBlot(new Vector2(-66, -76), 0.02f);
        _window.ScaleBlot(5.0f, 0.015f);
        await ToSignal(GetTree().CreateTimer(32f), "timeout");
        _camera.Turn("left");

        await ToSignal(GetTree().CreateTimer(13f), "timeout");
        _camera.setDarkness(1f);
        _timeHandler.AddTime(5, "years");
        _timeHandler.AddTime(4, "months");
        _timeHandler.AddTime(13, "days");
        await ToSignal(GetTree().CreateTimer(0.2f), "timeout");

        _camera.setDarkness(0.5f);

        await ToSignal(GetTree().CreateTimer(9f), "timeout");
        _camera.setDarkness(1f);
        _timeHandler.AddTime(8, "years");
        _timeHandler.AddTime(7, "months");
        _timeHandler.AddTime(1, "days");
        await ToSignal(GetTree().CreateTimer(0.2f), "timeout");

        _camera.setDarkness(0.6f);

        await ToSignal(GetTree().CreateTimer(3f), "timeout");
        _camera.setDarkness(1f);
        _timeHandler.AddTime(13, "years");
        _timeHandler.AddTime(7, "months");
        _timeHandler.AddTime(1, "days");
        await ToSignal(GetTree().CreateTimer(0.2f), "timeout");

        _camera.setDarkness(0.7f);

        _console.DisableDisplays();

        await ToSignal(GetTree().CreateTimer(5f), "timeout");

        _console.OminousGlow = true;
        
        await ToSignal(GetTree().CreateTimer(5f), "timeout");

        _camera.MoveSpeed = 2;
        _camera.Turn("right");

        await ToSignal(GetTree().CreateTimer(5f), "timeout");
        _soundScapeHandler.PlayFinalSong();
    }

    public override void RecordDone()
    {
        
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("left") && !finalCutSceneStarted)
        {
            _camera.Turn("left");
        }
        if (@event.IsActionPressed("right") && !finalCutSceneStarted)
        {
            _camera.Turn("right");
        }
    }
}