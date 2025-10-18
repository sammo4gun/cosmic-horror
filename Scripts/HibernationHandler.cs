using Godot;
using System.Threading.Tasks;

public partial class HibernationHandler : Node
{
    [Export]
    float HibernationLength = 2.0f; // length of dark screen during hibernation

    public bool IsHibernating = true;
    private Camera _camera;

    public override void _Ready()
    {
        base._Ready();
        _camera = GetParent().GetNode<Camera>("Camera");
        _ = EndHibernation();
    }

    public async Task EndHibernation()
    {
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
        _camera.EndHibernation();
        await ToSignal(_camera, "HibernationEnded");

        IsHibernating = false;
    }

    public async Task EnterHibernation(string newSceneName)
    {
        IsHibernating = true;
        _camera.StartHibernation();
        await ToSignal(_camera, "HibernationStarted");

        await ToSignal(GetTree().CreateTimer(HibernationLength), "timeout");

        GetTree().ChangeSceneToFile($"res://Scenes/{newSceneName}.tscn");
    }
}
