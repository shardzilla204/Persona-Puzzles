using Godot;
using Godot.Collections;

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
        _previousButton.Pressed += DecreaseWaveIndex;
        _nextButton.Pressed += IncreaseWaveIndex;
    }

    public void SetVelvetTrial(VelvetTrial velvetTrial)
    {
        _velvetTrial = velvetTrial;
        _waveIndex = 0;
        UpdateWaveInfo();
    }

    private void SetWaveCounterText(int waveCount)
    {
        _waveCounterLabel.Text = $"Wave {waveCount}/{_velvetTrial.Waves.Count}";
    }

    private void SetPersonaIcons(Array<Dictionary<string, Variant>> personaDictionaries)
    {
        foreach (Node child in _personaIconContainer.GetChildren())
        {
            child.QueueFree();  
        }

        const string NameKey = "Name";
        foreach (Dictionary<string, Variant> personaDictionary in personaDictionaries)
        {
            string personaName = personaDictionary[NameKey].As<string>();
            Persona persona = PersonaManager.GetPersonaByName(personaName);
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
        SetPersonaIcons(wave.PersonaDictionaries);
    }

     private void DecreaseWaveIndex()
    {
        _waveIndex -= 1;

        int maxIndex = _velvetTrial.Waves.Count - 1;
        if (_waveIndex < 0)
        {
            _waveIndex = maxIndex;
        }

        UpdateWaveInfo();
    }

    private void IncreaseWaveIndex()
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
