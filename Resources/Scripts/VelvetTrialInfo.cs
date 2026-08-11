using Godot;
using System.Collections.Generic;

namespace PersonaAndPuzzles;

public partial class VelvetTrialInfo : Container
{
    [Export]
    private Container _personaIconContainer;

    [Export]
    private CustomButton _previousButton;

    [Export]
    private CustomButton _nextButton;

    [Export]
    private Label _waveCounterLabel;

    private VelvetTrial _velvetTrial;
    private int _waveIndex = 0;

    public override void _Ready()
    {
        _previousButton.Pressed += OnPreviousButtonPressed;
        _nextButton.Pressed += OnNextButtonPressed;
    }

    public void SetVelvetTrial(VelvetTrial velvetTrial)
    {
        _velvetTrial = velvetTrial;
        _waveIndex = 0;
        UpdateWaveInfo();
    }

    private void SetWaveCounterText(int waveCount)
    {
        _waveCounterLabel.Text = $"Wave {waveCount}";
    }

    private void SetPersonaIcons(List<Persona> personas)
    {
        foreach (Node child in _personaIconContainer.GetChildren())
        {
            child.QueueFree();  
        }

        foreach (Persona persona in personas)
        {
            VelvetTrialPersona velvetTrialPersona = GetVelvetTrialPersona(persona);
            _personaIconContainer.AddChild(velvetTrialPersona);
        }
    }  

    private VelvetTrialPersona GetVelvetTrialPersona(Persona persona)
    {
        const string UID = "uid://cj887nqlhdxlg";
        VelvetTrialPersona velvetTrialPersona = GD.Load<PackedScene>(UID).Instantiate<VelvetTrialPersona>();
        velvetTrialPersona.SetPersona(persona);

        return velvetTrialPersona;
    }

    private void UpdateWaveInfo()
    {
        VelvetTrialWave wave = _velvetTrial.Waves[_waveIndex];
        SetWaveCounterText(_waveIndex + 1);
        SetPersonaIcons(wave.Personas);
    }

     private void OnPreviousButtonPressed()
    {
        _waveIndex -= 1;

        int maxIndex = _velvetTrial.Waves.Count - 1;
        if (_waveIndex < 0)
        {
            _waveIndex = maxIndex;
        }

        UpdateWaveInfo();
    }

    private void OnNextButtonPressed()
    {
        _waveIndex += 1;

        int maxIndex = _velvetTrial.Waves.Count - 1;
        if (_waveIndex > maxIndex)
        {
            _waveIndex = 0;
        }

        UpdateWaveInfo();
    }
}
