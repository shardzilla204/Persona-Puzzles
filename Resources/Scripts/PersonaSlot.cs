using Godot;

namespace PersonaAndPuzzles;

public partial class PersonaSlot : TextureRect
{
    [Export]
    private TextureRect _personaIcon;

    [Export] 
    private TextureRect _skillTypeIcon;

    [Export]
    private Label _powerLabel;

    public Persona Persona;

    public int Power = 0;

    public override void _Ready()
    {
        Persona randomPersona = PersonaManager.GetRandomPersona();
        SetPersona(randomPersona);
    }

    public void SetPersona(Persona persona)
    {
        Persona = persona;
        _personaIcon.Texture = PersonaManager.GetPersonaImage(persona);
        SetSkillType(persona.SkillType);
    }

    private void SetSkillType(SkillType skillType)
    {
        Color skillTypeColor = SkillOrbManager.GetSkillTypeColor(skillType);
        SelfModulate = skillTypeColor;

        Texture2D skillTypeTexture = SkillOrbManager.GetSkillTypeTexture(skillType);
        _skillTypeIcon.Texture = skillTypeTexture;
    }

    public void IncreasePowerBySkillOrbs(int skillOrbCount)
    {
        int powerAcummulated = 0;
        int powerIncrement = 10;
        for (int i = 0; i < skillOrbCount; i++)
        {
            powerAcummulated += powerIncrement;
        }

        Power += powerAcummulated;

        TweenPowerIncrease();
        _powerLabel.Text = $"{Power}";
    }

    public void IncreasePowerByCombo()
    {
        Power = Mathf.RoundToInt(Power * 1.1f);

        TweenPowerIncrease();
        _powerLabel.Text = Power != 0 ? $"{Power}" : "";
    }

    private void TweenPowerIncrease()
    {
        Vector2 startingPosition = _powerLabel.Position;
        Vector2 startingScale = _powerLabel.Scale;
        
        Vector2 targetPosition = startingPosition + new Vector2(0, -10);
        Vector2 targetScale = startingScale + new Vector2(1.25f, 1.25f);

        Tween tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(_powerLabel, "position", targetPosition, ComboLabel.TweenDuration / 2);
        tween.TweenProperty(_powerLabel, "scale", targetScale, ComboLabel.TweenDuration / 2);
        tween.Chain().TweenProperty(_powerLabel, "position", startingPosition, ComboLabel.TweenDuration / 2);
        tween.TweenProperty(_powerLabel, "scale", startingScale, ComboLabel.TweenDuration / 2);
    }

    public void TweenAttack()
    {
        if (Power == 0) return;

        TweenSelfAttack();
        TweenPowerLabelAttack();
    }

    private void TweenSelfAttack()
    {
        Vector2 startingPosition = Position;

        Vector2 targetPositionA = startingPosition + new Vector2(0, 10);
        Vector2 targetPositionB = startingPosition + new Vector2(0, -10);

        int propertyTweenerCount = 3;
        float duration = 0.5f / propertyTweenerCount;
        Tween tween = CreateTween().SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(this, "position", targetPositionA, duration);
        tween.TweenProperty(this, "position", targetPositionB, duration);
        tween.TweenProperty(this, "position", startingPosition, duration);
    }

    private void TweenPowerLabelAttack()
    {
        Vector2 targetPosition = new Vector2(0, -10);
        Vector2 targetScale = new Vector2(1.25f, 1.25f);
        Color targetModulate = Colors.White;
        targetModulate.A = 0;

        int propertyTweenerCount = 3;
        float duration = 0.5f / propertyTweenerCount;
        Tween tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(_powerLabel, "position", targetPosition, duration);
        tween.TweenProperty(_powerLabel, "scale", targetScale, duration);
        tween.TweenProperty(_powerLabel, "modulate", targetModulate, duration);
        tween.Finished += () =>
        {
            Power = 0;
            _powerLabel.Text = "";
        };
    }
}
