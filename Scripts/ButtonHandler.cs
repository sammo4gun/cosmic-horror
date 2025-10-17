using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ButtonHandler : Node
{
    public Dictionary<string, bool> Buttons = new Dictionary<string, bool>();
    public string OrderPressed = "";

    public bool _checkingLaunchSequence = false;

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
        string buttonName = GetButtonName(button);
        Buttons[buttonName] = toggled;
        if (buttonName == "Launch" && toggled)
        {
            CheckLaunchSequence();
        }
        else if (buttonName.Length == 1 && toggled) OrderPressed += GetButtonName(button);
        else if (buttonName.Length == 1 && !toggled) OrderPressed = OrderPressed.Replace(buttonName, "");
    }

    private async void CheckLaunchSequence()
    {
        if (_checkingLaunchSequence) return;
        _checkingLaunchSequence = true;

        foreach (string buttonName in Buttons.Keys.ToList())
        {
            var button = GetNode<TextureButton>("FlippableButton" + buttonName);
            // disable button masks
            button.MouseFilter = Control.MouseFilterEnum.Ignore;
        }

        var console = (Console)GetParent();

        await ToSignal(GetTree().CreateTimer(1), "timeout");

        bool correct = console.LaunchCodesPressed();

        foreach (string buttonName in Buttons.Keys.ToList())
        {
            var button = GetNode<TextureButton>("FlippableButton" + buttonName);
            // disable button masks
            if (!(correct && buttonName == "Launch"))
            {
                button.MouseFilter = Control.MouseFilterEnum.Stop;
                button.ButtonPressed = false;
            }
        }

        _checkingLaunchSequence = false;
    }

    private static string GetButtonName(TextureButton button)
    {
        return ((string)button.Name).Substr("FlippableButton".Length, ((string)button.Name).Length - "FlippableButton".Length).ToString();
    }

}
