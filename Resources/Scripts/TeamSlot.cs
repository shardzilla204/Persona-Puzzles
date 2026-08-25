using Godot;

namespace PersonaAndPuzzles;

public partial class TeamSlot : TextureRect
{
    [Export]
    private TextureRect _personaIcon;

    [Export]
    private TextureRect _skillTypeIcon;

    [Export]
    private TextureRect _emptyIcon;

    public Persona Persona;
    public int Index;

    private bool _isHovering;
    private bool _isMousePressed;

    public override void _Ready()
    {
        MouseEntered += () => _isHovering = true;
        MouseExited += () => _isHovering = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (!_isHovering || 
            @event is not InputEventMouseButton eventMouseButton ||
            !eventMouseButton.IsPressed()) return;

        // Remove persona from loadout on double click
        if (eventMouseButton.DoubleClick)
        {
            CompendiumSlot compendiumSlot = GetCompendium().FindCompendiumSlot(Persona);
            compendiumSlot.ToggleUsage(false);

            SetPersona(null);

            PersonaManager.SetLoadout(null, Index);
            return;
        }

        // Show persona stats
        if (!_isMousePressed)
        {
            _isMousePressed = true;

            LoadingBar loadingBar = PackedScenes.GetLoadingBar();
            GetTree().Root.AddChild(loadingBar);

            loadingBar.GlobalPosition = GetGlobalMousePosition() - loadingBar.Size / 2;

            loadingBar.Connect(LoadingBar.SignalName.Filled, 
                Callable.From(ShowCompendiumSlotStats));
        }
        else
        {
            _isMousePressed = false;

            PersonaAndPuzzles.Signals.EmitSignal(Signals.SignalName.MouseReleased);
        }
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        return data.As<CompendiumSlot>() is CompendiumSlot;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        CompendiumSlot newSlot = data.As<CompendiumSlot>();
        newSlot.ToggleUsage(true);

        Persona newPersona = newSlot.Persona;
        Persona oldPersona = Persona;

        SetPersona(newPersona);

        if (oldPersona == null)
        {
            PersonaManager.SetLoadout(newPersona, Index);
        }
        else
        {
            // Swap out old persona with new persona
            CompendiumSlot oldSlot = GetCompendium().FindCompendiumSlot(oldPersona);
            oldSlot.ToggleUsage(false);

            PersonaManager.SetLoadout(newPersona, oldPersona.ID);
        }
    }

    public void SetPersona(Persona persona)
    {
        Persona = persona;
        
        const string UID = "uid://bkmlw2i12i8u8";
        Texture2D circleTexture = GD.Load<Texture2D>(UID);

        _personaIcon.Texture = persona != null ? PersonaManager.GetPersonaImage(persona) : null;
        _personaIcon.Visible = persona != null;
        _skillTypeIcon.Texture = persona != null ? SkillOrbManager.GetSkillTypeTexture(persona.SkillType) : circleTexture;
        _skillTypeIcon.SelfModulate = persona != null ? Color.FromHtml("ffffff") /* White */ : Color.FromHtml("666666") /* Dark Gray */;
        _skillTypeIcon.Visible = persona != null;
        _emptyIcon.Visible = persona == null;
        SelfModulate = persona != null ? SkillOrbManager.GetSkillTypeColor(persona.SkillType) : Color.FromHtml("808080") /* Gray */;
    }

    private void ShowCompendiumSlotStats()
    {
        CompendiumStats compendiumStats = PackedScenes.GetCompendiumStats(Persona);

        PersonaTeam personaTeam = GetParent().GetOwner<PersonaTeam>();
        Compendium compendium = personaTeam.GetOwner<Compendium>();
        Node mainMenu = compendium.GetParent();
        mainMenu.AddChild(compendiumStats);
        mainMenu.MoveChild(compendiumStats, 2);

        PersonaAndPuzzles.Signals.EmitSignal(Signals.SignalName.CompendiumSlotLoaded);
    }

    private Compendium GetCompendium()
    {
        PersonaTeam personaTeam = GetParent().GetOwner<PersonaTeam>();
        return personaTeam.GetOwner<Compendium>();
    } 
}
