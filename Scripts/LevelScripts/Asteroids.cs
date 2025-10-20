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

        // starting time, distance, and speed
        _timeHandler.StartTimer(DateTime.ParseExact("12-04-1981 17:23:14.000", "dd-MM-yyyy HH:mm:ss.FFF", null));
        _spaceHandler.StartDistance(1_296_487_315f);
        Speed = 15f;

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
        // _console.OutputLine("ERR - Unexpected t_wakeup{p=2.0}");
        // _console.OutputLine("Hibernation_length=39 days{p=1.0}");
        // _console.OutputLine("Verifying {p=0.3}. . . . . . . . . . . {p=0.5}. {p=0.3}. {p=0.3}.");
        // _console.OutputLine("Verification complete");
        // _console.OutputLine("Boot successful");
        // _console.OutputLine("SYSERR - failed to save logs");
        await ToSignal(_console, "TextFinished");

        _camera.ApplyShake(50, 10);
        _camera.Emergency = true;
        _soundScapeHandler.Crash();
        _window.SetAsteroidsVisible(false);
        _window.SetSpinning(10000f);
        _window.DeleteAllAsteroids();

        _console.OutputLine("ALERT! Message received");
        _console.OutputLine("Dated 5034hrs - broadcaster active");
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
        _camera.Emergency = true;
        _camera.ApplyShake(50f, 5f);
        _soundScapeHandler.Crash();

        _console.OutputLine("**************************");
        _console.OutputLine("CRITICAL FAILURE{p=1.0}");
        _console.OutputLine("**************************");
        _console.OutputLine("CRITICAL FAILURE{p=1.5}");
        _console.OutputLine("**************************");
        _console.OutputLine("Impact confirmed");
        _console.OutputLine("Hull integrity {p=0.5}. {p=0.5}. {p=0.5}. uncompromised");
        _console.OutputLine("thruster2 damaged");
        _console.OutputLine("Computing stabilizing sequence...{p=2.0}");
        _console.OutputLine("Stabilizing Thruster Sequence A-1-B-5");
        _console.LaunchCodes = "A1B5";

        while (!enteredSafeLaunchCode)
        {
            await ToSignal(GetTree().CreateTimer(rng.RandfRange(2.0f, 5.0f)), "timeout");
            _camera.ApplyShake(rng.RandfRange(10f, 40f), 3f);
        }

        _camera.Emergency = false;
        _camera.ApplyShake(10f, 0f);
        _soundScapeHandler.CrashFixed();
        triggeredDangerCutscene = false;
        _console.ResetThrusterSequence();
        _console.OutputLine("Course stabilized{p=1.0}");
        _console.OutputLine("Assessing damage...{p=2.0}");
        _console.OutputLine("Hull integrity -{p=1.0} 45% -{p=1.0} pass");
        _console.OutputLine("THRUSTER1 - {p=1.0}Operational");
        _console.OutputLine("THRUSTER2 - ERROR{p=1.0}");
        _console.OutputLine("THRUSTER2 destroyed");
        _console.OutputLine("==========================={p=1.0}");
        _console.OutputLine("THRUSTER2 dumped");
        _console.OutputLine("Engage backup thruster2...");
        await ToSignal(_console, "TextFinished");
        _console.ToggleActivateButton("BackupRight", true);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("left") && !triggeredDangerCutscene)
        {
            _camera.Turn("left");
        }
        if (@event.IsActionPressed("right") && !triggeredDangerCutscene)
        {
            _camera.Turn("right");
        }
    }

    public void BackupDeployed()
    {
        _console.OutputLine("===========================");
        _console.OutputLine("Backup thruster2 engaged");
        _console.OutputLine("THRUSTER2 - operational");
        _console.OutputLine("Backup thruster checks");
        _console.OutputLine("1/2 available");
        RequestLaunchcodeCheck();
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
        _console.OutputLine("target = extrasolar_space");
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
        }
    }

    public override void ButtonPressed(string buttonName, bool toggled)
    {
        if (buttonName == "Hibernation" && toggled) _ = _hibernationHandler.EnterHibernation("LevelScenes/5_solar_flare");
        if (buttonName == "BackupRight" && toggled)
        {
            BackupDeployed();
        }
    }
}
