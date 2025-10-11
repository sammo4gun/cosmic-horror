using Godot;
using System.Threading.Tasks;

public partial class HibernationHandler : Node
{
    [Export]
    float HibernationLength = 5.0f; // length of dark screen during hibernation

    public bool IsHibernating = false;
    private Camera _camera;

    public override void _Ready()
    {
        base._Ready();
        _camera = GetParent().GetNode<Camera>("Camera");
    }

    public async Task EnterHibernation(int TimePassed, string TimeUnit, int DistancePassed)
    {
        IsHibernating = true;
        _camera.StartHibernation();
        await ToSignal(_camera, "HibernationStarted");

        ((Shuttle)GetParent()).GetNode<TimeHandler>("TimeHandler").AddTime(TimePassed, TimeUnit);
        ((Shuttle)GetParent()).GetNode<SpaceHandler>("SpaceHandler").AddDistance(DistancePassed);

        await ToSignal(GetTree().CreateTimer(HibernationLength), "timeout");

        _camera.EndHibernation();
        await ToSignal(_camera, "HibernationEnded");

        IsHibernating = false;
    }
}
