using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace PersonaAndPuzzles;

public partial class VelvetTrialSlot : PersonaSlot
{
    [Export]
    private Label _powerLabel;

    public int Power = 0;

    private const float _Offset = 10;
    private const float _Scale = 1.25f;

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
        
        Vector2 targetPosition = startingPosition + new Vector2(0, -_Offset);
        Vector2 targetScale = startingScale + new Vector2(_Scale, _Scale);

        float duration = ComboLabel.TweenDuration / 2;
        Tween tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(_powerLabel, "position", targetPosition, duration);
        tween.TweenProperty(_powerLabel, "scale", targetScale, duration);
        tween.Chain().TweenProperty(_powerLabel, "position", startingPosition, duration);
        tween.TweenProperty(_powerLabel, "scale", startingScale, duration);
    }

    public async Task TweenAttackAsync()
    {
        if (Power == 0) return;

        TweenAttack();
        TweenPowerLabel();
    }

    private void TweenAttack()
    {
        Vector2 startingPosition = Position;

        Vector2 targetPositionA = startingPosition + new Vector2(0, _Offset);
        Vector2 targetPositionB = startingPosition + new Vector2(0, -_Offset);

        const float Duration = 0.2f;
        Tween tween = CreateTween().SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(this, "position", targetPositionA, Duration);
        tween.TweenProperty(this, "position", targetPositionB, Duration);
        tween.TweenProperty(this, "position", startingPosition, Duration);
    }

    private void TweenPowerLabel()
    {
        Vector2 startingPosition = _powerLabel.Position;
        Vector2 startingScale = _powerLabel.Scale;

        Vector2 targetPosition = new Vector2(0, -_Offset);
        Vector2 targetScale = new Vector2(_Scale, _Scale);

        const float Duration = 0.2f;
        Tween tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(_powerLabel, "position", targetPosition, Duration);
        tween.TweenProperty(_powerLabel, "scale", targetScale, Duration);
        tween.Chain().TweenProperty(_powerLabel, "position", startingPosition, Duration);
        tween.TweenProperty(_powerLabel, "scale", startingScale, Duration);
        tween.Finished += () =>
        {
            Power = 0;
            _powerLabel.Text = "";
        };
    }
}
