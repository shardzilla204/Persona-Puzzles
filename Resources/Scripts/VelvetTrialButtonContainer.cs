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
        CallDeferred(MethodName.SetScrollContainerSize);
        AddVelvetTrialButtons();
    }

    private void SetScrollContainerSize()
    {
        ScrollContainer scrollContainer = _buttonContainer.GetParent<ScrollContainer>();
        scrollContainer.Size = scrollContainer.GetParent<Control>().Size;
    }

    private void AddVelvetTrialButtons()
    {
        const string UID = "uid://bvxxdui3buniy";
        for (int i = 0; i < VelvetTrialManager.VelvetTrials.Count; i++)
        {
            VelvetTrialButton velvetTrialButton = GD.Load<PackedScene>(UID).Instantiate<VelvetTrialButton>();
            velvetTrialButton.Name = $"Trial {i + 1}";
            velvetTrialButton.VelvetTrial = VelvetTrialManager.VelvetTrials[i];
            _buttonContainer.AddChild(velvetTrialButton);
            Buttons.Add(velvetTrialButton);
        }

        // Add filler node
        _buttonContainer.AddChild(new Control());
    }
}
