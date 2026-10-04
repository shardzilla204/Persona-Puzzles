using System.Collections.Generic;
using Godot;

namespace PersonaAndPuzzles;

public partial class CompendiumEnhance : Control
{
    [Signal]
    public delegate void PersonaUpdatedEventHandler();
    [Export]
    private PersonaEnhance _personaEnhance;

    public override void _Ready()
    {
        _personaEnhance.PersonaLevelMaxed += AddOverclockEnhance;

        if (_personaEnhance.Persona.Level >= PersonaManager.Instance.GetMaxLevel(_personaEnhance.Persona))
        {
            AddOverclockEnhance();
        }
        else
        {
            AddMaterialEnhance();
        }
    }

    private void AddMaterialEnhance()
    {
        const string UID = "uid://be0e0p3g5ingh";
        MaterialEnhance materialEnhance = GD.Load<PackedScene>(UID).Instantiate<MaterialEnhance>();
        materialEnhance.Persona = _personaEnhance.Persona;
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
        List<Persona> personas = PersonaManager.Instance.Personas.FindAll(PersonaMatch);

        int overclockCount = PersonaManager.Instance.GetOverclockCount(_personaEnhance.Persona.Rank);
        if (_personaEnhance.Persona.Overclock >= overclockCount ||
            personas.Count <= 0) return;

        const string UID = "uid://xm335fp66js3";
        OverclockEnhance overclockEnhance = GD.Load<PackedScene>(UID).Instantiate<OverclockEnhance>();
        overclockEnhance.Persona = _personaEnhance.Persona;
        overclockEnhance.Connect(OverclockEnhance.SignalName.Overclocked,
            new Callable(this, MethodName.OnOverclocked));

        _personaEnhance.AddSibling(overclockEnhance);
    }

    private bool PersonaMatch(Persona persona)
    {
        return persona.Name == _personaEnhance.Persona.Name && persona.ID != _personaEnhance.Persona.ID;
    }

    private void OnMaterialsCleared()
    {
        _personaEnhance.TweenExperience(0);
        _personaEnhance.SetExperienceText(0);
    }

    private void OnEnhancedPersona(int totalExperience)
    {
        int maxLevel = PersonaManager.Instance.GetMaxLevel(_personaEnhance.Persona);
        if (_personaEnhance.Persona.Level >= maxLevel) return;
        
        _personaEnhance.GiveExperience(totalExperience);

        EmitSignal(SignalName.PersonaUpdated);
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

        int maxLevel = PersonaManager.Instance.GetMaxLevel(_personaEnhance.Persona);
        if (_personaEnhance.Persona.Level < maxLevel) 
        {
            AddMaterialEnhance();
        }

        EmitSignal(SignalName.PersonaUpdated);
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
}
