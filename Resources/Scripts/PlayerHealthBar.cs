using Godot;

namespace PersonaAndPuzzles;

public partial class PlayerHealthBar : HealthBar
{
    public override void _Ready()
    {
        SetMaxHealth();
    }

    private void SetMaxHealth()
    {
        const int Multiplier = 1;
        
        int endurance = 0;
        foreach (Persona persona in PersonaManager.Instance.Loadout.Personas)
        {
            if (persona == null) continue;
            
            endurance += persona.Endurance;
        }
        MaxValue = endurance * Multiplier;
        Value = MaxValue;

        _healthLabel.Text = $"{Value} / {MaxValue}";
    }
}
