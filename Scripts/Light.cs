using Godot;
using System;

public partial class Light : Node2D
{
    private Sprite2D _onLightSprite;
    private Sprite2D _offLightSprite;

    public override void _Ready()
    {
        base._Ready();
        _onLightSprite = GetNode<Sprite2D>("OnLight");
        _offLightSprite = GetNode<Sprite2D>("OffLight");
    }

    public void SetLightState(bool isOn)
    {
        _onLightSprite.Visible = isOn;
        _offLightSprite.Visible = !isOn;
    }
}
