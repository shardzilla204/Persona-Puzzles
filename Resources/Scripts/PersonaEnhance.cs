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

    [Export]
    private CustomButton _exitButton;

    public Persona Persona;

    public override void _Ready()
    {
        _personaSlot.SetPersona(Persona);
        _personaNameLabel.Text = $"{Persona.Name}";
        _exitButton.Pressed += OnExitButtonPressed;

        Refresh();
        RefreshOverclock();
    }

    private void OnExitButtonPressed()
    {
        GetParent().GetOwner().QueueFree();
    }

    public void RefreshOverclock()
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
            Color color = personaOverclock >= 0 ? Colors.White : new Color("#4f4b47");
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

        int levels = PersonaManager.Instance.GetLevels(Persona, totalExperience);
        int experienceRemainder = GetExperienceRemainder(totalExperience, levels, Persona.Level);

        int maxLevel = PersonaManager.Instance.GetMaxLevel(Persona);
        int targetLevel = Mathf.Min(Persona.Level + levels, maxLevel);
        PersonaManager.Instance.SetLevel(Persona, targetLevel);

        if (Persona.Level >= maxLevel)
        {
            Persona.Experience = 0;

            ConvertMaterials(experienceRemainder);

            _initialProgressBar.MaxValue = 0;
            _finalProgressBar.MaxValue = 0;

            SetLevelText(-1);
            SetExperienceText(-1);
            
            EmitSignal(SignalName.PersonaLevelMaxed);
            return;
        }

        _initialProgressBar.MaxValue = PersonaManager.Instance.GetMaxExperience(Persona.Level);
        _finalProgressBar.MaxValue = PersonaManager.Instance.GetMaxExperience(Persona.Level);

        Persona.Experience = experienceRemainder;
        TweenExperience(experienceRemainder);

        SetLevelText();
        SetExperienceText();
    }

    private void ConvertMaterials(int experience)
    {
        Dictionary<EnhancementMaterial, int> materials = new Dictionary<EnhancementMaterial, int>()
        {
            { EnhancementMaterial.Legend, 0 },
            { EnhancementMaterial.Anecdote, 0 },
            { EnhancementMaterial.Memory, 0 }
        };

        foreach (EnhancementMaterial material in materials.Keys)
        {
            int materialCount = 0;
            int materialExperience = PersonaManager.Instance.GetMaterialExperience(material);
            while (experience >= materialExperience)
            {
                materialCount++;
                experience -= materialExperience;
            }
            materials[material] = materialCount;
        }

        EmitSignal(SignalName.ConvertedMaterials, materials);
    }

    private int GetExperienceRemainder(int experience, int levels, int startingLevel)
    {
        int maxExperience = PersonaManager.Instance.GetMaxExperience(levels);
        for (int i = 0; i <= levels; i++)
        {
            experience -= maxExperience;
            maxExperience = PersonaManager.Instance.GetMaxExperience(startingLevel + i);
        }
        return experience;
    }

    private int GetExperienceDifference()
    {
        int maxExperience = PersonaManager.Instance.GetMaxExperience(Persona.Level);
        return maxExperience - Persona.Experience;
    }

    public void SetLevelText(int levels = 0)
    {
        string levelsText = levels <= 0 ? "" : $"[color='ffe146'] (+{levels})[/color]";

        int maxLevel = PersonaManager.Instance.GetMaxLevel(Persona);
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
        int levels = PersonaManager.Instance.GetLevels(Persona, totalExperience);
        int maxLevels = PersonaManager.Instance.GetMaxLevel(Persona);

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
        if (Persona.Level >= PersonaManager.Instance.GetMaxLevel(Persona))
        {
            _initialProgressBar.MaxValue = 0;
            _finalProgressBar.MaxValue = 0;

            SetLevelText(-1);
            SetExperienceText(-1);
            return;
        }

        _initialProgressBar.Value = Persona.Experience;
        _initialProgressBar.MaxValue = PersonaManager.Instance.GetMaxExperience(Persona.Level);
        
        _finalProgressBar.Value = Persona.Experience;
        _finalProgressBar.MaxValue = PersonaManager.Instance.GetMaxExperience(Persona.Level);

        SetLevelText();
        SetExperienceText();
    }
}
