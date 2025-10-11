using Godot;
using System;

public partial class Console : Node2D
{
    private TextDisplay _textDisplay;

    public override void _Ready()
    {
        base._Ready();
        _textDisplay = GetNode<TextDisplay>("TextDisplay");
    }

    public void OutputLine(string line)
    {
        _textDisplay.AddLine(line);
    }

    public void ToggleRaiseText()
    {
        _textDisplay.ToggleRaise();
    }
}
