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

    private VelvetTrial _velvetTrial;

    public override void _Ready()
    {
        _startButton.Pressed += StartVelvetTrial;
        
        foreach (VelvetTrialButton trialButton in _buttonContainer.Buttons)
        {
            trialButton.Selected += OnTrialSelected;
        }

        VelvetTrialButton firstTrialButton = _buttonContainer.Buttons[0];
        _velvetTrial = firstTrialButton.VelvetTrial;
        _trialInfo.SetVelvetTrial(firstTrialButton.VelvetTrial);
    }

    private void OnTrialSelected(VelvetTrial velvetTrial)
    {
        _velvetTrial = velvetTrial;
        _trialInfo.SetVelvetTrial(velvetTrial);
    }

    private void StartVelvetTrial()
    {
        EmitSignal(SignalName.Started, _velvetTrial);
    }
}
