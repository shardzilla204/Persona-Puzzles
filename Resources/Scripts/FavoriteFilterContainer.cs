using Godot.Collections;

namespace PersonaAndPuzzles;

public partial class FavoriteFilterContainer : FilterContainer
{
    private enum Filter
    {
        All,
        Favorite,
        Unfavorite    
    }

    private Dictionary<Filter, FilterType> _filters = new Dictionary<Filter, FilterType>()
    {
        { Filter.All, FilterType.Default },
        { Filter.Favorite, FilterType.Default },
        { Filter.Unfavorite, FilterType.Default },
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
