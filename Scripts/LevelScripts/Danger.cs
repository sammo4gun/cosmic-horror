using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

// SCENE_ID: Danger
// Window: The player sees earth moving away far away from the shuttle, with the moon in the shot
// 
public partial class Danger : Shuttle
{
    public bool TriggeredConsole = false;
    public bool TriggeredDials = false;
    public bool CourseCorrected = false;
    public bool LaunchCodesEntered = false;

    public override void _Ready()
    {
        base._Ready();

        _window.SetWindow("Stars");

        // starting time, distance, and speed
        _timeHandler.StartTimer(DateTime.ParseExact("30-10-1981 00:23:14.000", "dd-MM-yyyy HH:mm:ss.FFF", null));
        _spaceHandler.StartDistance(1_517_243_000f);
        Speed = 21f;

        _camera.setDarkness(0.1f);

        _console.ToggleActivateButton("Hibernation", false); // so we can't hibernate right away.
        _console.ToggleButtonPressed("Hibernation", true, silent: true); // sothe hibernation button is off
        _console.ToggleButtonPressed("BackupRight", true, silent: true); // to set the backup to being used
        _recordPlayer.LoadSong(6, repeated: true, loadBar: false);

        _ = _hibernationHandler.EndHibernation(delay: 1.5f, speedFactor: 4);
        // _ = _hibernationHandler.EndHibernation(delay: 0f, speedFactor: 1);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_camera.FacingConsole) if (!TriggeredConsole) TriggerConsole();
    }

    public async void TriggerConsole()
    {
        TriggeredConsole = true;

        await ToSignal(GetTree().CreateTimer(1f), "timeout");

        _console.ToggleRaiseText();
        //Character line(ish)
        //                  |                                      |
        _console.OutputLine("Bootsys v95.2.5");
        _console.OutputLine("Initialising \"Voyager1\"");
        _console.OutputLine("ERR - Unexpected t_wakeup{p=2.0}");
        _console.OutputLine("Hibernation_length=39 days{p=1.0}");
        _console.OutputLine("Verifying {p=0.3}. . . . . . . . . . . {p=0.5}. {p=0.3}. {p=0.3}.");
        _console.OutputLine("Verification complete");
        _console.OutputLine("Boot successful");
        _console.OutputLine("SYSERR - failed to save logs");
        await ToSignal(_console, "TextFinished");

        _console.RadioAlert(true);
        _recordPlayer.Disabled = false;
        _console.OutputLine("ALERT! Message received");
        _console.OutputLine("Dated 130hrs - broadcaster active");

    }

    public override void RecordStarted()
    {
        if (!TriggeredDials)
        {
            TriggeredDials = true;
            _console.RadioAlert(false);
            _console.OutputLine("Course misalignment confirmed");
            _console.OutputLine("Adjust course appropriately");
            _console.OutputLine("Confirm course correct?");
            _console.ToggleActivateDials(true);
            _console.RequestInput();
        }
    }

    public async void RequestLaunchcodeCheck()
    {
        await ToSignal(_console, "TextFinished");
        _recordPlayer.StopPlaying();

        _console.OutputLine("Loading post_orbit_checklog.yaml{p=1.0}");
        _console.OutputLine("===============================");
        _console.OutputLine("Pre-hibernation checklog");
        _console.OutputLine("┗╸Enter thruster sequence{p=1.0}");
        _console.OutputLine("===============================");
        _console.OutputLine("Thruster sequence loaded");
        _console.OutputLine("┗╸Confirm sequence E-4-5"); ;
        _console.LaunchCodes = "E45";
    }

    public override void LaunchCodesEnteredHandler(bool correct, bool shuffled)
    {
        if (correct)
        {
            _console.OutputLine("Thruster Sequence received");
            LaunchCodesEntered = true;
            AllDoneOutput();
        }
        else if (shuffled)
        {
            _console.OutputLine("Thruster sequence received");
            _console.OutputLine("Ordering incorrect");
            _console.OutputLine("Awaiting further instruction");
        }
        else
        {
            _console.OutputLine("Thruster sequence incorrect");
            _console.OutputLine("Awaiting further instruction");
        }
    }

    public void AllDoneOutput()
    {
        _console.OutputLine("Completed pre-launch checklog");
        _console.OutputLine("==============================={p=1.0}");
        _console.OutputLine("Hibernation module load successful");
        _console.OutputLine("target = interstellar_space");
        _console.OutputLine("hibernation time ~8 months");
        _console.OutputLine("Confirm hibernation?");
        _console.RequestInput();
    }

    public override void InputReceivedHandler(string question, string input)
    {
        if (question == "Confirm course correct?")
        {
            _console.ToggleActivateDials(false);
            float leftValue = _console.getDialValue("left");
            float rightValue = _console.getDialValue("right");
            string leftFormatted = string.Format("{0:F1}", leftValue);
            string rightFormatted = string.Format("{0:F1}", rightValue);
            if (leftValue >= 0) leftFormatted = "+" + leftFormatted;
            if (rightValue >= 0) rightFormatted = "+" + rightFormatted;
            _console.OutputLine("Course bearing set to");
            _console.OutputLine($"┣╸Left: {leftFormatted}°");
            _console.OutputLine($"┗╸Right: {rightFormatted}°{{p=2.0}}");
            _console.OutputLine("Error simulating course{p=2.0}");
            _console.OutputLine("Confirm course bearing? (y/n)");
            _console.RequestInput();
        }
        if (question == "Confirm course bearing? (y/n)")
        {
            if (input.ToLower() == "y")
            {
                _console.OutputLine("New bearing confirmed{p=2.0}");
                RequestLaunchcodeCheck();
            }
            else
            {
                _console.ToggleActivateDials(true);
                _console.OutputLine("==========================");
                _console.OutputLine("Cancelled course correct");
                _console.OutputLine("Adjust course appropriately");
                _console.OutputLine("Confirm course correct?");
                _console.RequestInput();
            }
        }
        if (question == "Confirm hibernation?")
        {
            _console.ToggleRaiseText();
            _console.ToggleActivateButton("Hibernation", true);
            _console.ToggleButtonPressed("Hibernation", false, silent: true);
        }
    }

    public override void ButtonPressed(string buttonName, bool toggled)
    {
        if (buttonName == "Hibernation" && toggled) _ = _hibernationHandler.EnterHibernation("LevelScenes/7_asteroids");
    }
}
