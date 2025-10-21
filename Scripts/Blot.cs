using Godot;

public partial class Blot : ColorRect
{
    private Vector2 _desiredPosition;
    private Vector2 _desiredScale;
    private float _lerpSpeed = 0.5f;
    private float _scaleLerpSpeed = 0.5f;

    public override void _Ready()
    {
        _desiredPosition = GlobalPosition;
        _desiredScale = Scale;
    }

    public void SetMoveTarget(Vector2 target, float? speedOverride = null)
    {
        _desiredPosition = target;
        if (speedOverride.HasValue)
            _lerpSpeed = speedOverride.Value;
    }

    public void SetScaleTarget(float scale, float? speedOverride = null)
    {
        _desiredScale = new Vector2(scale, scale);
        if (speedOverride.HasValue)
            _scaleLerpSpeed = speedOverride.Value;
    }

    public override void _Process(double delta)
    {
        float t = (float)delta * _lerpSpeed;
        Position = Position.Lerp(_desiredPosition, t);

        float ts = (float)delta * _scaleLerpSpeed;
        Scale = Scale.Lerp(_desiredScale, ts);
    }
}
