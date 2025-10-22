using Godot;

public partial class RecordPlayer : TextureButton
{
    [Signal]
    public delegate void MusicDoneEventHandler();
    [Signal]
    public delegate void MusicStartedEventHandler();

    public bool showLoadBar;
    
    private AudioStreamPlayer _musicPlayer;
    private AudioStreamPlayer _loadPlayer;
    private AudioStreamPlayer _backgroundPlayer;
    private AudioStreamPlayer _unloadPlayer;
    private AudioStreamPlayer _pausePlayer;

    private ColorRect _notLoadedBar;
    private ColorRect _loadBar;

    public float SongLength;
    public bool Repeated = false;

    public override void _Ready()
    {
        base._Ready();
        _loadPlayer = GetNode<AudioStreamPlayer>("LoadPlayer");
        _backgroundPlayer = GetNode<AudioStreamPlayer>("BackgroundPlayer");
        _unloadPlayer = GetNode<AudioStreamPlayer>("UnloadPlayer");
        _pausePlayer = GetNode<AudioStreamPlayer>("PausePlayer");

        _notLoadedBar = GetNode<ColorRect>("LoadBarFillerNotDone");
        _loadBar = GetNode<ColorRect>("LoadBarFiller");

        Toggled += RecordActivated;
        _loadPlayer.Finished += RecordStarted;
        _pausePlayer.Finished += RecordStopped;

        // LoadSong(1, false, true);
    }

    public void LoadSong(int id, bool repeated, bool loadBar)
    {
        _musicPlayer = GetNode<AudioStreamPlayer>($"MusicPlayer{id}");
        SongLength = (float)_musicPlayer.Stream.GetLength();
        _musicPlayer.Finished += RecordDone;
        showLoadBar = loadBar;
        Repeated = repeated;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_musicPlayer.Playing && !_musicPlayer.StreamPaused && showLoadBar)
        {
            _loadBar.Size = new Vector2(_loadBar.Size.X + (float)delta / SongLength * 165, _loadBar.Size.Y);
        }
        else if (!showLoadBar)
        {
            _loadBar.Size = new Vector2(0, _loadBar.Size.Y);
        }
        // IF the musicplayer is playing and not paused, progress the loading bar accordingly.
        
        if (GetNode<AudioStreamPlayer>("MusicPlayer8").Playing)
        {
            if (GetNode<AudioStreamPlayer>("MusicPlayer8").VolumeDb < -10f)
                GetNode<AudioStreamPlayer>("MusicPlayer8").VolumeDb += 0.1f * (float)delta; // 0.1 dB per second
        }
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
        if (!Repeated)
        {
            Disabled = true;
            ButtonPressed = false;
            _unloadPlayer.Play();
            _backgroundPlayer.Stop();
            // what else should happen when the record is done?
            EmitSignal("MusicDone");
        }
        else
        {
            _loadBar.Size = new Vector2(0, _loadBar.Size.Y);
            _loadPlayer.Play();
        }
    }

    private void RecordStarted()
    {
        if (!_musicPlayer.Playing && !_musicPlayer.StreamPaused)
        {
            EmitSignal("MusicStarted");
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

    public void StopPlaying()
    {
        Disabled = true;
        ButtonPressed = false;
        _unloadPlayer.Play();
        _backgroundPlayer.Stop();
    }
}
