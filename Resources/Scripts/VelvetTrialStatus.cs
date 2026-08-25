using Godot;

namespace PersonaAndPuzzles;

public partial class VelvetTrialStatus : ColorRect
{
    [Signal]
    public delegate void RestartedEventHandler();

    [Signal]
    public delegate void ExitedEventHandler();

    [Export]
    private Label _statusLabel;

    [Export]
    private CustomButton _restartButton;

    [Export]
    private CustomButton _exitButton;

    public override void _Ready()
    {
        _statusLabel.Visible = false;
        
        _restartButton.Pressed += OnRestartButtonPressed;
        _exitButton.Pressed += OnExitButtonPressed;
    }

    public void SetStatusLabel(string status)
    {
        _statusLabel.Visible = true;
        _statusLabel.Text = $"{status}";
    }

    private void OnRestartButtonPressed()
    {
        EmitSignal(SignalName.Restarted);
    }

    private void OnExitButtonPressed()
    {
        EmitSignal(SignalName.Exited);
    }
}
