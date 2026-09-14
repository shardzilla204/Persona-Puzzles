using Godot;
using System;

namespace PersonaAndPuzzles;

public partial class PersonaCompendiumTeam : PersonaTeam
{
    [Export]
    private CustomButton _filterButton;

    private string _skillTypeFilter;
    private string _favoriteFilter;
    private string _statTypeFilter;

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
}
