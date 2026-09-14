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

    private LoadingBar _loadingBar;

    public override void _Ready()
    {
        MouseEntered += () => _isHovering = true;
        MouseExited += () => _isHovering = false;

        MouseDefaultCursorShape = Persona == null ? CursorShape.Arrow : CursorShape.PointingHand;
    }

    public override void _Process(double delta)
    {
        if (!_isHovering)
        {
            if (Persona == null) return;
            if (IsInstanceValid(_loadingBar)) _loadingBar.QueueFree();
            return;
        }

        if (Input.IsMouseButtonPressed(MouseButton.Left) && !_isMousePressed)
        {
            _isMousePressed = true;

            _loadingBar = PackedScenes.GetLoadingBar();
            _loadingBar.GlobalPosition = GetGlobalMousePosition() - _loadingBar.Size / 2;
            _loadingBar.Connect(LoadingBar.SignalName.Filled, 
                Callable.From(ShowCompendiumSlotStats));

            GetTree().Root.AddChild(_loadingBar);
        }
        else if (!Input.IsMouseButtonPressed(MouseButton.Left))
        {
            _isMousePressed = false;
            PersonaAndPuzzles.Signals.EmitSignal(Signals.SignalName.MouseReleased);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (!_isHovering || 
            @event is not InputEventMouseButton eventMouseButton ||
            !eventMouseButton.IsPressed() || 
            Persona == null) return;

        // Remove persona from loadout on double click
        if (eventMouseButton.DoubleClick)
        {
            CompendiumSlot compendiumSlot = GetCompendium().FindCompendiumSlot(Persona);
            compendiumSlot.ToggleUsage(false);

            SetPersona(null);

            PersonaManager.SetLoadout(null, Index);
            return;
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
        PersonaStats personaStats = PackedScenes.GetPersonaStats(Persona);

        PersonaTeam personaTeam = GetParent().GetOwner<PersonaTeam>();
        Node owner = personaTeam.GetOwner();
        Node mainMenu = owner.GetParent();
        mainMenu.AddChild(personaStats);
        mainMenu.MoveChild(personaStats, 2);

        PersonaAndPuzzles.Signals.EmitSignal(Signals.SignalName.CompendiumSlotLoaded);
    }

    private Compendium GetCompendium()
    {
        PersonaTeam personaTeam = GetParent().GetOwner<PersonaTeam>();
        return personaTeam.GetOwner<Compendium>();
    } 
}
