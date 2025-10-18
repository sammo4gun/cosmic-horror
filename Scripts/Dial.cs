using System;
using Godot;

public partial class Dial : Node2D
{
    [Export] public float RotationSpeed = 1.0f; // How fast it turns with scroll
    [Export] public float DragSensitivity = 0.01f; // How fast it turns when dragging
    [Export] private float ValueCap = 48.0f;
    [Export] public float DialRadius = 64.0f;
    [Export] public float DisplayValueCap = 24.0f;

    private Sprite2D _dialSprite;
    private RichTextLabel _numDisplay;
    private AudioStreamPlayer _turnSoundPlayer;

    public float CurrentValue = 0.0f;
    [Export] public bool Activated = false;

    private bool _dragging = false;
    private Vector2 _lastMousePos;
    private bool _mouseOver = false;

    public override void _Ready()
    {
        base._Ready();
        _dialSprite = GetNode<Sprite2D>("DialSprite");
        _numDisplay = GetNode<RichTextLabel>("Number");
        _turnSoundPlayer = GetNode<AudioStreamPlayer>("SoundPlayer");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (!Activated) return;
        if (_dialSprite.Rotation != Mathf.DegToRad(CurrentValue * 10))
        {
            if (!_turnSoundPlayer.Playing)
                // _turnSoundPlayer.PitchScale = (float)GD.RandRange(0.7, 0.74);
                _turnSoundPlayer.Play();
        }
        _dialSprite.Rotation = Mathf.DegToRad(CurrentValue * 10);
        // format to 3 decimal places
        string formattedValue = string.Format("{0:F3}", CurrentValue / ValueCap * DisplayValueCap);
        if (CurrentValue >= 0)
            _numDisplay.Text = "+" + formattedValue;
        else
            _numDisplay.Text = formattedValue;
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        if (!Activated) return;
        if (@event is InputEventMouseButton mouseEvent)
        {
            var mousePos = GetGlobalMousePosition();
            var distance = mousePos.DistanceTo(GlobalPosition);

            // Detect mouse hover using distance (simple circular hit area)
            _mouseOver = distance <= DialRadius;
            if (mouseEvent.ButtonIndex == MouseButton.WheelUp && mouseEvent.Pressed && _mouseOver)
            {
                CurrentValue = Mathf.Clamp(CurrentValue + RotationSpeed, -ValueCap, ValueCap);
            }
            else if (mouseEvent.ButtonIndex == MouseButton.WheelDown && mouseEvent.Pressed && _mouseOver)
            {
                CurrentValue = Mathf.Clamp(CurrentValue - RotationSpeed, -ValueCap, ValueCap);
            }

            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                if (mouseEvent.Pressed && _mouseOver)
                {
                    _dragging = true;
                    _lastMousePos = mouseEvent.Position;
                }
                else
                {
                    _dragging = false;
                }
            }
        }

        if (@event is InputEventMouseMotion motionEvent && _dragging)
        {
            Vector2 delta = motionEvent.Position - _lastMousePos;
            CurrentValue = Mathf.Clamp(CurrentValue + Math.Sign(delta.X) * RotationSpeed, -ValueCap, ValueCap);
            _lastMousePos = motionEvent.Position;
        }
    }

    public void ToggleActivate(bool activate)
    {
        Activated = activate;
        if (activate) _numDisplay.Visible = true;
        else _numDisplay.Visible = false;
    }
}