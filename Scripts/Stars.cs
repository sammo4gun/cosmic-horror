using Godot;
using System;

public partial class Stars : ColorRect
{
    private Shader shader;
    private ShaderMaterial material;
    private float offset = 0f;
    public float spinningStrength = 0f;
    public override void _Ready()
    {
        base._Ready();

        // Load the shader from your .gdshader file
        shader = GD.Load<Shader>("res://Shaders/stars.gdshader");

        // Create a ShaderMaterial using that shader
        material = new ShaderMaterial();
        material.Shader = shader;

        // Assign the material
        this.Material = material;
        var rng = new RandomNumberGenerator();
        material.SetShaderParameter("offset", new Vector2(rng.RandfRange(-100f, 100f), rng.RandfRange(-100f, 100f)));
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (spinningStrength > 0)
        {
            Vector2 _currentOffset = (Vector2)material.GetShaderParameter("offset");
            material.SetShaderParameter("offset", new Vector2(_currentOffset.X+spinningStrength*(float)delta, _currentOffset.Y+spinningStrength*(float)delta*0.5f));
        }
    }
}
