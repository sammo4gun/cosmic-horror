using Godot;
using System;

public partial class RadioReceiver : Node2D
{
    private AnimatedSprite2D _onSprite;
    private Sprite2D _offSprite;

    public override void _Ready()
    {
        base._Ready();
        _onSprite = GetNode<AnimatedSprite2D>("OnSprite");
        _offSprite = GetNode<Sprite2D>("OffSprite");
    }

    public void SetAlertState(bool isOn)
    {
        _onSprite.Visible = isOn;
        _offSprite.Visible = !isOn;
        if (isOn)
        {
            GetNode<AudioStreamPlayer>("AlertStartPlayer").Play();
            GetNode<AudioStreamPlayer>("AlertPlayer").Play();
        }
        else GetNode<AudioStreamPlayer>("AlertPlayer").Stop();
    }
}
