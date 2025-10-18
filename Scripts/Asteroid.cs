using Godot;
using System;

public partial class Asteroid : ColorRect
{
    private float minRadius = 0.001f;
    private float shrinkRate = 0.2f;
    private string asteroidShader = "res://Shaders/asteroid.gdshader";

    [Export] public float MinRadius { get => minRadius; set => minRadius = value; }
    [Export] public float ShrinkRate { get => shrinkRate; set => shrinkRate = value; }
    [Export] public string AsteroidShader { get => asteroidShader; set => asteroidShader = value; }

    [Export] public float TravelDuration { get; set; } = 10f;
    [Export] public Vector2 TargetPosition { get; set; } = Vector2.Zero;

    public Vector2 SpawnOrigin { get; set; } = Vector2.Zero;

    private Vector2 startPosition;
    private float travelElapsed = 0f;

    public override void _Ready()
    {
        base._Ready();

        ZIndex = -10;

        var shader = GD.Load<Shader>(AsteroidShader);
        var material = new ShaderMaterial { Shader = shader };
        Material = material;
        material.SetShaderParameter("cell_amount", 10);

        var rng = new RandomNumberGenerator();
        rng.Randomize();

        int sides = rng.RandiRange(10, 24);
        float jagAmplitude = rng.RandfRange(0.01f, 0.1f);
        float jagFreq1 = rng.RandfRange(2.5f, 6.0f);
        float jagFreq2 = rng.RandfRange(2.5f, 6.0f);
        if (Mathf.IsEqualApprox(jagFreq1, jagFreq2))
            jagFreq2 += 0.1f;

        material.SetShaderParameter("polygon_sides", sides);
        material.SetShaderParameter("jag_amplitude", jagAmplitude);
        material.SetShaderParameter("jag_freq1", jagFreq1);
        material.SetShaderParameter("jag_freq2", jagFreq2);

        var worldPeriod = new Vector3(
            rng.RandfRange(6.5f, 12.5f),
            rng.RandfRange(6.5f, 12.5f),
            rng.RandfRange(6.5f, 12.5f)
        );
        material.SetShaderParameter("world_period", worldPeriod);

        PivotOffset = new Vector2(Size.X / 2, Size.Y / 2);

        startPosition = Position;
        if (TargetPosition == Vector2.Zero)
        {
            var offset = new Vector2(rng.RandfRange(50, 700), rng.RandfRange(50, 400));
            TargetPosition = startPosition + offset;
        }
    }

    public override void _Process(double delta)
    {
        if (!Shrink(delta))
            MoveTowardsTarget(delta);
        base._Process(delta);
    }

    private void MoveTowardsTarget(double delta)
    {
        if (TravelDuration <= 0f) return;
        if (travelElapsed >= TravelDuration) return;

        travelElapsed += (float)delta;
        float t = travelElapsed / TravelDuration;
        if (t > 1f) t = 1f;

        // Logarithmic ease-out: fast start, slows near end (maps [0,1] -> [0,1])
        float eased = MathF.Log(1f + t * (MathF.E - 1f));

        Position = startPosition.Lerp(SpawnOrigin, eased);
    }

    // Returns true if asteroid was freed.
    private bool Shrink(double delta)
    {
        float currentRadius = Scale.X;

        if (currentRadius <= MinRadius)
        {
            QueueFree();
            return true;
        }

        float logFactor = MathF.Log(currentRadius + 1f);
        currentRadius -= (float)delta * ShrinkRate * logFactor;

        if (currentRadius <= MinRadius)
        {
            QueueFree();
            return true;
        }

        Scale = new Vector2(currentRadius, currentRadius);
        return false;
    }
}
