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

    private LoadingBar _loadingBar;

    public override void _Ready()
    {
        MouseEntered += () => _isHovering = true;
        MouseExited += () => _isHovering = false;

        SetFavoriteTextureVisibility(Persona.IsFavorite);
    }

    public override void _Process(double delta)
    {
        if (!_isHovering) 
        {
            if (IsInstanceValid(_loadingBar)) _loadingBar.QueueFree();
            return;
        }

        bool isLeftMouseButtonPressed = Input.IsMouseButtonPressed(MouseButton.Left);
        if (isLeftMouseButtonPressed && !_isMousePressed)
        {
            _isMousePressed = true;

            _loadingBar = PackedScenes.GetLoadingBar();
            _loadingBar.GlobalPosition = GetGlobalMousePosition() - _loadingBar.Size / 2;
            _loadingBar.Connect(LoadingBar.SignalName.Filled, 
                Callable.From(OnLoadingBarFilled));

            GetTree().Root.AddChild(_loadingBar);
        }
        else if (!isLeftMouseButtonPressed)
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

        if (IsInstanceValid(_loadingBar)) _loadingBar.QueueFree();
        
        return this;
    }

    private void OnLoadingBarFilled()
    {
        PersonaStats personaStats = PackedScenes.GetPersonaStats(Persona);
        personaStats.Connect(PersonaStats.SignalName.FavoriteButtonToggled, 
            new Callable(this, MethodName.SetFavoriteTextureVisibility));
        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.ChangedCanvas, 
            Callable.From(personaStats.QueueFree));

        Compendium compendium = GetParent().GetOwner<Compendium>();
        Node mainMenu = compendium.GetParent();
        mainMenu.AddChild(personaStats);
        mainMenu.MoveChild(personaStats, 2);

        PersonaAndPuzzles.Signals.EmitSignal(Signals.SignalName.CompendiumSlotLoaded);
    }

    public override void SetPersona(Persona persona)
    {
        base.SetPersona(persona);

        _personaLevelLabel.Text = $"Lvl. {persona.Level}";
    }
    
    public void ToggleUsage(bool isInUse)
    {
        IsInUse = isInUse;

        float darkenedAmount = 0.4f;
        Modulate = isInUse ? Colors.White.Darkened(darkenedAmount) : Colors.White;
    }

    public void SetFavoriteTextureVisibility(bool isVisible)
    {
        _favoriteTexture.Visible = isVisible;
    }
}
