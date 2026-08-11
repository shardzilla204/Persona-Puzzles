using System.Collections.Generic;
using Godot;

namespace PersonaAndPuzzles;

public partial class PersonaTeam : TextureRect
{
    [Export]
    private Container _personaTeamSlotContainer;

    [Export]
    private CustomButton _filterButton;

    [Export]
    private CustomButton _dropdownButton;

    [Export]
    private Label _nameLabel;

    public List<TeamSlot> TeamSlots = new List<TeamSlot>();
    public PersonaRoster Loadout;

    private string _skillTypeFilter;
    private string _favoriteFilter;
    private string _statTypeFilter;

    // When loading a loadout from a save file have the index of the loadout.
    public override void _Ready()
    {
        _filterButton.Pressed += OnFilterButtonPressed;
        _dropdownButton.Pressed += OnDropdownPressed;

        _nameLabel.Text = PersonaManager.Roster.Name;
        Loadout = PersonaManager.Loadouts[0];

        RemoveChildren();
        AddTeamSlots();
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        if (data.As<CompendiumSlot>() is CompendiumSlot slot)
        {
            return !HasPersona(slot.Persona.ID);
        }
        return false;
    }

    private void AddTeamSlots()
    {
        const string UID = "uid://bfox35ygy4i12";
        for (int i = 0; i < PersonaManager.Roster.Personas.Count; i++)
        {
            Persona persona = PersonaManager.Roster.Personas[i];

            TeamSlot teamSlot = GD.Load<PackedScene>(UID).Instantiate<TeamSlot>();
            teamSlot.Index = i;
            teamSlot.SetPersona(persona);
            _personaTeamSlotContainer.AddChild(teamSlot);
            TeamSlots.Add(teamSlot);
        }
    }

    private void RemoveChildren()
    {
        foreach (Node child in _personaTeamSlotContainer.GetChildren())
        {
            child.QueueFree();
        }
    }

    private void OnFilterButtonPressed()
    {
        Compendium compendium = GetParent().GetOwner<Compendium>();
        Control mainMenu = compendium.GetParent<Control>();

        const string UID = "uid://c7qklaahiqwoy";
        CompendiumFilter compendiumFilter = GD.Load<PackedScene>(UID).Instantiate<CompendiumFilter>();
        compendiumFilter.SkillType = _skillTypeFilter;
        compendiumFilter.Favorite = _favoriteFilter;
        compendiumFilter.StatType = _statTypeFilter;
        compendiumFilter.Connect(CompendiumFilter.SignalName.Confirmed, 
            new Callable(compendium, Compendium.MethodName.OnCompendiumFilterConfirmed));
        compendiumFilter.Connect(CompendiumFilter.SignalName.Confirmed, 
            new Callable(this, MethodName.OnCompendiumFilterConfirmed));
        
        mainMenu.AddChild(compendiumFilter);
    }

    private void OnCompendiumFilterConfirmed(string targetSkillType, string targetFavorite, string targetStatType)
    {
        _skillTypeFilter = targetSkillType;
        _favoriteFilter = targetFavorite;
        _statTypeFilter = targetStatType;
    }

    private void OnDropdownPressed()
    {
        Compendium compendium = GetParent().GetOwner<Compendium>();
        Control mainMenu = compendium.GetParent<Control>();

        const string UID = "uid://brvlkxorlpgex";
        PersonaLoadouts personaLoadouts = GD.Load<PackedScene>(UID).Instantiate<PersonaLoadouts>();
        personaLoadouts.Compendium = compendium;
        personaLoadouts.Connect(PersonaLoadouts.SignalName.LoadoutSelected,
            new Callable(this, MethodName.OnLoadoutSelected));
        personaLoadouts.Connect(PersonaLoadouts.SignalName.LoadoutNamedChanged,
            new Callable(this, MethodName.OnLoadoutNameChanged));

        mainMenu.AddChild(personaLoadouts);
    }

    private void OnLoadoutSelected(int loadoutIndex)
    {
        Loadout = PersonaManager.Loadouts[loadoutIndex];

        _nameLabel.Text = Loadout.Name;

        RemoveChildren();
        AddTeamSlots();
    }

    private void OnLoadoutNameChanged()
    {
        _nameLabel.Text = Loadout.Name;
    }

    private bool HasPersona(string id)
    {
        TeamSlot teamSlot = null;
        foreach (TeamSlot slot in TeamSlots)
        {
            if (slot.Persona == null || 
                slot.Persona.ID != id) continue;

            teamSlot = slot;
            break;
        }

        return teamSlot != null;
    }
}
