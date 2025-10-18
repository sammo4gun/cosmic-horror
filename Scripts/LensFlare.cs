using Godot;

public partial class LensFlare : ColorRect
{
    [Export] public Node2D Sun; // Assign your Sun node in the inspector

    public override void _Process(double delta)
    {
        if (Sun == null) return;
        if (Material is not ShaderMaterial sm) return;

        // Convert world position of Sun to screen space (pixels)
        var viewport = GetViewport();
        var cam = viewport.GetCamera2D();
        Vector2 sunScreen = Sun.Position;

        if (cam != null)
        {
            Vector2 vpSize = viewport.GetVisibleRect().Size;
            // Top-left of screen in world space
            Vector2 worldTopLeft = cam.Position - vpSize * 0.5f;
            sunScreen = Sun.Position - worldTopLeft;
        }

        // If your shader expects normalized (0..1) coordinates, uncomment:
        // sunScreen /= GetViewport().GetVisibleRect().Size;

        sm.SetShaderParameter("sun_position", sunScreen);
    }
}
