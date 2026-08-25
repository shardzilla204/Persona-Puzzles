using System.Threading.Tasks;
using Godot;

namespace PersonaAndPuzzles;

public partial class PersonaEnemy : Container
{
    [Signal]
    public delegate void DefeatedEventHandler(PersonaEnemy enemy);

    [Signal]
    public delegate void TargetedEventHandler(PersonaEnemy enemy);

    [Signal]
    public delegate void CapturedEventHandler(PersonaEnemy enemy);

    [Export]
    private Button _targetButton;

    [Export]
    private TextureRect _targetTexture;

    [Export]
    private TextureRect _personaTexture;

    [Export]
    private TextureRect _cardTexture;

    [Export]
    private EnemyHealthBar _healthBar;

    public Persona Persona;
    public float CaptureRate;

    public override void _Ready()
    {
        _personaTexture.Texture = PersonaManager.GetPersonaImage(Persona);
        _cardTexture.Scale = Vector2.Zero;
        
        _targetButton.Toggled += OnTargetButtonToggled;

        _healthBar.SetMaxHealth(Persona);
    }

    public async Task TakeDamageAsync(int damage)
    {
        PrintRich.PrintLine($"{Persona.Name} Taking {damage} Damage", TextColor.Yellow);
        
        await _healthBar.TakeDamageAsync(damage);
    }

    public bool HasDefeated()
    {
        return _healthBar.HasDepleted();
    }

    private void OnTargetButtonToggled(bool isToggled)
    {
        ToggleButton(isToggled);
        EmitSignal(SignalName.Targeted, this);
    }

    public void ToggleButton(bool isToggled)
    {
        _targetTexture.Visible = isToggled;
        _targetButton.ButtonPressed = isToggled;
    }

    public async Task CaptureAsync()
    {
        _healthBar.Visible = false;

        await TweenPersonaCaptureAsync();
        await TweenCardCaptureAsync();

        EmitSignal(SignalName.Captured, this);
        QueueFree();
    }

    public async Task DefeatAsync()
    {
        _healthBar.Visible = false;

        await TweenDefeatAsync();

        EmitSignal(SignalName.Defeated, this);
        QueueFree();
    }

    public void TweenAttack()
    {
        const int OffsetY = 10;
        Vector2 startingPosition = _personaTexture.Position;
        Vector2 targetPosition = _personaTexture.Position - new Vector2(0, startingPosition.Y - OffsetY);

        const float Duration = 0.35f;
        Tween tween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetTrans(Tween.TransitionType.Circ);
        tween.TweenProperty(_personaTexture, "position", targetPosition, Duration);
        tween.Chain().TweenProperty(_personaTexture, "position", startingPosition, Duration);
    }

    private async Task TweenPersonaCaptureAsync()
    {
        Vector2 startingScale = Vector2.One;
        Vector2 targetScale = new Vector2(-startingScale.X, 1);

        const float Duration = 0.2f;
        Tween spinTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetLoops(2);
        spinTween.TweenProperty(_personaTexture, "scale", targetScale, Duration);
        spinTween.TweenProperty(_personaTexture, "scale", startingScale, Duration);

        await ToSignal(spinTween, Tween.SignalName.Finished);

        Tween tween = CreateTween();
        tween.TweenProperty(_personaTexture, "scale", new Vector2(0, 1), Duration);
    }

    private async Task TweenCardCaptureAsync()
    {
        Vector2 startingScale = Vector2.One;
        Vector2 targetScale = new Vector2(-startingScale.X, 1);

        const float Duration = 0.3f;
        Tween spinTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetLoops(2);
        spinTween.TweenProperty(_cardTexture, "scale", targetScale, Duration);
        spinTween.TweenProperty(_cardTexture, "scale", startingScale, Duration);

        const float OffsetY = 20;
        Vector2 targetPosition = new Vector2(_cardTexture.Position.X, _cardTexture.Position.Y - OffsetY);
        Color targetModulate = new Color(Colors.White, 0);

        Tween disappearTween = CreateTween().SetParallel(true);
        disappearTween.TweenProperty(_cardTexture, "position", targetPosition, 1f);
        disappearTween.TweenProperty(_cardTexture, "modulate", targetModulate, 1f);

        await ToSignal(spinTween, Tween.SignalName.Finished);

        Tween tween = CreateTween();
        tween.TweenProperty(_cardTexture, "scale", Vector2.One, Duration);

        await ToSignal(tween, Tween.SignalName.Finished);
    }

    private async Task TweenDefeatAsync()
    {
        Color targetModulate = new Color(Colors.White, 0);

        const float Duration = 0.3f;
        Tween tween = CreateTween().SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(_personaTexture, "modulate", targetModulate, Duration);

        await ToSignal(tween, Tween.SignalName.Finished);
    }

    public double GetHealth()
    {
        return _healthBar.Value;
    }

    public bool HasCaptured()
    {
        return true;
        RandomNumberGenerator RNG = new RandomNumberGenerator();
        float value = RNG.RandfRange(0, 100);

        return CaptureRate > value;
    }
}
