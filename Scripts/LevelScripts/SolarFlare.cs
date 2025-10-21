using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

// SCENE_ID: Solar Flare
// 
public partial class SolarFlare : Shuttle
{
    public bool TriggeredConsole = false;
    public bool LaunchCodesEntered = false;
    public bool RecordChecked = false;

    public override void _Ready()
    {
        base._Ready();

        _window.SetWindow("SolarFlare");

        // starting time, distance, and speed
        _timeHandler.StartTimer(DateTime.ParseExact("21-09-1981 09:23:14.000", "dd-MM-yyyy HH:mm:ss.FFF", null));
        _spaceHandler.StartDistance(1_458_491_009f);
        Speed = 15f;

        _console.ToggleActivateButton("Hibernation", false); // so we can't hibernate right away.
        _console.ToggleButtonPressed("Hibernation", true, silent: true); // sothe hibernation button is off
        // _console.ToggleButtonPressed("BackupLeft", true, silent: true); // to set the backup to being used
        _recordPlayer.LoadSong(5, repeated: false, loadBar: true);

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

        await ToSignal(GetTree().CreateTimer(1f), "timeout");

        _console.ToggleRaiseText();
        //Character line(ish)
        //                  |                                      |
        _console.OutputLine("Bootsys v95.2.5");
        _console.OutputLine("Initialising \"Voyager1\"");
        _console.OutputLine("Hibernation_length=39 days{p=1.0}");
        _console.OutputLine("Verifying {p=0.3}. {p=0.8}. {p=0.8}. . . {p=0.5}. {p=0.5}. . . . . . . .{p=1.0}");
        _console.OutputLine("Verification complete{p=1.0}");
        _console.OutputLine("Boot successful");
        _console.OutputLine("System load logged in usr/logs/290119800229130.json");
        _console.OutputLine("Running preliminary diagnostics...{p=0.2}");
        _console.OutputLine("Hull integrity - 45% - WARNING");
        _console.OutputLine("Battery cell total charge - 101%");
        _console.OutputLine("Solar flare detected - charging");
        _console.OutputLine("THRUSTER1 - Operational");
        _console.OutputLine("THRUSTER2 - Operational");
        _console.OutputLine("Velocity - 13.41km/s - STABLE");
        _console.OutputLine("Run advanced diagnostics? (y/n)");
        _console.RequestInput();
    }

    public override void InputReceivedHandler(string question, string input)
    {
        if (question == "Run advanced diagnostics? (y/n)")
        {
            if (input.ToLower() == "y")
            {
                //Character line(ish)
                //                  |                                      |
                _console.OutputLine("Running usr/sys/advanced_diag.sh");
                _console.OutputLine("===============================");
                _console.OutputLine("Postlaunch tests:");
                _console.OutputLine("Running . {p=0.3}W W W . {p=0.5}. {p=0.5}. {p=0.5}E . . . {p=1.0}W W W");
                _console.OutputLine("FAILED with 6 warning and 1 errors");
                _console.OutputLine("Transmission receiver damaged");
                _console.OutputLine("Simulating trajectory... {p=0.8}");
                _console.OutputLine("Trajectory Outline Confirmed");
                _console.OutputLine("Interstellar entry t-minus:{p=0.8}");
                _console.OutputLine("┣╸#ERR days{p=0.8}");
                _console.OutputLine("┣╸#ERR months{p=0.8}");
                _console.OutputLine("┗╸#ERR years{p=0.3}");
                _console.OutputLine("Backup thruster checks");
                _console.OutputLine("1/2 available");
                _console.OutputLine("===============================");
                RequestDriveCheck();

            }
            else if (input.ToLower() == "n")
            {
                _console.OutputLine("Skipping advanced diagnostics");
                RequestDriveCheck();
            }
            else
            {
                _console.OutputLine("Err: Input not recognized\n>   Expected y/n response", noquestion: true);
                _console.RequestInput();
            }
        }
        else if (question == "Confirm hibernation?")
        {
            _console.ToggleRaiseText();
            _console.ToggleActivateButton("Hibernation", true);
            _console.ToggleButtonPressed("Hibernation", false, silent: true);
        }
    }

    public void RequestDriveCheck()
    {
        _recordPlayer.Disabled = false;
        _console.OutputLine("Loading solar_charge_checklog.yaml{p=1.0}");
        _console.OutputLine("===============================");
        _console.OutputLine("Pre-hibernation checklog");
        _console.OutputLine("┣╸Run Golden Drive integrity test");
        _console.OutputLine("┗╸Enter thruster sequence{p=1.0}");
        _console.OutputLine("===============================");
        _console.OutputLine("Golden Drive activated, awaiting run");
        _console.OutputLine("Thruster sequence loaded");
        _console.OutputLine("┗╸Confirm sequence C-A-3");
        _console.LaunchCodes = "CA3";
    }

    public override void LaunchCodesEnteredHandler(bool correct, bool shuffled)
    {
        if (correct)
        {
            _console.OutputLine("Thruster Sequence received");
            LaunchCodesEntered = true;
            if (RecordChecked) AllDoneOutput();
            else _console.OutputLine("Awaiting drive integrity test");
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

    public override void RecordStarted()
    {
        // _console.OutputLine("Thruster sequence loaded.");
        // _console.OutputLine("Confirm sequence C-2-E.");
        _console.OutputLine("Running integrity diagnostics...");
    }

    public override void RecordDone()
    {
        _console.OutputLine("Integrity diagnostics completed");
        _console.OutputLine("Data leakage detected");
        _console.OutputLine("Golden Drive integrity");
        _console.OutputLine(" - Estimated 75%");
        RecordChecked = true;
        if (LaunchCodesEntered) AllDoneOutput();
        else _console.OutputLine("Awaiting thruster sequence...");
    }


    public override void ButtonPressed(string buttonName, bool toggled)
    {
        if (buttonName == "Hibernation" && toggled) _ = _hibernationHandler.EnterHibernation("LevelScenes/6_danger");
    }

    public void AllDoneOutput()
    {
        _console.OutputLine("Completed pre-launch checklog");
        _console.OutputLine("==============================={p=1.0}");
        _console.OutputLine("Hibernation module load successful");
        _console.OutputLine("target = interstellar_space");
        _console.OutputLine("hibernation time ~9 months");
        _console.OutputLine("Confirm hibernation?");
        _console.RequestInput();
    }
}