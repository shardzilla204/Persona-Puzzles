using System.Collections.Generic;
using Godot;

namespace PersonaAndPuzzles;

public partial class VelvetTrial : Control
{
    [Export]
    private SkillOrbGrid _skillOrbGrid;

    [Export]
    private PersonaSlotContainer _personaSlotContainer;

    public override void _Ready()
    {
        _skillOrbGrid.SkillTypeIncreasedBySkillOrbs += OnSkillTypeIncreasedBySkillOrbs;
        _skillOrbGrid.SkillTypeIncreasedByCombo += OnSkillTypeIncreasedByCombo;
        _skillOrbGrid.FinishedCombos += OnFinishedCombos;
    }

    private void OnSkillTypeIncreasedBySkillOrbs(SkillType skillType, int skillOrbCount)
    {
        List<PersonaSlot> personaSlots = _personaSlotContainer.FindPersonaSlots(skillType);
        foreach (PersonaSlot personaSlot in personaSlots)
        {
            personaSlot.IncreasePowerBySkillOrbs(skillOrbCount);
        }
    }

    private void OnSkillTypeIncreasedByCombo()
    {
        foreach (PersonaSlot personaSlot in _personaSlotContainer.PersonaSlots)
        {
            personaSlot.IncreasePowerByCombo();
        }
    }

    private void OnFinishedCombos()
    {
        int totalPower = 0;
        foreach (PersonaSlot personaSlot in _personaSlotContainer.PersonaSlots)
        {
            totalPower += personaSlot.Power;
        }

        foreach (PersonaSlot personaSlot in _personaSlotContainer.PersonaSlots)
        {
            personaSlot.TweenAttack();
        }

        GD.Print($"Total Power: {totalPower}");
    }
}
