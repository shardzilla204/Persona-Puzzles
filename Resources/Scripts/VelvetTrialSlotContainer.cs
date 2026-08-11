using Godot;
using System.Collections.Generic;

namespace PersonaAndPuzzles;

public partial class VelvetTrialSlotContainer : Container
{
	public List<VelvetTrialSlot> Slots = new List<VelvetTrialSlot>();

	public override void _Ready()
	{
		RemoveChildren();
		AddVelvetTrialSlots();
	}

	public List<VelvetTrialSlot> FindSlots(SkillType skillType)
	{
		return Slots.FindAll(slot => slot.Persona.SkillType == skillType);
	}

	private void AddVelvetTrialSlots()
	{
		const string UID = "uid://djjy1hqpufkop";
		foreach (Persona persona in PersonaManager.Roster.Personas)
		{
			VelvetTrialSlot velvetTrialSlot = GD.Load<PackedScene>(UID).Instantiate<VelvetTrialSlot>();
			velvetTrialSlot.SetPersona(persona);
			AddChild(velvetTrialSlot);
			Slots.Add(velvetTrialSlot);
		}
	}

	private void RemoveChildren()
	{
		foreach (Node child in GetChildren())
		{
			child.QueueFree();
		}
		Slots.Clear();
	}
}
