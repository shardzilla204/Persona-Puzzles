using Godot;
using System;
using System.Collections.Generic;

namespace PersonaAndPuzzles;

public enum FilterType
{
    Default,
    SkillType,
    StatType
}

public partial class FilterContainer : VBoxContainer
{
    [Signal]
    public delegate void FilterSelectedEventHandler(string filterName);

    [Export]
    protected Container _container;

    public string SelectedFilterName;

    protected List<FilterOption> FilterOptions = new List<FilterOption>();

    public override void _Ready()
    {
        // Have the method be called after the signal that's connected with FilterOption.cs
        CallDeferred(MethodName.ConnectUnfilteredSignal);
    }

    protected void AddFilter(FilterType filterType, string name)
    {
        FilterOption filterOption = GetFilterOption(filterType);
        filterOption.Connect(FilterOption.SignalName.Selected, 
            new Callable(this, MethodName.OnFilterOptionSelected));
        filterOption.Name = name;

        // Converts the name into either SkillType enum or StatType enum. Depending on the filter option type.
        if (filterOption is SkillTypeFilterOption skillTypeFilterOption)
        {
            SkillType statType = Enum.Parse<SkillType>(name);
            skillTypeFilterOption.SkillType = statType;
            _container.AddChild(skillTypeFilterOption);
            FilterOptions.Add(skillTypeFilterOption);

            return;
        }
        else if (filterOption is StatTypeFilterOption statTypeFilterOption)
        {
            StatType statType = Enum.Parse<StatType>(name);
            statTypeFilterOption.StatType = statType;
            _container.AddChild(statTypeFilterOption);
            FilterOptions.Add(statTypeFilterOption);

            return;
        }

        _container.AddChild(filterOption);
        FilterOptions.Add(filterOption);
    }

    protected void OnFilterOptionSelected(FilterOption targetOption)
    {
        List<FilterOption> filterOptions = FilterOptions.FindAll(option => option != targetOption);
        foreach (FilterOption filterOption in filterOptions)
        {
            filterOption.Unfilter();
        }

        SelectedFilterName = targetOption.Name;

        EmitSignal(SignalName.FilterSelected, SelectedFilterName);
    }

    protected FilterOption GetFilterOption(FilterType filterType) => filterType switch
    {
        FilterType.Default => PackedScenes.GetFilterOption<FilterOption>(filterType),
        FilterType.SkillType => PackedScenes.GetFilterOption<SkillTypeFilterOption>(filterType),
        FilterType.StatType => PackedScenes.GetFilterOption<StatTypeFilterOption>(filterType),
        _ => PackedScenes.GetFilterOption<FilterOption>(filterType)
    };

    private void ConnectUnfilteredSignal()
    {
        CompendiumFilter compendiumFilter = GetParent().GetOwner<CompendiumFilter>();
        compendiumFilter.Connect(CompendiumFilter.SignalName.Unfiltered,
            Callable.From(EnableDefaultFilter));
    }

    protected void EnableDefaultFilter()
    {
        FilterOption filterOption = FilterOptions[0];
        filterOption.Filter();
        filterOption.EmitSignal(FilterOption.SignalName.Selected, filterOption);
    }

    public virtual void SetFilter(string filterName)
    {
        FilterOption targetOption = FilterOptions.Find(option => option.Name == filterName);
        targetOption.Filter();
        targetOption.EmitSignal(FilterOption.SignalName.Selected, targetOption);
    }
}
