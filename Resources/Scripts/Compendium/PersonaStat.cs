using Godot;

namespace PersonaAndPuzzles;

public partial class PersonaStat : HBoxContainer
{
    [Export]
    private Label _statTypeLabel;

    [Export]
    private Label _statValueLabel;

    [Export]
    private ProgressBar _statProgressBar;

    public void Set(string statAbbreviation, int statValue)
    {
        _statTypeLabel.Text = statAbbreviation;
        _statValueLabel.Text = $"{statValue}";
        _statProgressBar.Value = statValue;
    }
}
