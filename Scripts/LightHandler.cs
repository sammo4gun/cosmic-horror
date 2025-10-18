using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LightHandler : Node
{
    public Dictionary<string, Light> Lights = new Dictionary<string, Light>();

    public override void _Ready()
    {
        base._Ready();

        foreach (Light light in GetChildren().Cast<Light>())
        {
            Lights[GetLightName(light)] = light;
        }
    }

    public void Set(string lightName, bool toggled)
    {
        Lights[lightName].SetLightState(toggled);
    }

    private static string GetLightName(Light light)
    {
        return ((string)light.Name).Substr("FlippableLight".Length, ((string)light.Name).Length - "FlippableLight".Length).ToString();
    }


    // Flicker lights code would go here/under the process code

}
