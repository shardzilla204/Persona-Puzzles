using Godot;

namespace PersonaAndPuzzles;

public partial class PlayerInformation : TextureRect
{
    [Export]
    private Label _nameLabel;

    [Export]
    private Label _levelLabel;

    [Export]
    private Label _konpakuGemsLabel;

    [Export]
    private Label _experienceLabel;

    [Export]
    private ProgressBar _experienceProgressBar;

    [Export]
    private Label _staminaLabel;

    [Export]
    private ProgressBar _staminaProgressBar;

    public override void _Ready()
    {
        _nameLabel.Text = Player.Instance.Name;
        _levelLabel.Text = $"LVL. {Player.Instance.Level}";
        _konpakuGemsLabel.Text = $"{Player.Instance.KonpakuGems:N0}";

        string experienceNotated = PersonaAndPuzzles.GetNotation(Player.Instance.Experience);
        string maxExperienceNotated = PersonaAndPuzzles.GetNotation(Player.Instance.MaxExperience);
        _experienceLabel.Text = $"{experienceNotated}/{maxExperienceNotated}";
        _staminaLabel.Text = $"{Player.Instance.Stamina}/{Player.Instance.MaxStamina}";

        _experienceLabel.Visible = false;
        _staminaLabel.Visible = false;

        _experienceProgressBar.Value = Player.Instance.Experience;
        _experienceProgressBar.MaxValue = Player.Instance.MaxExperience;

        _staminaProgressBar.Value = Player.Instance.Stamina;
        _staminaProgressBar.MaxValue = Player.Instance.MaxStamina;

        _experienceProgressBar.MouseEntered += OnMouseEnteredExperienceBar;
        _experienceProgressBar.MouseExited += OnMouseExitedExperienceBar;

        _staminaProgressBar.MouseEntered += OnMouseEnteredStaminaBar;
        _staminaProgressBar.MouseExited += OnMouseExitedStaminaBar;
    }

    public override void _Process(double delta)
    {
        if (_experienceLabel.Visible)
        {
            _experienceLabel.Position = GetLocalMousePosition() - (_experienceLabel.Size / 2);
        }

        if (_staminaLabel.Visible)
        {
            _staminaLabel.Position = GetLocalMousePosition() - (_staminaLabel.Size / 2);
        }
    }

    private void OnMouseEnteredExperienceBar()
    {
        _experienceLabel.Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Hidden;
    }

    private void OnMouseExitedExperienceBar()
    {
        _experienceLabel.Visible = false;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    private void OnMouseEnteredStaminaBar()
    {
        _staminaLabel.Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Hidden;
    }

    private void OnMouseExitedStaminaBar()
    {
        _staminaLabel.Visible = false;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }
}
