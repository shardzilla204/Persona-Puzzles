using Godot;
using Godot.Collections;

namespace PersonaAndPuzzles;

public partial class PersonaEnhance : Control
{
    [Signal]
    public delegate void PersonaLevelMaxedEventHandler();

    [Signal]
    public delegate void PersonaLevelPreviewMaxedEventHandler(bool isMaxed);

    [Signal]
    public delegate void ConvertedMaterialsEventHandler(Dictionary<EnhancementMaterial, int> materials);

    [Export]
    private PersonaSlot _personaSlot;

    [Export]
    private Label _personaNameLabel;

    [Export]
    private RichTextLabel _personaLevelLabel;

    [Export]
    private RichTextLabel _experienceLabel;

    [Export]
    private ProgressBar _initialProgressBar;

    [Export]
    private ProgressBar _finalProgressBar;

    [Export]
    private Container _overclockContainer;

    public Persona Persona;

    private string _skillTypeFilter;
    private string _favoriteFilter;
    private string _statTypeFilter;

    public override void _Ready()
    {
        _personaSlot.SetPersona(Persona);
        _personaNameLabel.Text = $"{Persona.Name}";

        Refresh();
        RefreshOverclock();
    }

    public void RefreshOverclock()
    {
        foreach (Node child in _overclockContainer.GetChildren())
        {
            child.QueueFree();
        }
        
        int overclockCount = PersonaManager.GetOverclockCount(Persona.Rank);
        int personaOverclock = Persona.Overclock;
        const string UID = "uid://dk2wpe0jpwrhd";
        for (int i = 0; i < overclockCount; i++)
        {
            personaOverclock--;
            Color color = personaOverclock >= 0 ? Colors.White : new Color("4f4b47") /* Grey */;
            TextureRect overclockTextureRect = new TextureRect()
            {
                Texture = GD.Load<Texture2D>(UID),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                CustomMinimumSize = new Vector2(10, 10),
                Modulate = color
            };
            _overclockContainer.AddChild(overclockTextureRect);
        }
    }

    public void GiveExperience(int experience)
    {
        int totalExperience = Persona.Experience + experience;

        int levels = GetLevels(totalExperience);
        int experienceRemainder = GetExperienceRemainder(totalExperience, levels, Persona.Level);

        int maxLevel = PersonaManager.GetMaxLevel(Persona);
        Persona.Level = Mathf.Min(Persona.Level + levels, maxLevel);

        if (Persona.Level >= maxLevel)
        {
            Persona.Experience = 0;

            ConvertMaterials(experienceRemainder);

            SetLevelText(-1);
            SetExperienceText(-1);
            
            EmitSignal(SignalName.PersonaLevelMaxed);
            return;
        }

        _initialProgressBar.MaxValue = GetMaxExperience(Persona.Level);
        _finalProgressBar.MaxValue = GetMaxExperience(Persona.Level);

        Persona.Experience = experienceRemainder;
        TweenExperience(experienceRemainder);

        SetLevelText();
        SetExperienceText();
    }

    private void ConvertMaterials(int experience)
    {
        Dictionary<EnhancementMaterial, int> _materials = new Dictionary<EnhancementMaterial, int>()
        {
            { EnhancementMaterial.Legend, 0 },
            { EnhancementMaterial.Anecdote, 0 },
            { EnhancementMaterial.Memory, 0 }
        };

        foreach (EnhancementMaterial material in _materials.Keys)
        {
            int materialCount = 0;
            int materialExperience = PersonaManager.GetMaterialExperience(material);
            while (experience >= materialExperience)
            {
                materialCount++;
                experience -= materialExperience;
            }
            _materials[material] = materialCount;
        }

        EmitSignal(SignalName.ConvertedMaterials, _materials);
    }

    private int GetLevels(int experience)
    {
        int levels = 0;
        int maxExperience = GetMaxExperience(Persona.Level);
        while (experience >= maxExperience)
        {
            levels++;
            experience -= maxExperience;
            maxExperience = GetMaxExperience(Persona.Level + levels);
        }
        return levels;
    }

    private int GetExperienceRemainder(int experience, int levels, int startingLevel)
    {
        int maxExperience = GetMaxExperience(levels);
        for (int i = 0; i <= levels; i++)
        {
            experience -= maxExperience;
            maxExperience = GetMaxExperience(startingLevel + i);
        }
        return experience;
    }

    public int GetMaxExperience(int level)
    {
        int maxExperience = 0;
        const int LinearGrowth = 100;
        const float ExponentialGrowth = 0.05f;
        for (int i = 0; i < level; i++)
        {
            maxExperience += LinearGrowth;
            maxExperience += Mathf.RoundToInt(maxExperience * ExponentialGrowth);
        }
        return maxExperience;
    }

    private int GetExperienceDifference()
    {
        int maxExperience = GetMaxExperience(Persona.Level);
        return maxExperience - Persona.Experience;
    }

    private void OnCompendiumFilterConfirmed(string targetSkillType, string targetFavorite, string targetStatType)
    {
        _skillTypeFilter = targetSkillType;
        _favoriteFilter = targetFavorite;
        _statTypeFilter = targetStatType;
    }

    public void SetLevelText(int levels = 0)
    {
        string levelsText = levels <= 0 ? "" : $"[color='ffe146'] (+{levels})[/color]";

        int maxLevel = PersonaManager.GetMaxLevel(Persona);
        _personaLevelLabel.Text = $"Lvl. {Persona.Level} / {maxLevel}{levelsText}";
    }
    
    public void SetExperienceText(int totalExperience = 0)
    {
        if (totalExperience == -1)
        {
            _experienceLabel.Text = $"Next Lvl. In: Is Maxed";
            return;
        }

        string totalExperienceText = totalExperience == 0 ? "" : $"[color='ffe146'](+{totalExperience:N0})[/color]";
        int levels = GetLevels(totalExperience);
        int maxLevels = PersonaManager.GetMaxLevel(Persona);

        bool isPersonaLevelMaxed = Persona.Level + levels >= maxLevels;
        EmitSignal(SignalName.PersonaLevelPreviewMaxed, isPersonaLevelMaxed);

        int experienceDifference = GetExperienceDifference();
        _experienceLabel.Text = $"Next Lvl. In: {experienceDifference:N0} {totalExperienceText}";

        if (totalExperience == 0) return;
        
        SetLevelText(levels);
    }

    public void TweenExperience(int experience)
    {
        const float Duration = 0.25f;
        Tween tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(_initialProgressBar, "value", experience, Duration);
        tween.TweenProperty(_finalProgressBar, "value", experience, Duration);
    }

    public void TweenTargetExperience(int experience)
    {
        const float Duration = 0.25f;
        Tween tween = CreateTween().SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(_initialProgressBar, "value", experience, Duration);
    }

    public void Refresh()
    {
        _initialProgressBar.Value = Persona.Experience;
        _initialProgressBar.MaxValue = GetMaxExperience(Persona.Level);
        
        _finalProgressBar.Value = Persona.Experience;
        _finalProgressBar.MaxValue = GetMaxExperience(Persona.Level);

        SetLevelText();
        SetExperienceText();
    }
}
