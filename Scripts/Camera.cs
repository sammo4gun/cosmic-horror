using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using static Godot.Control;

public partial class Camera : Camera2D
{
    [Export]
    public int MoveSpeed { get; set; } = 8;
    [Export]
    public int ZoomSpeed { get; set; } = 8;
    [Export]
    public float ZoomFactor { get; set; } = 1.2f;
    [Export]
    public float TurnVolumeReductionDB = 3.0f;
    [Export]
    public float ZoomVolumeReductionDB = 3.0f;

    [Signal]
    public delegate void HibernationStartedEventHandler();
    [Signal]
    public delegate void HibernationEndedEventHandler();

    private static Vector2 LEFT_POSITION = new((648 / 2) + (648 * -2), 648 / 2);
    private static Vector2 SWITCH_POSITION_1 = new((648 / 2) + (648 * -1), 648 / 2);
    private static Vector2 CONSOLE_POSITION = new(648 / 2, 648 / 2);
    private static Vector2 WINDOW_POSITION = new((648 / 2) + (648 * 2), 648 / 2);
    private static Vector2 SWITCH_POSITION_2 = new((648 / 2) + (648 * 3), 648 / 2);
    private static Vector2 RIGHT_POSITION = new((648 / 2) + (648 * 4), 648 / 2);
    private static float MAX_DISTANCE = 1295f;

    public bool FacingConsole = false;
    private Vector2 TargetPosition;

    private TextureRect _mouseBlocker;
    private ColorRect _screenBlocker;
    private bool _hibernating = true;

    private int _consoleBusIndex = AudioServer.GetBusIndex("Console");
    private int _windowBusIndex = AudioServer.GetBusIndex("Window");

    public override void _Ready()
    {
        base._Ready();
        TargetPosition = Position;
        _mouseBlocker = GetNode<TextureRect>("MouseBlocker");
        _screenBlocker = GetNode<ColorRect>("ScreenBlocker");
        _screenBlocker.Visible = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        HandleZoom(delta);
        MoveToTarget(delta);
        HandleSwitching();
        HandleHibernationVisibility(delta);
        HandleVolume();
    }

    private void HandleVolume()
    {
        float distConsole = Math.Min(Position.DistanceTo(CONSOLE_POSITION), Position.DistanceTo(RIGHT_POSITION))/MAX_DISTANCE;
        float distWindow = Math.Min(Position.DistanceTo(WINDOW_POSITION), Position.DistanceTo(LEFT_POSITION)) / MAX_DISTANCE;

        float consoleReduction = distConsole * -TurnVolumeReductionDB;
        float windowReduction = distWindow * -TurnVolumeReductionDB;

        float zoomReduction = (Zoom.X - ZoomFactor) / (1 - ZoomFactor) * ZoomVolumeReductionDB;

        if (distConsole < 0.1) consoleReduction -= zoomReduction;
        else consoleReduction -= ZoomVolumeReductionDB; // maxed out zoom reduction

        if (distWindow < 0.1) windowReduction -= zoomReduction;
        else windowReduction -= ZoomVolumeReductionDB; // maxed out zoom reduction

        AudioServer.SetBusVolumeDb(_consoleBusIndex, consoleReduction);
        AudioServer.SetBusVolumeDb(_windowBusIndex, windowReduction);
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

    private void HandleHibernationVisibility(double delta)
    {
        if (_hibernating && _screenBlocker.Size.Y < 648)
        {
            _screenBlocker.Visible = true;
            _screenBlocker.Size = new Vector2(_screenBlocker.Size.X, _screenBlocker.Size.Y + 648 * (float)delta);
            _screenBlocker.Position = new Vector2(_screenBlocker.Position.X, _screenBlocker.Position.Y - 648 * (float)delta / 2);
        }
        if (!_hibernating && _screenBlocker.Size.Y > 10)
        {
            _screenBlocker.Size = new Vector2(_screenBlocker.Size.X, _screenBlocker.Size.Y - 648 * (float)delta);
            _screenBlocker.Position = new Vector2(_screenBlocker.Position.X, _screenBlocker.Position.Y + 648 * (float)delta / 2);
        }
        else if (!_hibernating && _screenBlocker.Size.Y <= 10)
        {
            _screenBlocker.Visible = false;
        }
    }
    
    public bool Turn(string direction)
    {
        if (_hibernating) return false;
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

    public async void StartHibernation()
    {
        if (!FacingConsole) Turn("left");
        
        // all the technical stuff required to enter hibernation
        _hibernating = true;
        _mouseBlocker.MouseFilter = MouseFilterEnum.Stop;
        _mouseBlocker.MouseForcePassScrollEvents = false;

        // all the visual stuff required to enter hibernation
        await ToSignal(GetTree().CreateTimer(1), "timeout");
        
        EmitSignal(SignalName.HibernationStarted);
    }
    
    public async void  EndHibernation()
    {
        // all the technical stuff required to leave hibernation
        _hibernating = false;
        _mouseBlocker.MouseFilter = MouseFilterEnum.Ignore;
        _mouseBlocker.MouseForcePassScrollEvents = true;
        
        // all the visual stuff required to leave hibernation
        await ToSignal(GetTree().CreateTimer(1), "timeout");
        
        EmitSignal(SignalName.HibernationEnded);
    }
}
