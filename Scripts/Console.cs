using Godot;
using System;
using System.Text;

public partial class Console : Node2D
{   
    private TextDisplay _textDisplay;
    private ButtonHandler _buttonHandler;
    private LightHandler _lightHandler;
    private Dial _flatDial;
    private Dial _heightDial;
    private RadioReceiver _radioReceiver;
    public string LaunchCodes;

    [Signal]
    public delegate void ButtonPressedEventHandler(string buttonName, bool toggled);

    public bool IsButtonPressed(string button) => _buttonHandler.Buttons[button];

    public override void _Ready()
    {
        base._Ready();
        _textDisplay = GetNode<TextDisplay>("TextDisplay");
        _buttonHandler = GetNode<ButtonHandler>("ButtonHandler");
        _lightHandler = GetNode<LightHandler>("LightHandler");
        _flatDial = GetNode<Dial>("FlatDial");
        _heightDial = GetNode<Dial>("HeightDial");
        _radioReceiver = GetNode<RadioReceiver>("RadioReceiver");

        _textDisplay.InputReceived += (question, input) => ((Shuttle)GetParent()).ReceiveInput(question, input);
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
            ((Shuttle)GetParent()).LaunchCodesEntered(correct: true, shuffled: false);
            return true;
        }
        else if (AreButtonsPressed(LaunchCodes, exact: true))
        {
            ((Shuttle)GetParent()).LaunchCodesEntered(correct: false, shuffled: true);
            return false;
        }
        else
        {
            ((Shuttle)GetParent()).LaunchCodesEntered(correct: false, shuffled: false);
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
        _flatDial.ToggleActivate(toggled);
        _heightDial.ToggleActivate(toggled);
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
}
