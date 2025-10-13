using Godot;
using System;

public partial class Console : Node2D
{
    private TextDisplay _textDisplay;
    private ButtonHandler _buttonHandler;

    public bool IsButtonPressed(string button) => _buttonHandler.Buttons[button];

    public override void _Ready()
    {
        base._Ready();
        _textDisplay = GetNode<TextDisplay>("TextDisplay");
        _buttonHandler = GetNode<ButtonHandler>("ButtonHandler");

        _textDisplay.InputReceived += (question, input) => ((Shuttle)GetParent()).ReceiveInput(question, input);
    }

    public void OutputLine(string line, bool noquestion = false)
    {
        _textDisplay.AddLine(line, noquestion);
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
