using Godot;
using System;
using System.Collections.Generic;

// SCENE_ID: Departing_Earth
// Window: The player sees earth moving away very slowly, starting out very close to the ground
// Get used to the basics: the launch code, and the french integrity check.
// Blast forwards 1 week!
public partial class EarthLeaving : Shuttle
{
    public bool TriggeredConsole = false;

    public override void _Ready()
    {
        base._Ready();
        // starting time, distance, and speed
        _timeHandler.StartTimer(DateTime.ParseExact("05-09-1989 12:56:01.000", "dd-MM-yyyy HH:mm:ss.FFF", null));
        _spaceHandler.StartDistance(20_000f);

        // _ = _hibernationHandler.EndHibernation(delay:1.5f, speedFactor: 4);
        _ = _hibernationHandler.EndHibernation(delay:0f, speedFactor: 1);
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

        await ToSignal(GetTree().CreateTimer(2f), "timeout");

        _console.ToggleRaiseText();
        //Character line(ish)
        //                  |                                      |
        _console.OutputLine("Bootsys v95.2.5");
        _console.OutputLine("Initialising \"Voyager1\"");
        _console.OutputLine("Verifying {p=0.3}. {p=0.3}. . . . {p=0.5}. {p=0.5}. . . . . {p=1.0}. . .");
        _console.OutputLine("Verification complete.");
        _console.OutputLine("Boot successful.");
        _console.OutputLine("System load logged in usr/logs/050919791256010.json");
        _console.OutputLine("Running preliminary diagnostics...");
        _console.OutputLine("Hull integrity - 98% - PASS");
        _console.OutputLine("Battery cell total charge - 94%");
        _console.OutputLine("THRUS1 - Operational.");
        _console.OutputLine("THRUS2 - Operational.");
        _console.OutputLine("Velocity - 17.34km/s - STABLE");
        _console.OutputLine("Run advanced diagnostics? (y/n)");
        _console.RequestInput();
        // _recordPlayer.Disabled = false;
        // _console.RadioAlert(true);
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
                _console.OutputLine("Postlaunch tests:");
                _console.OutputLine("Running . {p=0.3}. . . . {p=0.5}. {p=0.5}. . . . . {p=1.0}. . .");
                _console.OutputLine("PASSED with 1 warning and 0 errors");
                _console.OutputLine("Simulating trajectory... {p=0.8}");
                _console.OutputLine("Trajectory Outline Confirmed");
                _console.OutputLine("Jupiter orbit entry t-minus:{p=0.8}");
                _console.OutputLine("   26 days{p=0.8}");
                _console.OutputLine("   6 months{p=0.8}");
                _console.OutputLine("   2 years{p=0.3}");
                _console.OutputLine("Backup thruster check...");
                _console.OutputLine("2/2 available");

            }
            else if (input.ToLower() == "n")
            {
                _console.OutputLine("Uh okay well fukc u");
            }
            else
            {
                _console.OutputLine("Err: Input not recognized.\n>   Expected y/n response.", noquestion: true);
                _console.RequestInput();
            }
        }
    }


    public override void LaunchCodesEnteredHandler(bool correct, bool shuffled)
    {
        if (correct) _console.OutputLine("Launch code received. Ready for takeoff. Psheeewwww!!!");
        else if (shuffled) _console.OutputLine("Incorrect ordering on launch codes. Holding off on launch.");
        else _console.OutputLine("Launch codes incorrect. Awaiting instruction.");
    }

    public override void RecordDone()
    {
        _console.OutputLine("AAAA");
    }

    public override void ButtonPressed(string buttonName, bool toggled)
    {
        if (buttonName == "Hibernation" && toggled) _ = _hibernationHandler.EnterHibernation("LevelScenes/1_earth_leaving");
    }

}
