using Godot;

namespace PersonaAndPuzzles;

public partial class ComboLabel : Label
{
    private float _offsetY = 25;

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

        float duration = 0.5f;
        Tween tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(this, "position", targetPosition, duration);
        tween.TweenProperty(this, "modulate", targetModulate, duration);
    }

    public async void TweenExit()
    {
        Vector2 targetPosition = Position + new Vector2(0, -_offsetY);

        Color targetModulate = Colors.White;
        targetModulate.A = 0;

        float targetScale = 1.1f;

        float duration = 0.5f;
        Tween tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(this, "position", targetPosition, duration);
        tween.TweenProperty(this, "modulate", targetModulate, duration);
        tween.TweenProperty(this, "scale", new Vector2(targetScale, targetScale), duration);
        tween.Finished += QueueFree;
    }
}
