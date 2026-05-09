using Godot;
using System;
using System.Collections.Generic;

namespace PersonaAndPuzzles;

public partial class PersonaSlotContainer : Control
{
	public List<PersonaSlot> PersonaSlots = new List<PersonaSlot>();

	public override void _Ready()
	{
		foreach (PersonaSlot personaSlot in GetChildren())
		{
			PersonaSlots.Add(personaSlot);
		}
	}

	public List<PersonaSlot> FindPersonaSlots(SkillType skillType)
	{
		return PersonaSlots.FindAll(slot => slot.Persona.SkillType == skillType);
	}
}
