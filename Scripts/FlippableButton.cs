using Godot;
using System;

public partial class FlippableButton : TextureButton
{
    public override void _Ready()
    {
        base._Ready();
        Toggled += (toggled) => playSound(toggled);
    }

    private void playSound(bool toggled)
    {
        if (toggled)
            GetNode<AudioStreamPlayer>("OnSoundPlayer").Play();
        else
            GetNode<AudioStreamPlayer>("OffSoundPlayer").Play();
    }
}
