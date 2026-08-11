using Godot;
using System.Collections.Generic;

namespace PersonaAndPuzzles;

public partial class VelvetTrialButtonContainer : Control
{
    [Export]
    private Container _buttonContainer;

    public List<VelvetTrialButton> Buttons = new List<VelvetTrialButton>();

    public override void _Ready()
    {
        AddVelvetTrialButtons();
    }

    private void AddVelvetTrialButtons()
    {
        const string UID = "uid://bvxxdui3buniy";
        for (int i = 0; i < VelvetTrialManager.VelvetTrials.Count; i++)
        {
            VelvetTrialButton velvetTrialButton = GD.Load<PackedScene>(UID).Instantiate<VelvetTrialButton>();
            velvetTrialButton.Name = $"Velvet Trial {i + 1}";
            velvetTrialButton.VelvetTrial = VelvetTrialManager.VelvetTrials[i];
            _buttonContainer.AddChild(velvetTrialButton);
            Buttons.Add(velvetTrialButton);
        }
    }
}
