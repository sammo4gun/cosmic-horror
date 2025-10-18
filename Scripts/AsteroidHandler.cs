using Godot;

public partial class AsteroidHandler : Node2D
{
    [Export] public float SpawnInterval = 1f;
    [Export] public float SpawnMargin = 128f;
    [Export] public PackedScene AsteroidScene;
    // Optional: override spawn area (0 = use viewport size)
    [Export] public Vector2 CustomAreaSize = Vector2.Zero;

    private Timer _timer;
    private readonly RandomNumberGenerator _rng = new();

    public override void _Ready()
    {
        AsteroidScene ??= GD.Load<PackedScene>("res://Scenes/asteroid.tscn");

        _timer = new Timer
        {
            WaitTime = SpawnInterval,
            OneShot = false
        };
        AddChild(_timer);
        _timer.Timeout += SpawnAsteroid;
        _timer.Start();
    }

    private void SpawnAsteroid()
    {
        if (AsteroidScene == null) return;

        // Center is this node's global position now
        Vector2 center = GlobalPosition;

        // Determine area size
        Vector2 size = CustomAreaSize;
        if (size == Vector2.Zero)
            size = GetViewport().GetVisibleRect().Size; // raw viewport size

        float halfW = size.X * 0.5f;
        float halfH = size.Y * 0.5f;
        float m = SpawnMargin;

        int side = _rng.RandiRange(0, 3);
        float x = 0, y = 0;

        switch (side)
        {
            case 0: // Top
                x = center.X + _rng.RandfRange(-halfW, halfW);
                y = center.Y - halfH - m;
                break;
            case 1: // Bottom
                x = center.X + _rng.RandfRange(-halfW, halfW);
                y = center.Y + halfH + m;
                break;
            case 2: // Left
                x = center.X - halfW - m;
                y = center.Y + _rng.RandfRange(-halfH, halfH);
                break;
            case 3: // Right
                x = center.X + halfW + m;
                y = center.Y + _rng.RandfRange(-halfH, halfH);
                break;
        }

        var instance = AsteroidScene.Instantiate();
        if (instance is Asteroid asteroid)
        {
            const float originJitter = 128f;
            Vector2 offset = new(_rng.RandfRange(-originJitter, originJitter), _rng.RandfRange(-originJitter, originJitter));
            asteroid.SpawnOrigin = GlobalPosition + offset; // Jittered around handler position
        }
        if (instance is Node2D n2d)
            n2d.GlobalPosition = new Vector2(x, y);
        else if (instance is Control ctrl)
            ctrl.GlobalPosition = new Vector2(x, y);

        (GetTree().CurrentScene ?? this).AddChild(instance);
    }
}