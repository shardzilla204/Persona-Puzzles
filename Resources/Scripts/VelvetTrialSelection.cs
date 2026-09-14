using Godot;

namespace PersonaAndPuzzles;

public partial class VelvetTrialSelection : Control
{
    [Signal]
    public delegate void StartedEventHandler(VelvetTrial velvetTrial);

    [Export]
    private VelvetTrialButtonContainer _buttonContainer;

    [Export]
    private VelvetTrialInfo _trialInfo;

    [Export]
    private CustomButton _startButton;

    [Export]
    private Label _errorLabel;

    private VelvetTrial _velvetTrial;
    private bool _isOverMaxPersonaCount;

    public override void _EnterTree()
    {
        PersonaAndPuzzles.Signals.EmitSignal(Signals.SignalName.MoveOverlay);
    }

    public override void _Ready()
    {
        _isOverMaxPersonaCount = PersonaManager.Personas.Count > PersonaManager.MaxPersonas;
        
        Color targetModulate = _isOverMaxPersonaCount ? new Color("ff5645") : new Color("68c255");
        _startButton.SetModulate(targetModulate);

        _startButton.Pressed += StartVelvetTrial;
        
        foreach (VelvetTrialButton trialButton in _buttonContainer.Buttons)
        {
            trialButton.Selected += OnTrialSelected;
        }

        VelvetTrialButton firstTrialButton = _buttonContainer.Buttons[0];
        _velvetTrial = firstTrialButton.VelvetTrial;
        _trialInfo.SetVelvetTrial(firstTrialButton.VelvetTrial);

        SetErrorLabel();
    }

    private void OnTrialSelected(VelvetTrial velvetTrial)
    {
        _velvetTrial = velvetTrial;
        _trialInfo.SetVelvetTrial(velvetTrial);
    }

    private void StartVelvetTrial()
    {
        if (_isOverMaxPersonaCount) return;

        EmitSignal(SignalName.Started, _velvetTrial);
    }

    private void SetErrorLabel()
    {
        _errorLabel.Visible = _isOverMaxPersonaCount;
        _errorLabel.Text = "Too Many Personas In Compendium";
    }
}
