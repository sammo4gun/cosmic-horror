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
    public bool triggeredDangerCutscene = false;
    public bool enteredSafeLaunchCode = false;
    public bool BackupInstalled = false;
    public bool LaunchCodesEntered = false;


    private RandomNumberGenerator rng = new RandomNumberGenerator();

    public override void _Ready()
    {
        base._Ready();

        _window.SetWindow("Stars");
        _window.SetAsteroidsVisible(true);

        _console.ToggleActivateButton("Hibernation", false); // so we can't hibernate right away.
        _console.ToggleButtonPressed("Hibernation", true, silent: true); // sothe hibernation button is off
        _console.ToggleButtonPressed("BackupRight", true, silent: true); // to set the backup to being used
        // starting time, distance, and speed
        _timeHandler.StartTimer(DateTime.ParseExact("24-06-1982 12:05:59.000", "dd-MM-yyyy HH:mm:ss.FFF", null));
        _spaceHandler.StartDistance(2_315_487_315f);
        Speed = 21f;

        // _console.ToggleButtonPressed("BackupLeft", true, silent: true); // to set the backup to being used
        _recordPlayer.LoadSong(4, repeated: false, loadBar: false);

        // _ = _hibernationHandler.EndHibernation(delay:1.5f, speedFactor: 4);
        _ = _hibernationHandler.EndHibernation(delay: 0f, speedFactor: 1);
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
        _console.OutputLine("Hibernation_length=39 days{p=1.0}");
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

    public override void RecordStarted()
    {
        if (!triggeredDangerCutscene)
        {
            _console.RadioAlert(false);
            dangerCutscene();
        }
    }

    public async void dangerCutscene()
    {
        await ToSignal(GetTree().CreateTimer(15.0f), "timeout");
        triggeredDangerCutscene = true;
        if (!_camera.FacingConsole) _camera.Turn("left");
    }

    public override void RecordDone()
    {
        HandleCrash();
    }

    public async void HandleCrash()
    {
        _camera.ApplyShake(50, 10);
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
    }

    public async void BackupDeployed()
    {
        _console.OutputLine("===========================");
        _console.OutputLine("Backup thruster1 engaged");
        _console.OutputLine("THRUSTER1 - operational");
        _console.OutputLine("THRUSTER2 - destroyed");
        _console.OutputLine("Backup thruster checks");
        _console.OutputLine("0/2 available");
        RequestLaunchcodeCheck();

        _console.OutputLine("Computing stabilizing sequence...{p=2.0}");
        _console.OutputLine("Stabilizing Thruster Sequence A-1-B-5");
        _console.LaunchCodes = "A1B5";

        while (!enteredSafeLaunchCode)
        {
            await ToSignal(GetTree().CreateTimer(rng.RandfRange(2.0f, 5.0f)), "timeout");
            _camera.ApplyShake(rng.RandfRange(10f, 60f), 3f);
        }

        _camera.Emergency = false;
        _camera.ApplyShake(10f, 0f);
        _soundScapeHandler.CrashFixed();
        _console.ResetThrusterSequence();
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
        _console.OutputLine("┗╸Confirm sequence 5-3-D-2-C"); ;
        _console.LaunchCodes = "53D2C";
    }

    public override void LaunchCodesEnteredHandler(bool correct, bool shuffled)
    {
        if (correct)
        {
            _console.OutputLine("Thruster Sequence received");
            if (!enteredSafeLaunchCode)
            {
                enteredSafeLaunchCode = true;
            }
            else
            {
                LaunchCodesEntered = true;
                AllDoneOutput();
            }
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
        _console.OutputLine("hibernation time ~14 months");
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
        if (buttonName == "Hibernation" && toggled) _ = _hibernationHandler.EnterHibernation("LevelScenes/8_?");
        if (buttonName == "BackupLeft" && toggled)
        {
            BackupDeployed();
        }
    }
}
