using Godot;

namespace PersonaAndPuzzles;

public partial class CompendiumSlot : PersonaSlot
{
    [Export]
    private Label _personaLevelLabel;

    [Export]
    private TextureRect _favoriteTexture;

    private bool _isHovering;
    private bool _isMousePressed;

    public bool IsInUse;

    public override void _Ready()
    {
        MouseEntered += () => _isHovering = true;
        MouseExited += () => _isHovering = false;

        SetPersona(Persona);
    }

    public override void _Input(InputEvent @event)
    {
        if (!_isHovering || 
            @event is not InputEventMouseButton eventMouseButton) return;

        if (eventMouseButton.IsPressed() && !_isMousePressed)
        {
            _isMousePressed = true;

            LoadingBar loadingBar = PackedScenes.GetLoadingBar();
            GetTree().Root.AddChild(loadingBar);

            loadingBar.GlobalPosition = GetGlobalMousePosition() - loadingBar.Size / 2;

            loadingBar.Connect(LoadingBar.SignalName.Filled, 
                Callable.From(OnLoadingBarFilled));
        }
        else
        {
            _isMousePressed = false;

            PersonaAndPuzzles.Signals.EmitSignal(Signals.SignalName.MouseReleased);
        }
    }

    public override Variant _GetDragData(Vector2 atPosition)
    {
        if (IsInUse) return default;

        Control control = new Control();
        CompendiumSlot compendiumSlot = PackedScenes.GetCompendiumSlot(Persona);
        control.AddChild(compendiumSlot);
        SetDragPreview(control);
        compendiumSlot.Position = -compendiumSlot.Size / 2;
        
        return this;
    }

    private void OnLoadingBarFilled()
    {
        CompendiumStats compendiumStats = PackedScenes.GetCompendiumStats(Persona);
        compendiumStats.Connect(CompendiumStats.SignalName.FavoriteButtonToggled, 
            new Callable(this, MethodName.SetFavoriteTextureVisibility));

        Compendium compendium = GetParent().GetOwner<Compendium>();
        Node mainMenu = compendium.GetParent();
        mainMenu.AddChild(compendiumStats);
        mainMenu.MoveChild(compendiumStats, 2);

        PersonaAndPuzzles.Signals.EmitSignal(Signals.SignalName.CompendiumSlotLoaded);
    }

    public override void SetPersona(Persona persona)
    {
        base.SetPersona(persona);

        _personaLevelLabel.Text = $"Lvl. {persona.Level}";
    }

    private void DisplayMenu()
    {
        
    }

    public void ToggleUsage(bool isInUse)
    {
        IsInUse = isInUse;

        float darkenedAmount = 0.4f;
        Modulate = isInUse ? Colors.White.Darkened(darkenedAmount) : Colors.White;
        MouseDefaultCursorShape = isInUse ? CursorShape.Arrow : CursorShape.PointingHand;
    }

    public void SetFavoriteTextureVisibility(bool isVisible)
    {
        _favoriteTexture.Visible = isVisible;
    }
}
