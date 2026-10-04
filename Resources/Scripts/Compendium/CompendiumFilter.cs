using Godot;

namespace PersonaAndPuzzles;

public partial class CompendiumFilter : Control
{
    [Signal]
    public delegate void ConfirmedEventHandler();

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

    public override void _Ready()
    {
        _cancelButton.Pressed += QueueFree;
        _unfilterButton.Pressed += () => EmitSignal(SignalName.Unfiltered);
        _confirmButton.Pressed += OnConfirmButtonPressed;

        _skillTypeFilters.FilterSelected += (filterName) => PersonaManager.Instance.SkillTypeFilter = filterName;
        _favoriteFilters.FilterSelected += (filterName) => PersonaManager.Instance.FavoriteFilter = filterName;
        _statTypeFilters.FilterSelected += (filterName) => PersonaManager.Instance.StatTypeFilter = filterName;

        if (!string.IsNullOrEmpty(PersonaManager.Instance.SkillTypeFilter))
        {
            _skillTypeFilters.EnableFilter(PersonaManager.Instance.SkillTypeFilter);
        }
        else
        {
            PersonaManager.Instance.SkillTypeFilter = _skillTypeFilters.SelectedFilterName;
        }

        if (!string.IsNullOrEmpty(PersonaManager.Instance.FavoriteFilter))
        {
            _favoriteFilters.EnableFilter(PersonaManager.Instance.FavoriteFilter);
        }
        else
        {
            PersonaManager.Instance.FavoriteFilter = _favoriteFilters.SelectedFilterName;
        }

        if (!string.IsNullOrEmpty(PersonaManager.Instance.StatTypeFilter) && PersonaManager.Instance.StatTypeFilter.Contains("|") /* If the string is 'All' */)
        {
            _statTypeFilters.EnableFilter(PersonaManager.Instance.StatTypeFilter);
        }
        else
        {
            PersonaManager.Instance.StatTypeFilter = _statTypeFilters.SelectedFilterName;
        }
    }

    private void OnConfirmButtonPressed()
    {
        EmitSignal(SignalName.Confirmed);
        QueueFree();
    }
}
