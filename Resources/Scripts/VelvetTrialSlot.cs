using System.Collections.Generic;
using Godot;

namespace PersonaAndPuzzles;

public partial class VelvetTrialSlot : PersonaSlot
{
    [Export]
    private Label _powerLabel;

    public int Power = 0;

    public void IncreasePowerBySkillOrbs(int skillOrbCount)
    {
        int powerAccummulated = 0;
        int powerIncrement = GetPowerIncrement();
        for (int i = 0; i < skillOrbCount; i++)
        {
            powerAccummulated += powerIncrement;
        }

        Power += powerAccummulated;

        TweenPowerIncrease();
        _powerLabel.Text = $"{Power}";
    }

    public void IncreasePowerByCombo()
    {
        Power = Mathf.RoundToInt(Power * 1.1f);

        TweenPowerIncrease();
        _powerLabel.Text = Power != 0 ? $"{Power}" : "";
    }

    private int GetPowerIncrement()
    {
        List<PassiveSkill> offensiveSkills = PassiveSkillManager.GetOffensiveSkills(Persona);
        if (Persona.SkillType == SkillType.Physical || 
            Persona.SkillType == SkillType.Pierce)
        {
            return Persona.Strength;
        }
        else
        {
            return Persona.Magic;
        }
    }

    // Tweens
    private void TweenPowerIncrease()
    {
        Vector2 startingPosition = _powerLabel.Position;
        Vector2 startingScale = _powerLabel.Scale;
        
        Vector2 targetPosition = startingPosition + new Vector2(0, -10);
        Vector2 targetScale = startingScale + new Vector2(1.25f, 1.25f);

        float duration = ComboLabel.TweenDuration / 2;
        Tween tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(_powerLabel, "position", targetPosition, duration);
        tween.TweenProperty(_powerLabel, "scale", targetScale, duration);
        tween.Chain().TweenProperty(_powerLabel, "position", startingPosition, duration);
        tween.TweenProperty(_powerLabel, "scale", startingScale, duration);
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
