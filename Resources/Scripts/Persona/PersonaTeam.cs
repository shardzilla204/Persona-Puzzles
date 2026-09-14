using Godot;

namespace PersonaAndPuzzles;

public partial class PersonaTeam : TextureRect
{
    [Export]
    private Container _personaTeamSlotContainer;

    [Export]
    private CustomButton _dropdownButton;

    [Export]
    private Label _nameLabel;

    // When loading a loadout from a save file have the index of the loadout.
    public override void _EnterTree()
    {
        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.PersonaReleased, 
            Callable.From(Refresh));
    }

    public override void _Ready()
    {
        _dropdownButton.Pressed += OnDropdownPressed;

        _nameLabel.Text = PersonaManager.Loadout.Name;

        Refresh();
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
        for (int i = 0; i < PersonaManager.Loadout.Personas.Count; i++)
        {
            Persona persona = PersonaManager.Loadout.Personas[i];

            TeamSlot teamSlot = GD.Load<PackedScene>(UID).Instantiate<TeamSlot>();
            teamSlot.Index = i;
            teamSlot.SetPersona(persona);
            _personaTeamSlotContainer.AddChild(teamSlot);
        }
    }

    private void RemoveChildren()
    {
        foreach (Node child in _personaTeamSlotContainer.GetChildren())
        {
            child.QueueFree();
        }
    }

    protected virtual Node GetMainMenu()
    {
        return GetParent().GetParent();
    }

    private void OnDropdownPressed()
    {
        Node mainMenu = GetMainMenu();

        const string UID = "uid://brvlkxorlpgex";
        PersonaLoadouts personaLoadouts = GD.Load<PackedScene>(UID).Instantiate<PersonaLoadouts>();
        personaLoadouts.Connect(PersonaLoadouts.SignalName.LoadoutSelected,
            new Callable(this, MethodName.OnLoadoutSelected));
        personaLoadouts.Connect(PersonaLoadouts.SignalName.LoadoutNamedChanged,
            new Callable(this, MethodName.OnLoadoutNameChanged));

        mainMenu.AddChild(personaLoadouts);
    }

    private void OnLoadoutSelected(int loadoutIndex)
    {
        _nameLabel.Text = PersonaManager.Loadouts[loadoutIndex].Name;

        Refresh();
    }

    private void OnLoadoutNameChanged(int loadoutIndex, string name)
    {
        if (loadoutIndex != PersonaManager.Loadouts[loadoutIndex].Index) return;
        
        _nameLabel.Text = name;
    }

    private bool HasPersona(string id)
    {
        TeamSlot teamSlot = null;
        foreach (TeamSlot slot in _personaTeamSlotContainer.GetChildren())
        {
            if (slot.Persona == null || 
                slot.Persona.ID != id) continue;

            teamSlot = slot;
            break;
        }

        return teamSlot != null;
    }

    private void Refresh()
    {
        RemoveChildren();
        AddTeamSlots();
    }
}
