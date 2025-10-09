using Godot;
using System;

public partial class TextDisplay : Node2D
{
    private RichTextLabel _mainText;

    public override void _Ready()
    {
        base._Ready();

        _mainText = GetNode<RichTextLabel>("Text");
        _mainText.Text = "Hello!";
    }
}
