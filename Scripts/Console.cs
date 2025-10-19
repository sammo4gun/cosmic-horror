using Godot;
using System;
using System.Text;

public partial class Console : Node2D
{   
    private TextDisplay _textDisplay;
    private ButtonHandler _buttonHandler;
    private LightHandler _lightHandler;
    private Dial _leftDial;
    private Dial _rightDial;
    private DistDisplay _distDisplay;
    private TimeDisplay _timeDisplay;
    private RadioReceiver _radioReceiver;
    public string LaunchCodes;

    [Signal]
    public delegate void ButtonPressedEventHandler(string buttonName, bool toggled);
    [Signal]
    public delegate void LaunchCodesEnteredEventHandler(bool correct, bool ordered);
    [Signal]
    public delegate void InputReceivedEventHandler(string question, string input);
    [Signal]
    public delegate void TextFinishedEventHandler();

    public bool IsButtonPressed(string button) => _buttonHandler.Buttons[button];

    public override void _Ready()
    {
        base._Ready();
        _textDisplay = GetNode<TextDisplay>("TextDisplay");
        _buttonHandler = GetNode<ButtonHandler>("ButtonHandler");
        _lightHandler = GetNode<LightHandler>("LightHandler");
        _leftDial = GetNode<Dial>("LeftDial");
        _rightDial = GetNode<Dial>("RightDial");
        _radioReceiver = GetNode<RadioReceiver>("RadioReceiver");
        _timeDisplay = GetNode<TimeDisplay>("TimeDisplay");
        _distDisplay = GetNode<DistDisplay>("DistDisplay");

        _textDisplay.InputReceived += ReceiveInput;
    }

    public void ReceiveInput(string question, string input)
    {
        EmitSignal("InputReceived", question, input);
    }

    public void OutputLine(string line, bool noquestion = false)
    {
        _textDisplay.AddLine(line, noquestion);
    }

    public bool AreButtonsPressed(string buttons, bool exact = false, bool ordered = false)
    {
        if (ordered && buttons == _buttonHandler.OrderPressed) return true;
        foreach (var button in _buttonHandler.Buttons)
        {
            if (buttons.Contains(button.Key))
            {
                if (!button.Value) return false;
            }
            else if (exact && button.Value && button.Key != "Launch")
            {
                return false;
            }
        }
        return !ordered;
    }

    public bool LaunchCodesPressed()
    {
        if (LaunchCodes is null) return false;
        if (AreButtonsPressed(LaunchCodes, exact: true, ordered: true))
        {
            EmitSignal("LaunchCodesEntered", true, false);
            return true;
        }
        else if (AreButtonsPressed(LaunchCodes, exact: true))
        {
            EmitSignal("LaunchCodesEntered", false, true);
            return false;
        }
        else
        {
            EmitSignal("LaunchCodesEntered", false, false);
            return false;
        }
    }

    public void OnButtonPressed(string buttonName, bool toggled)
    {
        EmitSignal("ButtonPressed", buttonName, toggled);
    }

    public void SetLightState(string lightName, bool toggled)
    {
        _lightHandler.Set(lightName, toggled);
    }

    public void ToggleActivateDials(bool toggled)
    {
        _leftDial.ToggleActivate(toggled);
        _rightDial.ToggleActivate(toggled);
    }

    public void ToggleActivateButton(string buttonName, bool toggled)
    {
        _buttonHandler.ToggleButtonAvailable(buttonName, toggled);
    }

    public void ToggleButtonPressed(string buttonName, bool toggled, bool silent = true)
    {
        _buttonHandler.ToggleButtonPressed(buttonName, toggled, silent);
    }

    public void RequestInput()
    {
        _textDisplay.AskForInput();
    }

    public void ToggleRaiseText()
    {
        _textDisplay.ToggleRaise();
    }

    public void RadioAlert(bool isOn)
    {
        _radioReceiver.SetAlertState(isOn);
    }

    public void TextDisplayFinished()
    {
        EmitSignal("TextFinished");
    }

    public float getDialValue(string which)
    {
        if (which == "left")
        {
            return _leftDial.getValue();
        }
        else
        {
            return _rightDial.getValue();
        }
    }

    public void ResetThrusterSequence()
    {
        LaunchCodes = "";
        _buttonHandler.ResetThrusterSequence();
    }
}
