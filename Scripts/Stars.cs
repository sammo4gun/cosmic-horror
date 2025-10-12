using Godot;
using System;

public partial class Stars : ColorRect
{
    public override void _Ready()
    {
        base._Ready();

        // Load the shader from your .gdshader file
        var shader = GD.Load<Shader>("res://Shaders/stars.gdshader");

        // Create a ShaderMaterial using that shader
        var material = new ShaderMaterial();
        material.Shader = shader;

        // Assign the material
        this.Material = material;
    }
}
