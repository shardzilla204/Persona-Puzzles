using Godot;

namespace PersonaAndPuzzles;

public partial class PackedScenes : Node
{
    public static SkillOrb GetSkillOrb(SkillType skillType)
    {
        const string UID = "uid://q7xw4c2ix7lw";
        SkillOrb skillOrb = GD.Load<PackedScene>(UID).Instantiate<SkillOrb>();
        skillOrb.SkillType = skillType;
        
        return skillOrb;
    }

    public static ComboLabel GetComboLabel(int comboCount)
    {
        const string UID = "uid://dg41dcug83ks";
        ComboLabel comboLabel = GD.Load<PackedScene>(UID).Instantiate<ComboLabel>();
        comboLabel.Text = $"Combo {comboCount}";

        return comboLabel;
    }

    public static CompendiumSlot GetCompendiumSlot(Persona persona)
    {
        const string UID = "uid://bx43agqlgfr80";
        CompendiumSlot compendiumSlot = GD.Load<PackedScene>(UID).Instantiate<CompendiumSlot>();
        compendiumSlot.SetPersona(persona);

        return compendiumSlot;
    }

    public static TextureRect GetDragCircle()
    {
        const string UID = "uid://cmrji3nqlmrgc";
        TextureRect dragCircle = GD.Load<PackedScene>(UID).Instantiate<TextureRect>();
        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.CompendiumScrollEnded, 
            Callable.From(dragCircle.QueueFree));
        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.LoadingBarFilling, 
            Callable.From(dragCircle.QueueFree));
        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.CompendiumSlotLoaded, 
            Callable.From(dragCircle.QueueFree));

        return dragCircle;
    }

    public static LoadingBar GetLoadingBar()
    {
        const string UID = "uid://dp6kow1n3e6ns";
        LoadingBar loadingBar = GD.Load<PackedScene>(UID).Instantiate<LoadingBar>();

        return loadingBar;
    }

    public static PersonaStats GetPersonaStats(Persona persona)
    {
        const string UID = "uid://dos6wimuvhfa7";
        PersonaStats personaStats = GD.Load<PackedScene>(UID).Instantiate<PersonaStats>();
        personaStats.Set(persona);
        
        return personaStats;
    }

    public static FilterOption GetFilterOption<T>(FilterType filterType) where T : FilterOption
    {
        string UID = GetFilterOptionUID(filterType);
        return GD.Load<PackedScene>(UID).Instantiate<T>();
    }

    private static string GetFilterOptionUID(FilterType filterType) => filterType switch
    {
        FilterType.SkillType => "uid://btfc2qh4b2ttm",
        FilterType.StatType => "uid://dlvljgcqpculv",
        _ => "uid://b85g1ywxf10n"
    };
}
