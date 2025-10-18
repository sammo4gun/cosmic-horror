using Godot;
using System;

public partial class FlippableButton : TextureButton
{
    public override void _Ready()
    {
        base._Ready();
        Toggled += playSound;
        GetNode<AudioStreamPlayer>("StartupSoundPlayer").Finished += PlayHum;
    }

    private void playSound(bool toggled)
    {
        if (toggled)
            GetNode<AudioStreamPlayer>("OnSoundPlayer").Play();
        else
            GetNode<AudioStreamPlayer>("OffSoundPlayer").Play();
    }

    public void WrongLaunchCodeSound()
    {
        GetNode<AudioStreamPlayer>("WrongSoundPlayer").Play();
    }
    
    public void CorrectLaunchCodeSound()
    {
        GetNode<AudioStreamPlayer>("StartupSoundPlayer").Play();
    }

    public void PlayHum()
    {
        GetNode<AudioStreamPlayer>("HumSoundPlayer").Play();
    }
}
