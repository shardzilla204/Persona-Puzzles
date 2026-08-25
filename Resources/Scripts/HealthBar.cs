using System.Threading.Tasks;
using Godot;

namespace PersonaAndPuzzles;

public abstract partial class HealthBar : TextureProgressBar
{
    [Signal]
    public delegate void DepletedEventHandler();

    [Export]
    protected Label _healthLabel;

    public const float TweenDuration = 0.5f;

    public bool HasDepleted()
    {
        if (Value <= 0)
        {
            EmitSignal(SignalName.Depleted);
            return true;
        }
        return false;
    }

    public async Task TakeDamageAsync(int damage)
    {
        double targetValue = Mathf.Min(Value - damage, MaxValue);
        _healthLabel.Text = $"{targetValue} / {MaxValue}";

        await TweenValue(targetValue);
    }

    private async Task TweenValue(double targetValue)
    {
        Tween tween = CreateTween().SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(this, "value", targetValue, TweenDuration);
        tween.Finished += () => Value = targetValue;
        
        await ToSignal(tween, Tween.SignalName.Finished);
    }
}
