using Godot;
using System;

public partial class BlackScreen : ColorRect
{
    private Tween tween;

    public override void _Ready()
    {
        // Make input transparent so it doesn't block mouse events`
        MouseFilter = Control.MouseFilterEnum.Ignore;

        // Start transparent
        Color = new Color(0, 0, 0, 0);

        // Create tween for smooth transitions
        tween = CreateTween();
        tween.Stop();
    }

    public void FadeToBlack()
    {
        tween.Kill();
        tween = CreateTween();

        // fade to black over 3 seconds
        tween.TweenProperty(this, "color", new Color(0, 0, 0, 1), 5.0f);
        // wait 4 more seconds (total 7 seconds)
        tween.TweenInterval(12.249f);
        // flash to white
        tween.TweenProperty(this, "color", new Color(1, 1, 1, 1), 0.01f);
        // fade to black over 5 seconds
        tween.TweenProperty(this, "color", new Color(0, 0, 0, 1), 14.0f);
    }
}
