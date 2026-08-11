using Godot;
using System;

namespace PersonaAndPuzzles;

public partial class VelvetTrialButton : Control
{
    [Signal]
    public delegate void SelectedEventHandler(VelvetTrial velvetTrial);

    [Export]
    private CustomButton _button;

    [Export]
    private Label _name;

    public VelvetTrial VelvetTrial;

    public override void _Ready()
    {
        _button.Pressed += OnButtonPressed;
        _name.Text = Name;
    }

    private void OnButtonPressed()
    {
        EmitSignal(SignalName.Selected, VelvetTrial);
    }
}
