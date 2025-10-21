using Godot;
using System;

public partial class Menu : CanvasLayer
{

    public override void _Ready()
    {
        base._Ready();
    }

    private void _on_start_sequence_audio_finished()
    {
        GetTree().ChangeSceneToFile($"res://Scenes/LevelScenes/1_earth_leaving.tscn");
    }
}
