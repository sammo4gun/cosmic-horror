using Godot;
using System;

public partial class SoundScapeHandler : Node
{

    private AudioStreamPlayer _voyagerReversedPlayer;
    private AudioStreamPlayer _humPlayer;

    public override void _Ready()
    {
        base._Ready();
        _voyagerReversedPlayer = GetNode<AudioStreamPlayer>("VoyagerReversedPlayer");
        _humPlayer = GetNode<AudioStreamPlayer>("HumPlayer");

        _voyagerReversedPlayer.Play();
        _humPlayer.Play();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        // Fade out the voyager reversed sound over time
        if (_voyagerReversedPlayer.VolumeDb > -80)
            _voyagerReversedPlayer.VolumeDb -= 0.5f*(float)delta; // 1 dB per second
    }
}
