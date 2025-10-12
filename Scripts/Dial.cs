using System;
using Godot;

public partial class Dial : Node2D
{
    [Export] public float RotationSpeed = 1.0f; // How fast it turns with scroll
    [Export] public float DragSensitivity = 0.01f; // How fast it turns when dragging
    [Export] private float _ValueCap = 45.0f;    
    [Export] public float DialRadius = 64.0f;


    public float CurrentValue = 0.0f;

    private bool _dragging = false;
    private Vector2 _lastMousePos;
    private bool _mouseOver = false;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Rotation = Mathf.DegToRad(CurrentValue*10);
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            var mousePos = GetGlobalMousePosition();
            var distance = mousePos.DistanceTo(GlobalPosition);

            // Detect mouse hover using distance (simple circular hit area)
            _mouseOver = distance <= DialRadius;
            if (mouseEvent.ButtonIndex == MouseButton.WheelUp && mouseEvent.Pressed && _mouseOver)
            {
                CurrentValue = Mathf.Clamp(CurrentValue + RotationSpeed, -_ValueCap, _ValueCap);
            }
            else if (mouseEvent.ButtonIndex == MouseButton.WheelDown && mouseEvent.Pressed && _mouseOver)
            {
                CurrentValue = Mathf.Clamp(CurrentValue - RotationSpeed, -_ValueCap, _ValueCap);
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
            CurrentValue = Mathf.Clamp(CurrentValue + Math.Sign(delta.X) * RotationSpeed, -_ValueCap, _ValueCap);
            _lastMousePos = motionEvent.Position;
        }
    }
}