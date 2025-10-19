using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

// SCENE_ID: Saturn
// Window: The player sees saturn,and a bunch of ominous rocks....
// 
public partial class Saturn : Shuttle
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

        _window.SetWindow("Saturn");

        // starting time, distance, and speed
        _timeHandler.StartTimer(DateTime.ParseExact("08-09-1977 22:03:56.000", "dd-MM-yyyy HH:mm:ss.FFF", null));
        _spaceHandler.StartDistance(3_802_341f + 18_342f);
        Speed = 18f;

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
        _console.OutputLine("Verifying {p=0.3}. . . . . . . . . . . {p=0.5}. {p=0.3}. {p=0.3}.");
        _console.OutputLine("Verification complete");
        _console.OutputLine("Boot successful");
        _console.OutputLine("System load logged in usr/logs/080919772203560.json");
        _console.OutputLine("Running preliminary diagnostics...{p=0.2}");
        _console.OutputLine("Hull integrity - 98% - PASS");
        _console.OutputLine("Battery cell total charge - 93%");
        _console.OutputLine("THRUS1 - Operational");
        _console.OutputLine("THRUS2 - Operational");
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

        _console.OutputLine("ALERT!!!!");
        _console.OutputLine("Input code A1B5 NOW");
        _console.LaunchCodes = "A1B5";

        while (!enteredSafeLaunchCode)
        {
            await ToSignal(GetTree().CreateTimer(rng.RandfRange(2.0f, 5.0f)), "timeout");
            _camera.ApplyShake(rng.RandfRange(10f, 40f), 3f);
        }

        _camera.Emergency = false;
        _camera.ApplyShake(10f, 0f);
        _soundScapeHandler.CrashFixed();
        _console.OutputLine("Whew...");

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
        _console.OutputLine("target = jupiter_orbit");
        _console.OutputLine("hibernation time ~26 months");
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
        if (buttonName == "Hibernation" && toggled) _ = _hibernationHandler.EnterHibernation("LevelScenes/4_jupiter");
    }
}
