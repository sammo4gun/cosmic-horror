using Godot;
using System;

public partial class Window : Node2D
{
    public override void _Ready()
    {
        base._Ready();
    }

    public void SetWindow(string windowName)
    {
        GetNode<SubViewportContainer>(windowName).Visible = true;
    }

}
