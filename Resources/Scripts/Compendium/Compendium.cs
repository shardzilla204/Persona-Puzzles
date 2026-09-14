using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace PersonaAndPuzzles;

public partial class Compendium : Control
{
    [Export]
    protected Container _slotContainer;

    [Export]
    protected VScrollBar _scrollBar;

    [Export]
    protected RichTextLabel _personaCountLabel;

    private bool _isHovered;
    private bool _isMousePressed;
    private bool _isScrollBarHovered;

    private Vector2 _mouseStartPosition;

    protected List<CompendiumSlot> Slots = new List<CompendiumSlot>();

    public override void _EnterTree()
    {
        PersonaAndPuzzles.Signals.EmitSignal(Signals.SignalName.MoveOverlay);

        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.PersonaReleased, 
            Callable.From(Refresh));
    }

    public override void _Ready()
    {
        _scrollBar.ValueChanged += OnScrollBarValueChanged;
        _scrollBar.MouseEntered += () => _isScrollBarHovered = true;
        _scrollBar.MouseExited += () => _isScrollBarHovered = false;

        _slotContainer.MouseEntered += () => _isHovered = true;
        _slotContainer.MouseExited += () => _isHovered = false;
        
        SetCountLabelText();

        Fill(PersonaManager.Personas);
        CallDeferred(MethodName.SetScrollBar); // Starts when the compendium has finished loading
    }

    private void Refresh()
    {
        Clear();
        Fill(PersonaManager.Personas);
        SetCountLabelText();
    }

    private void SetCountLabelText()
    {
        string personaCountText = PersonaManager.Personas.Count > PersonaManager.MaxPersonas ? $"[color='e63948']{PersonaManager.Personas.Count}[/color]" : $"{PersonaManager.Personas.Count}";
        _personaCountLabel.Text = $"{personaCountText} / {PersonaManager.MaxPersonas}";
    }

    // When swiping down = Move container up
    // When swiping up = Move container down
    // public override void _Process(double delta)
    // {
    //     if (!_isHovered || !_isMousePressed) return;

    //     Vector2 mousePosition = GetLocalMousePosition();
    //     Vector2 dragDirection = _mouseStartPosition.DirectionTo(mousePosition);
    //     float dragDistance = _mouseStartPosition.DistanceTo(mousePosition);

    //     if (dragDirection.Y == 0) return;

    //     float dragValue = dragDistance / 50;
    //     if (dragDirection.Y > 0)
    //     {
    //         _scrollBar.Value -= dragValue;
    //     }
    //     else if (dragDirection.Y < 0)
    //     {
    //         _scrollBar.Value += dragValue;
    //     }

    //     PersonaAndPuzzles.Signals.EmitSignal(Signals.SignalName.CompendiumScrollStarted);
    // }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventMouseButton eventMouseButton) return;

        // InputDrag(eventMouseButton);

        if (!_isHovered) return;

        InputScroll(eventMouseButton);
    }
    
    protected virtual void Fill(List<Persona> personas)
    {
        foreach (Persona persona in personas)
        {
            CompendiumSlot compendiumSlot = PackedScenes.GetCompendiumSlot(persona);
            if (PersonaManager.Loadout.Personas.Contains(persona))
            {
                compendiumSlot.ToggleUsage(true);
            }

            _slotContainer.AddChild(compendiumSlot);
            Slots.Add(compendiumSlot);

            PrintRich.PrintPersona(persona);
        }
    }

    private void Clear()
    {
        Slots.Clear();
        foreach (Node child in _slotContainer.GetChildren())
        {
            child.QueueFree();
        }
    }

    private void InputScroll(InputEventMouseButton eventMouseButton)
    {
        const int ScrollStep = 15;
        if (eventMouseButton.ButtonIndex == MouseButton.WheelUp)
        {
            _scrollBar.Value -= ScrollStep;
        }
        else if (eventMouseButton.ButtonIndex == MouseButton.WheelDown)
        {
            _scrollBar.Value += ScrollStep;
        }
    }

    // private void InputDrag(InputEventMouseButton eventMouseButton)
    // {
    //     if (eventMouseButton.IsPressed() && 
    //         eventMouseButton.ButtonIndex == MouseButton.Left && 
    //         !_isMousePressed && 
    //         !_isScrollBarHovered)
    //     {
    //         _isMousePressed = true;
    //         _mouseStartPosition = GetLocalMousePosition();

    //         TextureRect dragCircle = PackedScenes.GetDragCircle();
    //         GetTree().Root.AddChild(dragCircle);

    //         dragCircle.Position = GetGlobalMousePosition() - dragCircle.Size / 2;
    //     }
    //     else
    //     {
    //         _isMousePressed = false;

    //         PersonaAndPuzzles.Signals.EmitSignal(Signals.SignalName.CompendiumScrollEnded);
    //     }
    // }

    private void SetScrollBar()
    {
        _scrollBar.MaxValue = _slotContainer.Size.Y - _scrollBar.Size.Y;
        _slotContainer.Position = new Vector2(_slotContainer.Position.X, _scrollBar.Position.Y);
    }

    public void SetSlotUsage(PersonaLoadout Loadout)
    {
        List<CompendiumSlot> slotsInUse = Slots.FindAll(slot => slot.IsInUse);
        foreach (CompendiumSlot slot in slotsInUse)
        {
            slot.ToggleUsage(false);
        }

        foreach (Persona persona in Loadout.Personas)
        {
            if (persona == null) continue;
            PrintRich.PrintPersona(persona);

            CompendiumSlot slot = FindCompendiumSlot(persona);
            if (slot == null) continue;
            slot.ToggleUsage(true);
        }
    }

    private void OnScrollBarValueChanged(double value)
    {
        _slotContainer.Position = new Vector2(_slotContainer.Position.X, (float) -value + _scrollBar.Position.Y);
    }

    public CompendiumSlot FindCompendiumSlot(Persona persona)
    {
        return Slots.Find(slot => slot.Persona.ID == persona.ID);
    }

    public void OnCompendiumFilterConfirmed(string targetSkillType, string targetFavorite, string targetStatType)
    {
        Clear();
        
        List<Persona> personas = [..PersonaManager.Personas];
        if (targetSkillType != "All")
        {
            SkillType skillType = Enum.Parse<SkillType>(targetSkillType);
            List<Persona> skillTypePersonas = personas.FindAll(persona => persona.SkillType == skillType);
            personas = [..skillTypePersonas];
        }
        
        if (targetFavorite == "Favorite")
        {
            List<Persona> favorite = personas.FindAll(persona => persona.IsFavorite == true);
            personas = [..favorite];
        }
        else if (targetFavorite == "Unfavorite")
        {
            List<Persona> unfavorite = personas.FindAll(persona => persona.IsFavorite == false);
            personas = [..unfavorite];
        }
        
        if (targetStatType != "All")
        {
            // 0 = Stat Type
            // 1 = State
            string[] statTypeStrings = targetStatType.Split("|");
            StatType statType = Enum.Parse<StatType>(statTypeStrings[0]);
            string state = statTypeStrings[1];
            bool isAscending = state == StatTypeFilterOption.AscendingState;
            List<Persona> statTypePersonas = GetPersonasByStatType(statType, personas, isAscending);
            personas = [..statTypePersonas];
        }

        Fill(personas);
    }

    private List<Persona> GetPersonasByStatType(StatType statType, List<Persona> personas, bool isAscending) => (statType, isAscending) switch
    {
        (StatType.Strength, true) => personas.OrderBy(persona => persona.Strength).ToList(),
        (StatType.Magic, true) => personas.OrderBy(persona => persona.Magic).ToList(),
        (StatType.Endurance, true) => personas.OrderBy(persona => persona.Endurance).ToList(),
        (StatType.Agility, true) => personas.OrderBy(persona => persona.Agility).ToList(),
        (StatType.Luck, true) => personas.OrderBy(persona => persona.Luck).ToList(),
        (StatType.Strength, false) => personas.OrderByDescending(persona => persona.Strength).ToList(),
        (StatType.Magic, false) => personas.OrderByDescending(persona => persona.Magic).ToList(),
        (StatType.Endurance, false) => personas.OrderByDescending(persona => persona.Endurance).ToList(),
        (StatType.Agility, false) => personas.OrderByDescending(persona => persona.Agility).ToList(),
        (StatType.Luck, false) => personas.OrderByDescending(persona => persona.Luck).ToList(),
        _ => personas
    };
}
 