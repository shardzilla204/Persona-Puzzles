using System.Collections.Generic;
using Godot;

namespace PersonaAndPuzzles;

public partial class VelvetTrialResult : ColorRect
{
    [Signal]
    public delegate void RestartedEventHandler();

    [Signal]
    public delegate void ExitedEventHandler();

    [Export]
    private Label _statusLabel;

    [Export]
    private Control _playerExperience;

    [Export]
    private Label _levelLabel;

    [Export]
    private TextureProgressBar _experienceProgressBar;

    [Export]
    private Label _experienceGainedLabel;

    [Export]
    private Control _konpakuGems;

    [Export]
    private Label _konpakuGemsLabel;

    [Export]
    private Control _capturedPersonas;

    [Export]
    private Container _capturedPersonasContainer;

    [Export]
    private Label _errorLabel;

    [Export]
    private Label _staminaLabel;

    [Export]
    private CustomButton _restartButton;

    [Export]
    private CustomButton _exitButton;

    public VelvetTrial VelvetTrial;

    public override void _Ready()
    {
        _statusLabel.Visible = false;
        
        _restartButton.Pressed += OnRestartButtonPressed;
        _exitButton.Pressed += OnExitButtonPressed;

        SetExperience();
        SetKonpakuGemsLabel();
        SetStaminaLabel();
    }

    public void SetStatusLabel(string status)
    {
        _statusLabel.Visible = true;
        _statusLabel.Text = $"{status}";
    }

    private void SetExperience()
    {
        _levelLabel.Text = $"{Player.Instance.Level}";
        _experienceGainedLabel.Text = $"+ {VelvetTrial.Experience} EXP";
        
        _experienceProgressBar.Value = Player.Instance.Experience;
        _experienceProgressBar.MaxValue = Player.Instance.MaxExperience;
    }

    private void SetKonpakuGemsLabel()
    {
        _konpakuGemsLabel.Text = $"+ {VelvetTrial.KonpakuGems}";
    }

    private void SetStaminaLabel()
    {
        _staminaLabel.Text = $"{VelvetTrial.Stamina}";
    }

    public void AddCapturedPersonas(List<Persona> capturedPersonas)
    {
        if (capturedPersonas.Count == 0)
        {
            _capturedPersonas.Visible = false;
        }
        else
        {
            const string UID = "uid://bx43agqlgfr80";
            foreach (Persona persona in capturedPersonas)
            {
                CompendiumSlot slot = GD.Load<PackedScene>(UID).Instantiate<CompendiumSlot>();
                slot.SetPersona(persona);

                _capturedPersonasContainer.AddChild(slot);
            }  
        }
    }

    private void OnRestartButtonPressed()
    {
        EmitSignal(SignalName.Restarted);
    }

    private void OnExitButtonPressed()
    {
        EmitSignal(SignalName.Exited);
    }

    public void DisableRestartButton(string errorMessage)
    {
        _errorLabel.Text = errorMessage;

        if (string.IsNullOrEmpty(errorMessage)) return;

        GD.Print($"Error Message: {errorMessage}");
        _restartButton.Disabled = true;
    }

    public void SetLoss()
    {
        _playerExperience.Visible = false;
        _konpakuGems.Visible = false;
        _capturedPersonas.Visible = false;
    }
}
