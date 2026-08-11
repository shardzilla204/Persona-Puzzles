using System;
using Godot;

namespace PersonaAndPuzzles;

public partial class CompendiumFilter : Control
{
    [Signal]
    public delegate void ConfirmedEventHandler(string skillType, string favorite, string statType);

    [Signal]
    public delegate void UnfilteredEventHandler();

    [Export]
    private SkillTypeFilterContainer _skillTypeFilters;

    [Export]
    private FavoriteFilterContainer _favoriteFilters;

    [Export]
    private StatTypeFilterContainer _statTypeFilters;

    [Export]
    private CustomButton _cancelButton;

    [Export]
    private CustomButton _unfilterButton;

    [Export]
    private CustomButton _confirmButton;

    public string SkillType;
    public string Favorite;
    public string StatType;

    public override void _Ready()
    {
        _cancelButton.Pressed += QueueFree;
        _unfilterButton.Pressed += () => EmitSignal(SignalName.Unfiltered);
        _confirmButton.Pressed += OnConfirmButtonPressed;

        _skillTypeFilters.FilterSelected += (filterName) => SkillType = filterName;
        _favoriteFilters.FilterSelected += (filterName) => Favorite = filterName;
        _statTypeFilters.FilterSelected += (filterName) => StatType = filterName;

        if (!string.IsNullOrEmpty(SkillType))
        {
            _skillTypeFilters.SetFilter(SkillType);
        }
        else
        {
            SkillType = _skillTypeFilters.SelectedFilterName;
        }

        if (!string.IsNullOrEmpty(Favorite))
        {
            _favoriteFilters.SetFilter(Favorite);
        }
        else
        {
            Favorite = _favoriteFilters.SelectedFilterName;
        }

        if (!string.IsNullOrEmpty(StatType))
        {
            _statTypeFilters.SetFilter(StatType);
        }
        else
        {
            StatType = _statTypeFilters.SelectedFilterName;
        }
    }

    private void OnConfirmButtonPressed()
    {
        EmitSignal(SignalName.Confirmed, SkillType, Favorite, StatType);
        QueueFree();
    }
}
