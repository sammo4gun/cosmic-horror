using Godot;

public partial class StartButton : TextureButton
{
    private SoundScapeHandler SoundScapeHandler;
    private AudioStreamPlayer StartSequenceAudio;
    private BlackScreen BlackScreen;

    public override void _Ready()
    {
        base._Ready();
        Pressed += OnPressed;
        StartSequenceAudio = GetParent().GetNode<AudioStreamPlayer>("StartSequenceAudio");
        SoundScapeHandler = GetParent().GetNode<SoundScapeHandler>("SoundScapeHandler");
        BlackScreen = GetParent().GetNode<BlackScreen>("BlackScreen");
    }

    private void OnPressed()
    {
        SoundScapeHandler.QueueFree();
        BlackScreen.FadeToBlack();
        StartSequenceAudio.Playing = true;
        QueueFree();
    }
}