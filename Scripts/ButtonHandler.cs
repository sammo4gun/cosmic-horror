using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ButtonHandler : Node
{
    public Dictionary<string, bool> Buttons = new Dictionary<string, bool>();

    public override void _Ready()
    {
        base._Ready();

        foreach (TextureButton button in GetChildren())
        {
            Buttons[GetButtonName(button)] = false;
            button.Toggled += (toggled) => OnButtonPressed(button, toggled);
        }
    }

    private void OnButtonPressed(TextureButton button, bool toggled)
    {
        Buttons[GetButtonName(button)] = toggled;
    }

    private static string GetButtonName(TextureButton button)
    {
        return ((string)button.Name)[((string)button.Name).Length - 1].ToString();
    }

}
