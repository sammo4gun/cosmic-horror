using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

// SCENE_ID: Asteroids
// Window: The player sees saturn,and a bunch of ominous rocks....
// 
public partial class Asteroids : Shuttle
{
    public bool TriggeredConsole = false;
    public bool enteredSafeLaunchCode = false;
    public bool BackupInstalled = false;
    public bool LaunchCodesEntered = false;


    private RandomNumberGenerator rng = new RandomNumberGenerator();

    public override void _Ready()
    {
        base._Ready();

        _window.SetWindow("Stars");
        _window.SetAsteroidsVisible(true, 1);

        _camera.setDarkness(0.2f);

        _console.ToggleActivateButton("Hibernation", false); // so we can't hibernate right away.
        _console.ToggleButtonPressed("Hibernation", true, silent: true); // sothe hibernation button is off
        _console.ToggleButtonPressed("BackupRight", true, silent: true); // to set the backup to being used
        // starting time, distance, and speed
        _timeHandler.StartTimer(DateTime.ParseExact("24-06-1982 12:05:59.000", "dd-MM-yyyy HH:mm:ss.FFF", null));
        _spaceHandler.StartDistance(2_315_487_315f);
        Speed = 21f;

        _recordPlayer.LoadSong(1, repeated: false, loadBar: false);

        _ = _hibernationHandler.EndHibernation(delay:1.5f, speedFactor: 4);
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
        _console.OutputLine("Hibernation_length=8 months{p=1.0}");
        _console.OutputLine("Verifying {p=0.3}. . . . . . . . . . . {p=0.5}. {p=0.3}. {p=0.3}.");
        _console.OutputLine("Verification complete");
        _console.OutputLine("Boot successful");
        _console.OutputLine("SYSERR - failed to save logs");
        _console.OutputLine("WARNING - High speed objects nearby");
        _console.OutputLine("Recommend course correct{p=2.0}");
        _console.OutputLine("Continue course or");
        await ToSignal(_console, "TextFinished");

        HandleCrash();
    }

    public async void HandleCrash()
    {
        _camera.ApplyShake(60f, 6f);
        _camera.Emergency = true;
        _soundScapeHandler.Crash();
        _window.SetAsteroidsVisible(false);
        _window.SetSpinning(10000f);
        _window.DeleteAllAsteroids();

        _console.OutputLine("**************************");
        _console.OutputLine("CRITICAL FAILURE{p=1.0}");
        _console.OutputLine("**************************");
        _console.OutputLine("CRITICAL FAILURE{p=1.5}");
        _console.OutputLine("**************************");
        _console.OutputLine("Impact confirmed");
        _console.OutputLine("Hull integrity ...{p=0.2} compromised");
        _console.OutputLine("thruster1 destroyed");
        _console.OutputLine("thruster2 destroyed");
        _console.OutputLine("Computing stabilizing sequence...{p=2.0}");
        _console.OutputLine("ERR - Cannot stabilize");
        _console.OutputLine("Thrusters missing");
        _console.OutputLine("Deploy backup thruster1");
        await ToSignal(_console, "TextFinished");
        _console.ToggleActivateButton("BackupLeft", true);

        while (!enteredSafeLaunchCode)
        {
            await ToSignal(GetTree().CreateTimer(rng.RandfRange(2.0f, 5.0f)), "timeout");
            _camera.ApplyShake(rng.RandfRange(10f, 60f), 4f);
        }

        Stabilized();
    }

    public void BackupDeployed()
    {
        _console.OutputLine("===========================");
        _console.OutputLine("Backup thruster1 engaged");
        _console.OutputLine("THRUSTER1 - operational");
        _console.OutputLine("THRUSTER2 - destroyed");
        _console.OutputLine("Backup thruster checks");
        _console.OutputLine("0/2 available");

        _console.OutputLine("===========================");
        _console.OutputLine("Computing stabilizing sequence...{p=2.0}");
        _console.OutputLine("Stabilizing Thruster Sequence A-1-B-5");
        _console.LaunchCodes = "A1B5";
    }

    public async void Stabilized()
    {
        _camera.Emergency = false;
        _camera.ApplyShake(10f, 2f);
        _soundScapeHandler.CrashFixed();
        _window.SetSpinning(100f);
        _console.ResetThrusterSequence();

        _console.OutputLine("Course stabilized{p=1.0}");
        _console.OutputLine("WARNING - Critical damage detected{p=2.0}");
        _console.OutputLine("Hull integrity -{p=1.0} 17% -{p=1.0} failure");
        _console.OutputLine("Sending distress beacon.{p=1.0}.{p=1.0}.{p=1.0}.{p=1.0}");
        _console.OutputLine("ERR - Transmittor damaged{p=1.0}");
        _console.OutputLine("THRUSTER1 - {p=1.0}Operational");
        _console.OutputLine("THRUSTER2 - ERROR{p=1.0}");
        _console.OutputLine("THRUSTER2 destroyed");
        _console.OutputLine("==========================={p=1.0}");
        _console.OutputLine("THRUSTER2 dumped");
        _console.OutputLine("Backup Thrusters depleted{p=2.0}");
        _console.OutputLine("SYSERR{p=1.0}");
        _console.OutputLine("SYSERR{p=1.0}");
        await ToSignal(_console, "TextFinished");
        RequestLaunchcodeCheck();
    }

    public void RequestLaunchcodeCheck()
    {
        _console.OutputLine("===============================");
        _console.OutputLine("Loading emergency sequence...{p=2.0}");
        _console.OutputLine("Thruster sequence loaded");
        _console.OutputLine("┗╸Confirm sequence 5-3"); ;
        _console.LaunchCodes = "53";
    }

    public override void LaunchCodesEnteredHandler(bool correct, bool shuffled)
    {
        if (!enteredSafeLaunchCode && correct)
        {
            enteredSafeLaunchCode = true;
        }
        else if (enteredSafeLaunchCode)
        {
            _console.OutputLine("ERR - critical failure");
            LaunchCodesEntered = true;
            AllDoneOutput();
        }
    }

    public void AllDoneOutput()
    {
        _console.OutputLine("Sequence not recognized");
        _console.OutputLine("✱✱ERROR✱✱");
        _console.OutputLine("✱✱ERROR✱✱");
        _console.OutputLine("✱✱ERROR✱✱");
        _console.OutputLine("✱✱Enation module load successful");
        _console.OutputLine("target = ???");
        _console.OutputLine("hibernation time ~SYSERR months");
        _console.OutputLine("Confirm hibernation?");
        _console.RequestInput();
    }

    public override void InputReceivedHandler(string question, string input)
    {
        if (question == "Confirm hibernation?")
        {
            _console.ToggleRaiseText();
            _console.ToggleActivateButton("Hibernation", true);
            _console.ToggleButtonPressed("Hibernation", false, silent: true);
        }
    }

    public override void ButtonPressed(string buttonName, bool toggled)
    {
        if (buttonName == "Hibernation" && toggled) _ = _hibernationHandler.EnterHibernation("LevelScenes/8_deep_space");
        if (buttonName == "BackupLeft" && toggled)
        {
            BackupDeployed();
            _console.ToggleActivateButton("BackupLeft", false);
        }
    }
}
