using System.Collections;
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

public partial class PersonaStats : TextureRect
{
    [Signal]
    public delegate void FavoriteButtonToggledEventHandler(bool isToggled);

    [Export]
    private Label _personaNameLabel;

    [Export]
    private Label _personaLevelLabel;

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

    [Export]
    private CustomButton _enhanceButton;

    [Export]
    private CustomButton _releaseButton;

    [Export]
    private Container _overclockContainer;

    public Persona Persona;

    public override void _Ready()
    {
        _favoriteButton.Toggled += OnFavoriteButtonToggled;
        _exitButton.Pressed += OnExitButtonPressed;
        _enhanceButton.Pressed += OnEnhanceButtonPressed;
        _releaseButton.Pressed += OnReleaseButtonPressed;

        SetFavoriteButtonModulate(Persona.IsFavorite);
        SetOverclock();
    }

    public void Set(Persona persona)
    {
        SetPersona(persona);
        FillSkillResistances();
        SetStats();
    }

    private void OnExitButtonPressed()
    {
        MainMenu mainMenu = GetParent<MainMenu>();
        mainMenu.ShowPreviousCanvas();

        QueueFree();
    }

    private void SetPersona(Persona persona)
    {
        Persona = persona;

        _personaNameLabel.Text = persona.Name;
        _personaLevelLabel.Text = $"Lvl. {persona.Level}";
        _personaPortrait.Texture = PersonaManager.Instance.GetPersonaImage(persona);
    }

    private void FillSkillResistances()
    { string UID = "uid://ep7cjwts3fmn";

        foreach (SkillType skillType in Persona.Resistances.Keys)
        {
            if (skillType == SkillType.Almighty) continue;
            Resistance resistance = Persona.Resistances[skillType];

            CompendiumSkillResistance compendiumSkillResistance = GD.Load<PackedScene>(UID).Instantiate<CompendiumSkillResistance>();
            compendiumSkillResistance.Set(skillType, resistance);

            _skillResistancesContainer.AddChild(compendiumSkillResistance);
        }
    }

    private void SetStats()
    {
        foreach (Node child in _statsContainer.GetChildren())
        {
            child.QueueFree();
        }

        const string UID = "uid://8kldhwqxuphj";

        string[] statAbbreviations = [ "St", "Ma", "En", "Ag", "Lu" ];
        for (int i = 0; i < statAbbreviations.Length; i++)
        {
            string statAbbreviation = statAbbreviations[i];
            int statValue = GetStatValue(Persona, statAbbreviation);
            PersonaStat personaStat = GD.Load<PackedScene>(UID).Instantiate<PersonaStat>();
            personaStat.Set(statAbbreviation, statValue); 

            _statsContainer.AddChild(personaStat);
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
        _favoriteButton.Modulate = isToggled ? new Color("#ffe146") : new Color("#2c2c2c");
        
        _favoriteButton.ButtonPressed = isToggled;
    }

    private void OnFavoriteButtonToggled(bool isToggled)
    {
        Persona.IsFavorite = isToggled;
        SetFavoriteButtonModulate(isToggled);
        EmitSignal(SignalName.FavoriteButtonToggled, isToggled);
    }

    private void OnEnhanceButtonPressed()
    {
        Signals.Instance.EmitSignal(Signals.SignalName.ChangedCanvas);

        const string UID = "uid://b7m6oh8k6x5w8";
        CompendiumEnhance compendiumEnhance = GD.Load<PackedScene>(UID).Instantiate<CompendiumEnhance>();
        compendiumEnhance.SetPersona(Persona);
        compendiumEnhance.Connect(CompendiumEnhance.SignalName.PersonaUpdated, 
            Callable.From(OnPersonaUpdated));
        
        AddSibling(compendiumEnhance);
    }

    private void OnPersonaUpdated()
    {
        SetPersona(Persona);
        SetStats();
        SetOverclock();
    }

    private void OnReleaseButtonPressed()
    {
        const string Warning = "This action cannot be undone";
        const string UID = "uid://cn6k7obbk8cx4";
        ConfirmationInterface confirmationInterface = GD.Load<PackedScene>(UID).Instantiate<ConfirmationInterface>();
        confirmationInterface.Accepted += ReleasePersona;
        confirmationInterface.SetWarningText(Warning);
        AddChild(confirmationInterface);
    }

    private void ReleasePersona()
    {
        MainMenu mainMenu = GetParent<MainMenu>();
        mainMenu.ShowPreviousCanvas();

        string text = $"{Persona.Name} has been released";
        PrintRich.PrintLine(text, TextColor.Yellow);

        PersonaManager.Instance.RemovePersona(Persona);
        QueueFree();
    }

    public void SetOverclock()
    {
        foreach (Node child in _overclockContainer.GetChildren())
        {
            child.QueueFree();
        }
        
        int overclockCount = PersonaManager.Instance.GetOverclockCount(Persona.Rank);
        int personaOverclock = Persona.Overclock;
        const string UID = "uid://dk2wpe0jpwrhd";
        for (int i = 0; i < overclockCount; i++)
        {
            personaOverclock--;
            Color color = personaOverclock >= 0 ? Colors.White : new Color("2c2c2c") /* Dark Gray */;
            TextureRect overclockTextureRect = new TextureRect()
            {
                Texture = GD.Load<Texture2D>(UID),
                ExpandMode = ExpandModeEnum.IgnoreSize,
                StretchMode = StretchModeEnum.KeepAspectCentered,
                CustomMinimumSize = new Vector2(20, 20),
                Modulate = color
            };
            _overclockContainer.AddChild(overclockTextureRect);
        }
    }
}
