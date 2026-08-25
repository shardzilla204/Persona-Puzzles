namespace PersonaAndPuzzles;

public partial class EnemyHealthBar : HealthBar
{
    public void SetMaxHealth(Persona persona)
    {
        const int Multiplier = 40;
        MaxValue = persona.Endurance * Multiplier;
        Value = MaxValue;

        _healthLabel.Text = $"{Value} / {MaxValue}";
    }
}
