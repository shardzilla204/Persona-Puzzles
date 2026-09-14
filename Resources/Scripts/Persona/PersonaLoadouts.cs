using System;
using Godot;

namespace PersonaAndPuzzles;

public partial class PersonaLoadouts : Control
{
    [Signal]
    public delegate void LoadoutSelectedEventHandler(int loadoutIndex);

    [Signal]
    public delegate void LoadoutNamedChangedEventHandler(int loadoutIndex, string name);

    [Export]
    private Button _backdropButton;

    [Export]
    private SplitContainer _splitContainer;

    [Export]
    private Container _loadoutButtonsContainer;

    private int _threshold = -265;
    private int _margin = 50;

    private bool _isTweening = false;

    public Compendium Compendium;

    public override void _Ready()
    {
        _backdropButton.Pressed += QueueFree;
        _splitContainer.DragEnded += OnDragEnded;
        _splitContainer.Dragged += OnDragged;

        ClearLoadoutsContainer();
        FillLoadoutsContainer();
    }

    public void ChangeLoadout(PersonaLoadout Loadout)
    {
        Compendium compendium = FindCompendium();
        if (compendium != null)
        {
            compendium.SetSlotUsage(Loadout); // Update the visuals first
        }

        PersonaManager.Loadout.ChangeLoadout(Loadout);
        EmitSignal(SignalName.LoadoutSelected, Loadout.Index);
        QueueFree();
    }

    private Compendium FindCompendium()
    {
        Node mainMenu = GetParent();
        bool hasCompendiumNode = mainMenu.HasNode("Compendium");
        if (!hasCompendiumNode) return null;
        
        Compendium compendium = mainMenu.GetNode<Compendium>("Compendium");
        return compendium;
    }

    private void SetColor(Color color)
    {
        _isTweening = true;
        _splitContainer.Modulate = color;
    }

    private void FillLoadoutsContainer()
    {
        const string UID = "uid://qicend1g781d";
        for (int i = 0; i < PersonaManager.Loadouts.Count; i++)
        {
            PersonaLoadoutButton personaLoadoutButton = GD.Load<PackedScene>(UID).Instantiate<PersonaLoadoutButton>();
            personaLoadoutButton.SetLoadout(i);
            _loadoutButtonsContainer.AddChild(personaLoadoutButton);
        }
    }

    private void ClearLoadoutsContainer()
    {
        foreach (Node child in _loadoutButtonsContainer.GetChildren())
        {
            child.QueueFree();
        }
    }

    private void OnDragged(long offset)
    {
        Color color = Colors.White;
        Color targetModulate = offset > _threshold + _margin ? color.Darkened(0.25f) : color;
        
        // Removes tweening delay
        if (_isTweening || _splitContainer.Modulate == targetModulate) return;

        const float Duration = 0.25f;
        Tween tween = CreateTween();
        tween.TweenMethod(Callable.From<Color>(SetColor), _splitContainer.Modulate, targetModulate, Duration);
        tween.Finished += () => _isTweening = false;
    }

    private void OnDragEnded()
    {
        int offset = _splitContainer.SplitOffsets[0];
        if (offset > _threshold + _margin)
        {
            QueueFree();
        }
        else if (offset > _threshold)
        {
            _splitContainer.SetSplitOffsets([_threshold]);
        }
    }
}
