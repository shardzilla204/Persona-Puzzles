using Godot;

namespace PersonaAndPuzzles;

public partial class ComboLabel : Label
{
    private float _offsetY = 25;
    public const float TweenDuration = 0.4f;

    public override void _Ready()
    {
        TweenEnter();
    }

    public void TweenEnter()
    {
        Color startModulate = Colors.White;
        startModulate.A = 0;
        Modulate = startModulate;
        
        Vector2 startPosition = Position + new Vector2(0, _offsetY);
        Position = startPosition;

        Vector2 targetPosition = Position + new Vector2(0, -_offsetY);
        Color targetModulate = Colors.White;

        Tween tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(this, "position", targetPosition, TweenDuration);
        tween.TweenProperty(this, "modulate", targetModulate, TweenDuration);
    }

    public async void TweenExit()
    {
        Vector2 targetPosition = Position + new Vector2(0, -_offsetY);

        Color targetModulate = Colors.White;
        targetModulate.A = 0;

        float targetScale = 1.1f;

        Tween tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(this, "position", targetPosition, TweenDuration);
        tween.TweenProperty(this, "modulate", targetModulate, TweenDuration);
        tween.TweenProperty(this, "scale", new Vector2(targetScale, targetScale), TweenDuration);
        tween.Finished += QueueFree;
    }
}
