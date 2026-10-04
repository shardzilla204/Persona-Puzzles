using Godot;

namespace PersonaAndPuzzles;

public partial class VelvetTrialButton : CustomButton
{
    [Signal]
    public delegate void SelectedEventHandler(VelvetTrialButton velvetTrialButton);

    [Export]
    private Label _nameLabel;

    [Export]
    private Label _staminaLabel;

    public VelvetTrial VelvetTrial;
    
    private bool _isSelected;

    public override void _Ready()
    {
        base._Ready();

        Pressed += OnButtonPressed;
        
        _nameLabel.Text = Name;
        _staminaLabel.Text = $"{VelvetTrial.Stamina}";
    }

    protected override void OnMouseEntered()
    {
        if (_isSelected) return;

        base.OnMouseEntered();
    }   

    protected override void OnMouseExited()
    {
        if (_isSelected) return;

        base.OnMouseExited();
    }

    private void OnButtonPressed()
    {
        if (_isSelected) return;

        EmitSignal(SignalName.Selected, this);
    }
}
