using Godot;

public partial class RecordPlayer : TextureButton
{
    [Signal]
    public delegate void ValidationDoneEventHandler();
    
    private AudioStreamPlayer _musicPlayer;
    private AudioStreamPlayer _loadPlayer;
    private AudioStreamPlayer _backgroundPlayer;
    private AudioStreamPlayer _unloadPlayer;
    private AudioStreamPlayer _pausePlayer;

    private ColorRect _notLoadedBar;
    private ColorRect _loadBar;

    public float SongLength;

    public override void _Ready()
    {
        base._Ready();
        _musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
        _loadPlayer = GetNode<AudioStreamPlayer>("LoadPlayer");
        _backgroundPlayer = GetNode<AudioStreamPlayer>("BackgroundPlayer");
        _unloadPlayer = GetNode<AudioStreamPlayer>("UnloadPlayer");
        _pausePlayer = GetNode<AudioStreamPlayer>("PausePlayer");

        _notLoadedBar = GetNode<ColorRect>("LoadBarFillerNotDone");
        _loadBar = GetNode<ColorRect>("LoadBarFiller");

        SongLength = (float)_musicPlayer.Stream.GetLength();

        Toggled += RecordActivated;
        _musicPlayer.Finished += RecordDone;
        _loadPlayer.Finished += RecordStarted;
        _pausePlayer.Finished += RecordStopped;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_musicPlayer.Playing && !_musicPlayer.StreamPaused)
        {
            _loadBar.Size = new Vector2(_loadBar.Size.X + (float) delta / SongLength * 200, _loadBar.Size.Y);
        }
        // IF the musicplayer is playing and not paused, progress the loading bar accordingly.
    }

    private void RecordActivated(bool toggle)
    {
        if (!_musicPlayer.Playing && toggle && !_musicPlayer.StreamPaused)
        {
            _notLoadedBar.Visible = true;
            _loadBar.Visible = true;
            _loadPlayer.Play();
        }
        else if (toggle && _musicPlayer.StreamPaused)
        {
            _loadPlayer.Play();
        }
        else if (_musicPlayer.Playing && !toggle)
        {
            _pausePlayer.Play();
        }
    }

    private void RecordDone()
    {
        Disabled = true;
        ButtonPressed = false;
        _unloadPlayer.Play();
        _backgroundPlayer.Stop();
        // what else should happen when the record is done?
        EmitSignal("ValidationDone");
    }

    private void RecordStarted()
    {
        if (!_musicPlayer.Playing && !_musicPlayer.StreamPaused)
        {
            _musicPlayer.Play();
            _backgroundPlayer.Play();
        }
        else // the musicplayer is paused and should be unpaused
        {
            _musicPlayer.StreamPaused = false;
            _backgroundPlayer.Play();
        }
    }

    private void RecordStopped() // unload sound done
    {
        if (_musicPlayer.Playing && !_musicPlayer.StreamPaused) // we are dealing with an unpause situation
        {
            _musicPlayer.StreamPaused = true;
            _backgroundPlayer.Stop();
        }
    }
}
