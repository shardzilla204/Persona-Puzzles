using Godot;
using System;

namespace PersonaAndPuzzles;

public partial class PersonaCompendiumTeam : PersonaTeam
{
    [Export]
    private CustomButton _filterButton;

    public override void _Ready()
    {
        base._Ready();
        _filterButton.Pressed += OnFilterButtonPressed;
    }

    protected override Node GetMainMenu()
    {
        Node compendium = GetParent().GetOwner();
        return compendium.GetParent();
    }

    private void OnFilterButtonPressed()
    {
        Compendium compendium = GetParent().GetOwner<Compendium>();
        Control mainMenu = compendium.GetParent<Control>();

        const string UID = "uid://c7qklaahiqwoy";
        CompendiumFilter compendiumFilter = GD.Load<PackedScene>(UID).Instantiate<CompendiumFilter>();
        compendiumFilter.Connect(CompendiumFilter.SignalName.Confirmed, 
            new Callable(compendium, Compendium.MethodName.FilterPersonas));
        
        mainMenu.AddChild(compendiumFilter);
    }
}
