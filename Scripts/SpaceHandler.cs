using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class SpaceHandler : Node
{
    public float DistanceFromEarth = 2_000.0f; // in km

    // Calculating distance from Earth in process
    public override void _Process(double delta)
    {
        base._Process(delta);
        DistanceFromEarth += GetAdjustedSpeed() * (float)delta;
        GD.Print($"delta Distance from Earth: {DistanceFromEarth} km");
    }

    // Calculating distance from Earth when time is added
    public void AddDistance(int n, string unit, float off_degrees_flat, float off_degrees_height)
    {
        float secondsPassed;
        switch (unit)
        {
            case "second s":
                secondsPassed = n;
                break;
            case "minutes":
                secondsPassed = n * 60;
                break;
            case "hours":
                secondsPassed = n * 3600;
                break;
            case "days":
                secondsPassed = n * 86400;
                break;
            case "months":
                secondsPassed = n * 2592000;
                break;
            case "years":
                secondsPassed = n * 31536000;
                break;
            default:
                GD.PrintErr("Invalid time unit specified.");
                return;
        }

        DistanceFromEarth += GetAdjustedSpeed() * secondsPassed;
    }
    
    private float GetAdjustedSpeed()
    {
        // Adjust for tilt
        float adjustedSpeed = ((Shuttle)GetParent()).Speed;
        adjustedSpeed *= (float)Math.Cos(Math.PI * ((Shuttle)GetParent()).SpinTilt / 180.0);
        adjustedSpeed *= (float)Math.Cos(Math.PI * ((Shuttle)GetParent()).HeightTilt / 180.0);
        return adjustedSpeed;
    }

    // Moving the spaceship appropriately through that space

    // Jumping ahead through that movement

    // Adding obstacles ahead

    // Scanning for the list of obstacles

    // Seeing if an obstacle has been/will be hit
}
