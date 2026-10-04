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
    private PersonaTeam _personaTeam;

    [Export]
    private Label _errorLabel;

    private VelvetTrial _velvetTrial;

    public override void _EnterTree()
    {
        Signals.Instance.EmitSignal(Signals.SignalName.MoveOverlay);
    }

    public override void _Ready()
    {
        _startButton.Pressed += StartVelvetTrial;
        _personaTeam.PersonaRemoved += Refresh;
        _personaTeam.LoadoutSelected += Refresh;
        _buttonContainer.TrialSelected += OnTrialSelected;

        VelvetTrialButton firstTrialButton = _buttonContainer.GetFirstButton();
        firstTrialButton.Toggle(true);
        _velvetTrial = firstTrialButton.VelvetTrial;
        _trialInfo.SetVelvetTrial(firstTrialButton.VelvetTrial);

        Refresh();
    }

    private void OnTrialSelected(VelvetTrial velvetTrial)
    {
        _velvetTrial = velvetTrial;
        _trialInfo.SetVelvetTrial(velvetTrial);
    }

    private void StartVelvetTrial()
    {
        string errorMessage = Player.Instance.GetErrorMessage(_velvetTrial);
        bool isMessageEmpty = string.IsNullOrEmpty(errorMessage);
        if (!isMessageEmpty) 
        {
            // Show error message
            SetStartButton(errorMessage);
            return;
        }

        EmitSignal(SignalName.Started, _velvetTrial);
    }

    private void SetStartButton(string errorMessage = "")
    {
        bool isMessageEmpty = string.IsNullOrEmpty(errorMessage);

        Color targetModulate = isMessageEmpty ? new Color("#68c255") : new Color("#ff5645");
        _startButton.SetModulate(targetModulate);
        
        _errorLabel.Visible = !isMessageEmpty;
        _errorLabel.Text = errorMessage;
    }

    private void Refresh()
    {
        string errorMessage = Player.Instance.GetErrorMessage(_velvetTrial);
        bool isMessageEmpty = string.IsNullOrEmpty(errorMessage);
        if (!isMessageEmpty) 
        {
            SetStartButton(errorMessage);
        }
        else
        {
            SetStartButton(); 
        }
    }
}
