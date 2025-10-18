using Godot;
using System.Threading.Tasks;

public partial class HibernationHandler : Node
{
    [Export]
    float HibernationLength = 4.0f; // length of dark screen during hibernation

    public bool IsHibernating = true;
    private Camera _camera;

    public override void _Ready()
    {
        base._Ready();
        _camera = GetParent().GetNode<Camera>("Camera");
    }

    public async Task EndHibernation(float delay, float speedFactor = 1.0f)
    {
        await ToSignal(GetTree().CreateTimer(delay), "timeout");
        _camera.EndHibernation(speedFactor);
        await ToSignal(_camera, "HibernationEnded");

        IsHibernating = false;
    }

    public async Task EnterHibernation(string newSceneName, float speedFactor = 1.0f)
    {
        IsHibernating = true;
        _camera.StartHibernation(speedFactor);
        await ToSignal(_camera, "HibernationStarted");

        await ToSignal(GetTree().CreateTimer(HibernationLength), "timeout");

        GetTree().ChangeSceneToFile($"res://Scenes/{newSceneName}.tscn");
    }
}
