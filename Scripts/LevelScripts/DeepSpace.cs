using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

// SCENE_ID: Deep Space
// 
public partial class DeepSpace : Shuttle
{
    public bool TriggeredConsole = false;
    public bool RecordRunning = false;
    public bool RecordChecked = false;

    public override void _Ready()
    {
        base._Ready();

        _window.SetWindow("Stars");
        _window.SetSpinning(300f);
        _window.ShowBlot(true);
        _window.SetBlotPos(new Vector2(467f, 61f));
        _window.MoveBlot(new Vector2(166f, 250f), 0.01f);
        _window.ScaleBlot(1.5f, 0.002f);

        _camera.setDarkness(0.3f);

        // starting time, distance, and speed
        _timeHandler.StartTimer(DateTime.ParseExact("03-08-1987 19:55:13.000", "dd-MM-yyyy HH:mm:ss.FFF", null));
        _spaceHandler.StartDistance(5_235_342_009f);
        Speed = 15f;

        _console.ToggleActivateButton("Hibernation", false); // so we can't hibernate right away.
        _console.ToggleButtonPressed("Hibernation", true, silent: true); // sothe hibernation button is off
        _console.ToggleButtonPressed("BackupRight", true, silent: true); // to set the backup to being used
        _console.ToggleButtonPressed("BackupLeft", true, silent: true); // to set the backup to being used
        _console.ToggleActivateButton("Launch", false); // so we can't launch right away.
        _console.ToggleButtonPressed("Launch", true, silent: true); // to set the launch to being used
        _recordPlayer.LoadSong(7, repeated: false, loadBar: true);

        _ = _hibernationHandler.EndHibernation(delay: 1.5f, speedFactor: 4);
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
        _console.OutputLine("✱✱ERROR✱✱");
        _console.OutputLine("Bootsys v95.2.5");
        _console.OutputLine("Initialising \"Voyager1\"");
        _console.OutputLine("Hibernation_length=SYSERR years{p=1.0}");
        _console.OutputLine("ERR - Cannot verify datetime{p=1.0}");
        _console.OutputLine("Boot failed{p=1.0}");
        _console.OutputLine("✱✱ERROR✱✱");
        _recordPlayer.Disabled = false;
        while (!RecordRunning)
        {
            _console.OutputLine("Boot failed{p=1.0}");
            _console.OutputLine("✱✱ERROR✱✱");
            await ToSignal(_console, "TextFinished");
        }

        while (!RecordChecked)
        {
            _console.OutputLine("✱✱ERning integrity diag000");
            _console.OutputLine("✱✱EROR✱✱");
            await ToSignal(_console, "TextFinished");
        }
        
        _console.OutputLine("=======================");
        _console.OutputLine("Integrity diagnostics completed");
        _console.OutputLine("MISSING - catastrophic data leakage");
        _console.OutputLine("MISSING - files corrupted");
        _console.OutputLine("please contact a system admin{p=3.0}");
        _console.OutputLine("┗╸✱✱EROR✱✱uence A-A-A-A-A");
        await ToSignal(_console, "TextFinished");
        _console.LaunchCodes = "AAAAA";
        _console.ToggleActivateButton("Launch", true); // so we can't launch right away.
        _console.ToggleButtonPressed("Launch", false, silent: false); // to set the launch to being used
    }

    public override void LaunchCodesEnteredHandler(bool correct, bool shuffled)
    {
        AllDoneOutput();
    }

    public override void RecordStarted()
    {
        RecordRunning = true; 
    }

    public override void RecordDone()
    {
        RecordChecked = true;
    }

    public override void InputReceivedHandler(string question, string input)
    {
        if (question == "Confirm ✱✱ERROR✱✱?")
        {
            _console.ToggleRaiseText();
            _console.ToggleActivateButton("Hibernation", true);
            _console.ToggleButtonPressed("Hibernation", false, silent: true);
        }
    }

    public void AllDoneOutput()
    {
        _console.OutputLine("Completed pre-launch checklog");
        _console.OutputLine("==============================={p=1.0}");
        _console.OutputLine("Hibernation module load successful");
        _console.OutputLine("target = ???");
        _console.OutputLine("hibernation time ~SYSERR months");
        _console.OutputLine("Confirm ✱✱ERROR✱✱?");
        _console.RequestInput();
    }

    public override void ButtonPressed(string buttonName, bool toggled)
    {
        if (buttonName == "Hibernation" && toggled) _ = _hibernationHandler.EnterHibernation("LevelScenes/9_eye");
    }
}