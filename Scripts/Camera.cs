using Godot;
using System;

public partial class Camera : Camera2D
{
    [Export]
    public int MoveSpeed { get; set; } = 8;
    [Export]
    public int ZoomSpeed { get; set; } = 8;
    [Export]
    public float ZoomFactor { get; set; } = 1.2f;

    private static Vector2 LEFT_POSITION = new((648 / 2) + (648 * -2), 648 / 2);
    private static Vector2 SWITCH_POSITION_1 = new((648 / 2) + (648 * -1), 648 / 2);
    private static Vector2 CONSOLE_POSITION = new(648 / 2, 648 / 2);
    private static Vector2 WINDOW_POSITION = new((648 / 2) + (648 * 2), 648 / 2);
    private static Vector2 SWITCH_POSITION_2 = new((648 / 2) + (648 * 3), 648 / 2);
    private static Vector2 RIGHT_POSITION = new((648 / 2) + (648 * 4), 648 / 2);

    public bool FacingConsole = true;
    private Vector2 TargetPosition;

    public override void _Ready()
    {
        base._Ready();
        TargetPosition = Position;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        HandleZoom(delta);
        MoveToTarget(delta);
        HandleSwitching();
    }

    private void HandleZoom(double delta)
    {
        if (Position.DistanceTo(TargetPosition) < 1)
        {
            Zoom = Zoom.Lerp(new Vector2(ZoomFactor, ZoomFactor), Math.Min(1, ZoomSpeed * (float)delta));
        }
        else Zoom = Zoom.Lerp(new Vector2(1, 1), Math.Min(1, ZoomSpeed * (float)delta));
    }

    private void MoveToTarget(double delta)
    {
        if (Position != TargetPosition && Zoom.X < 1.01f)
        {
            Position = Position.Lerp(TargetPosition, Math.Min(1, MoveSpeed * (float)delta));
        }
    }

    private void HandleSwitching()
    {
        if (TargetPosition == LEFT_POSITION && Position.X < SWITCH_POSITION_1.X)
        {
            Position = SWITCH_POSITION_2;
            TargetPosition = WINDOW_POSITION;
        }
        else if (TargetPosition == RIGHT_POSITION && Position.X > SWITCH_POSITION_2.X)
        {
            Position = SWITCH_POSITION_1;
            TargetPosition = CONSOLE_POSITION;
        }
    }
    
    public bool Turn(string direction)
    {
        if (Position.DistanceTo(TargetPosition) > 1) return false;
        if (direction == "right")
        {
            if (!FacingConsole)
            {
                TargetPosition = RIGHT_POSITION;
            }
            else
            {
                TargetPosition = WINDOW_POSITION;
            }
            FacingConsole = !FacingConsole;
            return true;
        }
        else if (direction == "left")
        {
            if (FacingConsole)
            {
                TargetPosition = LEFT_POSITION;
            }
            else
            {
                TargetPosition = CONSOLE_POSITION;
            }
            FacingConsole = !FacingConsole;
            return true;
        }
        return false;
    }
}
