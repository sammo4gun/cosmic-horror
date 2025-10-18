using Godot;
using System;

public partial class PlanetBody : ColorRect
{
    private float rotationSpeed = 8.0f;
    private float minRadius = 0.05f;
    private float shrinkRate = 0.05f;
    private string planetShader = "res://Shaders/planet.gdshader";
    [Export]
    public float MinRadius
    {
        get => minRadius;
        set => minRadius = value;
    }

    [Export]
    public float ShrinkRate
    {
        get => shrinkRate;
        set => shrinkRate = value;
    }

    [Export]
    public string PlanetShader
    {
        get => planetShader;
        set => planetShader = value;
    }

    [Export]
    public float RotationSpeed
    {
        get => rotationSpeed;
        set => rotationSpeed = value;
    }


    public override void _Ready()
    {
        base._Ready();

        // Load the shader from your .gdshader file
        var shader = GD.Load<Shader>(PlanetShader);

        // Create a ShaderMaterial using that shader
        var material = new ShaderMaterial();
        material.Shader = shader;

        // Assign the material
        this.Material = material;

        // (Optional) adjust shader parameters
        material.SetShaderParameter("rotation_speed", RotationSpeed);
        //material.SetShaderParameter("radius", 0.3f);
        material.SetShaderParameter("cell_amount", 10);

        this.PivotOffset = new Vector2(this.Size.X / 2, this.Size.Y / 2);
    }

    public override void _Process(double delta)
    {
        Shrink(delta);
        base._Process(delta);
    }

    // shrink function 
    public void Shrink(double delta)
    {
        float currentRadius = this.Scale.X;
        if (currentRadius > MinRadius)
        {
            // Logarithmic shrink: ShrinkRate scales with log(currentRadius)
            float logFactor = MathF.Log(currentRadius + 1); // +1 to avoid log(0)
            currentRadius -= (float)delta * ShrinkRate * logFactor;
        }
        this.Scale = new Vector2(currentRadius, currentRadius);

    }
}
