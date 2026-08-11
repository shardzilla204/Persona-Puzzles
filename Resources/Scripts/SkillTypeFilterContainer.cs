using Godot.Collections;

namespace PersonaAndPuzzles;

public partial class SkillTypeFilterContainer : FilterContainer
{
    private enum Filter
    {
        All,
        Physical,
        Pierce,
        Fire,
        Ice,
        Electric,
        Wind,
        Psychic,
        Nuclear,
        Bless,
        Curse,
        Almighty
    }

    private Dictionary<Filter, FilterType> _filters = new Dictionary<Filter, FilterType>()
    {
        { Filter.All, FilterType.Default },
        { Filter.Physical, FilterType.SkillType },
        { Filter.Pierce, FilterType.SkillType },
        { Filter.Fire, FilterType.SkillType },
        { Filter.Ice, FilterType.SkillType },
        { Filter.Wind, FilterType.SkillType },
        { Filter.Electric, FilterType.SkillType },
        { Filter.Psychic, FilterType.SkillType },
        { Filter.Nuclear, FilterType.SkillType },
        { Filter.Bless, FilterType.SkillType },
        { Filter.Curse, FilterType.SkillType },
        { Filter.Almighty, FilterType.SkillType }
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
}
