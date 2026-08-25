using Godot;

namespace PersonaAndPuzzles;

public partial class PersonaLoadout : Button
{
    [Export]
    private Container _teamSlotContainer;

    [Export]
    private Label _nameLabel;

    [Export]
    private CustomButton _editNameButton;

    public PersonaRoster Roster = new PersonaRoster();

    public override void _Ready()
    {
        Pressed += OnPressed;

        _editNameButton.Pressed += OnEditNameButtonPressed;

        _nameLabel.Text = Roster.Name;

        ClearTeamSlotContainer();
        FillTeamSlotContainer();
    }

    private void OnPressed()
    {
        Modulate = Colors.White.Darkened(0.25f);

        PersonaLoadouts personaLoadouts = GetParent().GetOwner<PersonaLoadouts>();
        GD.Print(personaLoadouts.Name);
        personaLoadouts.ChangeLoadout(Roster);
    }

    private void OnEditNameButtonPressed()
    {
        PersonaLoadouts personaLoadouts = GetParent().GetOwner<PersonaLoadouts>();
        Node mainMenu = personaLoadouts.GetParent();

        const string UID = "uid://dy0e01vg2lsmn";
        LoadoutNameEdit loadoutNameEdit = GD.Load<PackedScene>(UID).Instantiate<LoadoutNameEdit>();
        loadoutNameEdit.SetText(Roster.Name);
        loadoutNameEdit.Connect(LoadoutNameEdit.SignalName.Confirmed,
            new Callable(this, MethodName.OnLoadoutNameEditConfirmed));

        mainMenu.AddChild(loadoutNameEdit);
    }

    private void FillTeamSlotContainer()
    {
        const string UID = "uid://bfox35ygy4i12";
        foreach (Persona persona in Roster.Personas)
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
        Roster.Name = name;
        _nameLabel.Text = name;

        PersonaLoadouts personaLoadouts = GetParent().GetOwner<PersonaLoadouts>();
        personaLoadouts.EmitSignal(PersonaLoadouts.SignalName.LoadoutNamedChanged);
    }
}
