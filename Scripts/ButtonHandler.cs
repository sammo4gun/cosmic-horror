using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ButtonHandler : Node
{
    public Dictionary<string, bool> Buttons = new Dictionary<string, bool>();
    public Dictionary<string, bool> ButtonsActivated = new Dictionary<string, bool>();
    public string OrderPressed = "";

    public bool _checkingLaunchSequence = false;

    public override void _Ready()
    {
        base._Ready();

        foreach (FlippableButton button in GetChildren())
        {
            Buttons[GetButtonName(button)] = false;
            ButtonsActivated[GetButtonName(button)] = button.MouseFilter == Control.MouseFilterEnum.Stop;
            button.Toggled += (toggled) => OnButtonPressed(button, toggled);
        }
    }

    private void OnButtonPressed(FlippableButton button, bool toggled)
    {
        string buttonName = GetButtonName(button);
        Buttons[buttonName] = toggled;
        if (buttonName == "Launch" && toggled)
        {
            CheckLaunchSequence();
        }
        else if (buttonName.Length == 1 && toggled) OrderPressed += GetButtonName(button);
        else if (buttonName.Length == 1 && !toggled) OrderPressed = OrderPressed.Replace(buttonName, "");
        ((Console)GetParent()).OnButtonPressed(buttonName, toggled);
    }

    private async void CheckLaunchSequence()
    {
        if (_checkingLaunchSequence) return;
        _checkingLaunchSequence = true;

        foreach (string buttonName in Buttons.Keys.ToList())
        {
            if (buttonName.Length == 1 || buttonName == "Launch")
            {
                var button = GetNode<FlippableButton>("FlippableButton" + buttonName);
                // disable button masks
                button.MouseFilter = Control.MouseFilterEnum.Ignore;
            }
        }

        var console = (Console)GetParent();

        await ToSignal(GetTree().CreateTimer(1.3), "timeout");

        bool correct = console.LaunchCodesPressed();
        LaunchCodeSound(correct);

        await ToSignal(GetTree().CreateTimer(1.3), "timeout");

        foreach (string buttonName in Buttons.Keys.ToList())
        {
            var button = GetNode<FlippableButton>("FlippableButton" + buttonName);
            // flip buttons to false
            if (buttonName.Length == 1 || (!correct && buttonName == "Launch"))
            {
                button.MouseFilter = Control.MouseFilterEnum.Stop;
                button.ButtonPressed = false;
            }
        }

        _checkingLaunchSequence = false;
    }

    public void LaunchCodeSound(bool correct)
    {
        var button = GetNode<FlippableButton>("FlippableButtonLaunch");
        if (!correct) button.WrongLaunchCodeSound();
        else button.CorrectLaunchCodeSound();
    }

    private static string GetButtonName(FlippableButton button)
    {
        return ((string)button.Name).Substr("FlippableButton".Length, ((string)button.Name).Length - "FlippableButton".Length).ToString();
    }

}
