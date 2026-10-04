using Godot;
using System.Collections.Generic;

namespace PersonaAndPuzzles;

public partial class VelvetTrialButtonContainer : Control
{
    [Signal]
    public delegate void TrialSelectedEventHandler(VelvetTrial velvetTrial);

    [Export]
    private Container _buttonContainer;

    private List<VelvetTrialButton> _buttons = new List<VelvetTrialButton>();

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
        for (int i = 0; i < VelvetTrialManager.Instance.VelvetTrials.Count; i++)
        {
            Control wrapper = new Control();
            VelvetTrialButton velvetTrialButton = GD.Load<PackedScene>(UID).Instantiate<VelvetTrialButton>();
            velvetTrialButton.Name = $"Trial {i + 1}";
            velvetTrialButton.VelvetTrial = VelvetTrialManager.Instance.VelvetTrials[i];
            velvetTrialButton.Connect(VelvetTrialButton.SignalName.Selected, new Callable(this, MethodName.OnTrialSelected));
            wrapper.AddChild(velvetTrialButton);
            _buttonContainer.AddChild(wrapper);
            _buttons.Add(velvetTrialButton);
        }

        // Add filler node
        _buttonContainer.AddChild(new Control());
    }

    private void OnTrialSelected(VelvetTrialButton selectedButton)
    {
        foreach (VelvetTrialButton button in _buttons)
        {
            button.Toggle(false);
        }
        selectedButton.Toggle(true);
        EmitSignal(SignalName.TrialSelected, selectedButton.VelvetTrial);
    }

    public VelvetTrialButton GetFirstButton()
    {
        return _buttons[0];
    }
}
