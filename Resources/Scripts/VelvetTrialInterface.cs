using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace PersonaAndPuzzles;

public partial class VelvetTrialInterface : Control
{
    [Export]
    private SkillOrbGrid _skillOrbGrid;

    [Export]
    private VelvetTrialSlotContainer _velvetTrialSlotContainer;

    public override void _EnterTree()
    {
        SetSkillTypePool();
    }

    public override void _Ready()
    {
        _skillOrbGrid.SkillTypeIncreasedBySkillOrbs += OnSkillTypeIncreasedBySkillOrbs;
        _skillOrbGrid.SkillTypeIncreasedByCombo += OnSkillTypeIncreasedByCombo;
        _skillOrbGrid.FinishedCombos += OnFinishedCombos;
    }

    private void OnSkillTypeIncreasedBySkillOrbs(SkillType skillType, int skillOrbCount)
    {
        List<VelvetTrialSlot> slots = _velvetTrialSlotContainer.FindSlots(skillType);
        foreach (VelvetTrialSlot slot in slots)
        {
            slot.IncreasePowerBySkillOrbs(skillOrbCount);
        }
    }

    private void OnSkillTypeIncreasedByCombo()
    {
        foreach (VelvetTrialSlot personaSlot in _velvetTrialSlotContainer.Slots)
        {
            personaSlot.IncreasePowerByCombo();
        }
    }

    private void OnFinishedCombos()
    {
        int totalPower = 0;
        foreach (VelvetTrialSlot personaSlot in _velvetTrialSlotContainer.Slots)
        {
            totalPower += personaSlot.Power;
        }

        foreach (VelvetTrialSlot personaSlot in _velvetTrialSlotContainer.Slots)
        {
            personaSlot.TweenAttack();
        }

        GD.Print($"Total Power: {totalPower}");
    }

    private void SetSkillTypePool()
    {
        List<SkillType> skillTypes = new List<SkillType>();
        foreach (VelvetTrialSlot personaSlot in _velvetTrialSlotContainer.Slots)
        {
            SkillType skillType = personaSlot.Persona.SkillType;
            skillTypes.Add(skillType);
        }

        List<SkillType> distinctSkillTypes = skillTypes.Distinct().ToList();
        List<SkillType> unusedSkillTypes = new List<SkillType>();

        int skillTypeCount = Enum.GetValues<SkillType>().Count() - 1;
        for (int i = 0; i < skillTypeCount; i++)
        {
            SkillType skillType = (SkillType) i;
            if (!distinctSkillTypes.Contains(skillType))
            {
                unusedSkillTypes.Add(skillType);
                GD.Print($"{skillType}");
            }
        }

        RandomNumberGenerator RNG = new RandomNumberGenerator();
        while (distinctSkillTypes.Count <= SkillOrbGrid.MaxSkillTypes)
        {
            int unusedSkillTypeIndex = RNG.RandiRange(0, unusedSkillTypes.Count - 1);
            SkillType unusedSkillType = unusedSkillTypes[unusedSkillTypeIndex];
            unusedSkillTypes.RemoveAt(unusedSkillTypeIndex);
            distinctSkillTypes.Add(unusedSkillType);
        }

        _skillOrbGrid.SkillTypePool = distinctSkillTypes;
    }
}
