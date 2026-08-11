using Godot;

namespace PersonaAndPuzzles;

public enum StatType
{
    Strength,
    Magic,
    Endurance,
    Agility,
    Luck
}

public partial class CompendiumStats : TextureRect
{
    [Signal]
    public delegate void FavoriteButtonToggledEventHandler(bool isToggled);

    [Export]
    private Label _personaNameLabel;

    [Export]
    private TextureRect _personaPortrait;

    [Export]
    private Container _skillResistancesContainer;

    [Export]
    private Container _statsContainer;

    [Export]
    private CustomButton _favoriteButton;

    [Export]
    private CustomButton _exitButton;

    public Persona Persona;

    public override void _Ready()
    {
        _favoriteButton.Toggled += OnFavoriteButtonToggled;
        _exitButton.Pressed += QueueFree;

        SetFavoriteButtonModulate(Persona.IsFavorite);
    }

    public void Set(Persona persona)
    {
        Persona = persona;

        _personaNameLabel.Text = persona.Name;
        _personaPortrait.Texture = PersonaManager.GetPersonaImage(persona);

        FillSkillResistances();
        SetStats();
    }

    private void FillSkillResistances()
    {
        const string UID = "uid://ep7cjwts3fmn";

        foreach (SkillType skillType in Persona.Resistances.Keys)
        {
            GD.Print(skillType);
            Resistance resistance = Persona.Resistances[skillType];
            GD.Print(resistance);

            CompendiumSkillResistance compendiumSkillResistance = GD.Load<PackedScene>(UID).Instantiate<CompendiumSkillResistance>();
            compendiumSkillResistance.Set(skillType, resistance);

            _skillResistancesContainer.AddChild(compendiumSkillResistance);
        }
    }

    private void SetStats()
    {
        const string UID = "uid://8kldhwqxuphj";

        string[] statAbbreviations = [ "St", "Ma", "En", "Ag", "Lu" ];
        for (int i = 0; i < statAbbreviations.Length; i++)
        {
            string statAbbreviation = statAbbreviations[i];
            int statValue = GetStatValue(Persona, statAbbreviation);
            CompendiumStat compendiumStat = GD.Load<PackedScene>(UID).Instantiate<CompendiumStat>();
            compendiumStat.Set(statAbbreviation, statValue); 

            _statsContainer.AddChild(compendiumStat);
        }
    }

    private int GetStatValue(Persona persona, string statAbbreviation) => statAbbreviation switch
    {
        "St" => persona.Strength,
        "Ma" => persona.Magic,
        "En" => persona.Endurance,
        "Ag" => persona.Agility,
        "Lu" => persona.Luck,
        _ => 0  
    };

    private void SetFavoriteButtonModulate(bool isToggled)
    {
        _favoriteButton.Modulate = isToggled ? new Color("ffe146") /* Gargoyle Gas - Yellow Color */ 
            : new Color("00000080"); /* Black w/ 50% opacity */
        
        _favoriteButton.ButtonPressed = isToggled;
    }

    private void OnFavoriteButtonToggled(bool isToggled)
    {
        Persona.IsFavorite = isToggled;
        SetFavoriteButtonModulate(isToggled);
        EmitSignal(SignalName.FavoriteButtonToggled, isToggled);
    }
}
