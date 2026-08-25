using System;
using System.Collections.Generic;
using GC = Godot.Collections;

namespace PersonaAndPuzzles;

public partial class StatTypeFilterContainer : FilterContainer
{
    private enum Filter
    {
        All,
        Strength,
        Magic,
        Endurance,
        Agility,
        Luck
    }

    private GC.Dictionary<Filter, FilterType> _filters = new GC.Dictionary<Filter, FilterType>()
    {
        { Filter.All, FilterType.Default },
        { Filter.Strength, FilterType.StatType },
        { Filter.Magic, FilterType.StatType },
        { Filter.Endurance, FilterType.StatType },
        { Filter.Agility, FilterType.StatType },
        { Filter.Luck, FilterType.StatType }
    };

    public override void _Ready()
    {
        base._Ready();
        AddFilters();
    }

    private void AddFilters()
    {
        foreach (Filter filter in _filters.Keys)
        {
            FilterType filterType = _filters[filter];
            AddFilter(filterType, $"{filter}");
        }

        EnableDefaultFilter();
    }

    public override void EnableFilter(string filterName)
    {
        string[] filterStrings = filterName.Split("|");
        string statType = filterStrings[0];
        string state = filterStrings[1];

        List<StatTypeFilterOption> statTypeFilterOptions = new List<StatTypeFilterOption>();
        foreach (FilterOption filterOption in FilterOptions)
        {
            if (filterOption is not StatTypeFilterOption statTypeFilterOption) continue;
            
            statTypeFilterOptions.Add(statTypeFilterOption);
        }

        StatType targetStatType = Enum.Parse<StatType>(statType);
        StatTypeFilterOption targetStatTypeFilterOption = statTypeFilterOptions.Find(option => option.StatType == targetStatType);
        targetStatTypeFilterOption.SetState(state);
    }

}
