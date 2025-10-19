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

    public float CorrectLeftBearing = 25.0f;
    public float CorrectRightBearing = -19.5f;

    public override void _Ready()
    {
        base._Ready();

        _window.SetWindow("SolarFlare");

        // starting time, distance, and speed
        _timeHandler.StartTimer(DateTime.ParseExact("30-10-1981 00:23:14.000", "dd-MM-yyyy HH:mm:ss.FFF", null));
        _spaceHandler.StartDistance(1_458_491_009f);
        Speed = 15f;

        // _console.ToggleButtonPressed("BackupLeft", true, silent: true); // to set the backup to being used
        _recordPlayer.LoadSong(6, repeated: true, loadBar: false);

        _ = _hibernationHandler.EndHibernation(delay: 1.5f, speedFactor: 4);
        // _ = _hibernationHandler.EndHibernation(delay: 0f, speedFactor: 1);
        
        // we get started right away
        _console.RadioAlert(true);
        _recordPlayer.Disabled = false;
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
        _console.OutputLine("Verifying {p=0.3}. . . . . . . . . . . {p=0.5}. {p=0.3}. {p=0.3}.");
        _console.OutputLine("Verification complete");
        _console.OutputLine("Boot successful");
        _console.OutputLine("System load logged in usr/logs/080919772203560.json");
        _console.OutputLine("Running preliminary diagnostics...{p=0.2}");
        _console.OutputLine("Hull integrity - 98% - PASS");
        _console.OutputLine("Battery cell total charge - 93%");
        _console.OutputLine("THRUSTER1 - Operational");
        _console.OutputLine("THRUSTER2 - Operational");
        _console.OutputLine("Velocity - 17.68km/s - STABLE");
        _console.OutputLine("Preparing to vacate deep orbit...");

        await ToSignal(_console, "TextFinished");

        _console.RadioAlert(true);
        _recordPlayer.Disabled = false;
        _console.OutputLine("ALERT! Message received");
        _console.OutputLine("Dated 32hrs - Listen on broadcaster");
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
        _console.OutputLine("┗╸Confirm sequence D-C-A-3"); ;
        _console.LaunchCodes = "DCA3";
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
        _console.OutputLine("target = jupiter_orbit");
        _console.OutputLine("hibernation time ~26 months");
        _console.OutputLine("Confirm hibernation?");
        _console.RequestInput();
    }

    public override void InputReceivedHandler(string question, string input)
    {
        if (question == "Confirm course correct?")
        {
            float leftValue = _console.getDialValue("left");
            float rightValue = _console.getDialValue("right");
            string leftFormatted = string.Format("{0:F1}", leftValue);
            string rightFormatted = string.Format("{0:F1}", rightValue);
            if (leftValue >= 0) leftFormatted = "+" + leftFormatted;
            if (rightValue >= 0) rightFormatted = "+" + rightFormatted;
            if (leftValue == CorrectLeftBearing && rightValue == CorrectRightBearing)
            {
                _console.ToggleActivateDials(false);
                _console.OutputLine($"Course bearing set to \n>┣╸Left: {leftFormatted}°\n>┗╸Right: {rightFormatted}°{{p=1.0}}");
                _console.OutputLine("New bearing confirmed{p=2.0}");
                RequestLaunchcodeCheck();
            }
            else
            {
                _console.OutputLine($"Course bearing set to \n>┣╸Left: {leftFormatted}°\n>┗╸Right: {rightFormatted}°{{p=1.0}}\n>Error in estimating course \n>Please re-confirm course bearing", noquestion: true);
                _console.RequestInput();
                
            }
        }
        if (question == "Confirm hibernation?")
        {
            _console.ToggleRaiseText();
            _console.ToggleActivateButton("Hibernation", true);
        }
    }

    public override void ButtonPressed(string buttonName, bool toggled)
    {
        if (buttonName == "Hibernation" && toggled) _ = _hibernationHandler.EnterHibernation("LevelScenes/3_jupiter");
    }
}
