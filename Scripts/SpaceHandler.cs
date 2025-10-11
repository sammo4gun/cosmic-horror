using Godot;
using System;

public partial class SpaceHandler : Node
{
    public float DistanceFromEarth; // in km

    public override void _Ready()
    {
        base._Ready();
        DistanceFromEarth = 1_000.0f;
    }

    // Calculating distance from Earth in process
    public override void _Process(double delta)
    {
        base._Process(delta);
        DistanceFromEarth += ((Shuttle)GetParent()).Speed * (float)delta;
    }

    // Calculating distance from Earth when time is added
    public void AddDistance(int n_km)
    {
        DistanceFromEarth += n_km;
    }
}
