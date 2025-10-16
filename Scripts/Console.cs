using Godot;
using System;

public partial class Console : Node2D
{
    private TextDisplay _textDisplay;
    private ButtonHandler _buttonHandler;
    private LightHandler _lightHandler;

    public bool IsButtonPressed(string button) => _buttonHandler.Buttons[button];

    public override void _Ready()
    {
        base._Ready();
        _textDisplay = GetNode<TextDisplay>("TextDisplay");
        _buttonHandler = GetNode<ButtonHandler>("ButtonHandler");
        _lightHandler = GetNode<LightHandler>("LightHandler");

        _textDisplay.InputReceived += (question, input) => ((Shuttle)GetParent()).ReceiveInput(question, input);
    }

    public void OutputLine(string line, bool noquestion = false)
    {
        _textDisplay.AddLine(line, noquestion);
    }

    public bool AreButtonsPressed(string buttons, bool exact = false)
    {
        foreach (var button in _buttonHandler.Buttons)
        {
            if (buttons.Contains(button.Key))
            {
                if (!button.Value) return false;
            }
            else if (exact && button.Value)
            {
                return false;
            }
        }
        return true;
    }

    public void SetLightState(string lightName, bool toggled)
    {
        _lightHandler.Set(lightName, toggled);
    }

    public void RequestInput()
    {
        _textDisplay.AskForInput();
    }

    public void ToggleRaiseText()
    {
        _textDisplay.ToggleRaise();
    }
}
