using Godot;

namespace PersonaAndPuzzles;

public partial class CompendiumEnhance : Control
{
    [Export]
    private PersonaEnhance _personaEnhance;

    [Export]
    private CustomButton _backButton;

    public override void _EnterTree()
    {
        PersonaAndPuzzles.Signals.EmitSignal(Signals.SignalName.MoveOverlay);
    }

    public override void _Ready()
    {
        _personaEnhance.PersonaLevelMaxed += AddOverclockEnhance;
        _backButton.Pressed += OnBackButtonPressed;

        AddMaterialEnhance();
    }

    private void AddMaterialEnhance()
    {
        const string UID = "uid://be0e0p3g5ingh";
        MaterialEnhance materialEnhance = GD.Load<PackedScene>(UID).Instantiate<MaterialEnhance>();
        materialEnhance.Connect(MaterialEnhance.SignalName.MaterialsCleared, 
            Callable.From(OnMaterialsCleared));
        materialEnhance.Connect(MaterialEnhance.SignalName.EnhancedPersona, 
            new Callable(this, MethodName.OnEnhancedPersona));
        materialEnhance.Connect(MaterialEnhance.SignalName.MaterialsUpdated,
            new Callable(this, MethodName.OnMaterialsUpdated));

        _personaEnhance.AddSibling(materialEnhance);
        _personaEnhance.Connect(PersonaEnhance.SignalName.PersonaLevelPreviewMaxed,
            new Callable(materialEnhance, MaterialEnhance.MethodName.OnPersonaLevelMaxed));
        _personaEnhance.Connect(PersonaEnhance.SignalName.ConvertedMaterials,
            new Callable(materialEnhance, MaterialEnhance.MethodName.AddConvertedMaterials));
        _personaEnhance.Connect(PersonaEnhance.SignalName.PersonaLevelMaxed,
            Callable.From(materialEnhance.QueueFree));
    }

    private void AddOverclockEnhance()
    {
        int overclockCount = PersonaManager.GetOverclockCount(_personaEnhance.Persona.Rank);
        if (_personaEnhance.Persona.Overclock >= overclockCount) return;

        const string UID = "uid://xm335fp66js3";
        OverclockEnhance overclockEnhance = GD.Load<PackedScene>(UID).Instantiate<OverclockEnhance>();
        overclockEnhance.Persona = _personaEnhance.Persona;
        overclockEnhance.Connect(OverclockEnhance.SignalName.Overclocked,
            new Callable(this, MethodName.OnOverclocked));

        _personaEnhance.AddSibling(overclockEnhance);
    }

    private void OnMaterialsCleared()
    {
        _personaEnhance.TweenExperience(0);
        _personaEnhance.SetExperienceText(0);
    }

    private void OnEnhancedPersona(int totalExperience)
    {
        int maxLevel = PersonaManager.GetMaxLevel(_personaEnhance.Persona);
        if (_personaEnhance.Persona.Level >= maxLevel) return;
        
        _personaEnhance.GiveExperience(totalExperience);
    }

    private void OnMaterialsUpdated(int totalExperience)
    {
        _personaEnhance.TweenTargetExperience(_personaEnhance.Persona.Experience + totalExperience);
        _personaEnhance.SetExperienceText(totalExperience);
    }

    private void OnOverclocked()
    {
        _personaEnhance.Refresh();
        _personaEnhance.RefreshOverclock();

        AddMaterialEnhance();
    }

    private int GetExperience(EnhancementMaterial material) => material switch
    {
        EnhancementMaterial.Memory => 100,
        EnhancementMaterial.Anecdote => 500,
        EnhancementMaterial.Legend => 2000,
        _ => 0
    };

    public void SetPersona(Persona persona)
    {
        _personaEnhance.Persona = persona;
    }

    public void IsPersonaMaxLevel()
    {
        
    }

    private void OnBackButtonPressed()
    {
        const string UID = "uid://dos6wimuvhfa7";
        PersonaStats personaStats = GD.Load<PackedScene>(UID).Instantiate<PersonaStats>();
        personaStats.Set(_personaEnhance.Persona);
        GetParent().AddChild(personaStats);

        QueueFree();
    }
}
