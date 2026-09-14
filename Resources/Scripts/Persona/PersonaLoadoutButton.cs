using Godot;

namespace PersonaAndPuzzles;

public partial class PersonaLoadoutButton : Button
{
    [Export]
    private Container _teamSlotContainer;

    [Export]
    private Label _nameLabel;

    [Export]
    private CustomButton _editNameButton;

    private PersonaLoadout _Loadout = new PersonaLoadout();

    public override void _Ready()
    {
        Pressed += OnPressed;

        _editNameButton.Pressed += OnEditNameButtonPressed;

        _nameLabel.Text = _Loadout.Name;

        ClearTeamSlotContainer();
        FillTeamSlotContainer();
    }

    private void OnPressed()
    {
        Modulate = Colors.White.Darkened(0.25f);

        PersonaLoadouts personaLoadouts = GetParent().GetOwner<PersonaLoadouts>();
        personaLoadouts.ChangeLoadout(_Loadout);
    }

    private void OnEditNameButtonPressed()
    {
        PersonaLoadouts personaLoadouts = GetParent().GetOwner<PersonaLoadouts>();
        Node mainMenu = personaLoadouts.GetParent();

        const string UID = "uid://dy0e01vg2lsmn";
        LoadoutNameEdit loadoutNameEdit = GD.Load<PackedScene>(UID).Instantiate<LoadoutNameEdit>();
        loadoutNameEdit.SetText(_Loadout.Name);
        loadoutNameEdit.Connect(LoadoutNameEdit.SignalName.Confirmed,
            new Callable(this, MethodName.OnLoadoutNameEditConfirmed));

        mainMenu.AddChild(loadoutNameEdit);
    }

    public void SetLoadout(int LoadoutIndex)
    {
        _Loadout = PersonaManager.Loadouts[LoadoutIndex];
    }

    private void FillTeamSlotContainer()
    {
        const string UID = "uid://bfox35ygy4i12";
        foreach (Persona persona in _Loadout.Personas)
        {
            TeamSlot teamSlot = GD.Load<PackedScene>(UID).Instantiate<TeamSlot>();
            teamSlot.SetPersona(persona);
            _teamSlotContainer.AddChild(teamSlot);
        }
    }

    private void ClearTeamSlotContainer()
    {
        foreach (Node child in _teamSlotContainer.GetChildren())
        {
            child.QueueFree();
        }
    }

    private void OnLoadoutNameEditConfirmed(string name)
    {
        if (_Loadout.Index == PersonaManager.Loadout.Index)
        {
            PersonaManager.Loadout.Name = name;
        }

        PersonaManager.Loadouts[_Loadout.Index].Name = name;

        _nameLabel.Text = name;

        PersonaLoadouts personaLoadouts = GetParent().GetOwner<PersonaLoadouts>();
        personaLoadouts.EmitSignal(PersonaLoadouts.SignalName.LoadoutNamedChanged, _Loadout.Index, name);
    }
}
