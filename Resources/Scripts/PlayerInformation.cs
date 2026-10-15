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
        SetStaminaLabelText();

        _experienceLabel.Visible = false;
        _staminaLabel.Visible = false;

        _experienceProgressBar.Value = Player.Instance.Experience;
        _experienceProgressBar.MaxValue = Player.Instance.MaxExperience;

        _staminaProgressBar.Value = Player.Instance.Stamina;
        _staminaProgressBar.MaxValue = Player.Instance.MaxStamina;

        _experienceProgressBar.MouseEntered += ShowExperience;
        _experienceProgressBar.MouseExited += HideExperience;

        _staminaProgressBar.MouseEntered += ShowStamina;
        _staminaProgressBar.MouseExited += HideStamina;
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

    private void ShowExperience()
    {
        _experienceLabel.Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Hidden;
    }

    private void HideExperience()
    {
        _experienceLabel.Visible = false;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    private void ShowStamina()
    {
        SetStaminaLabelText();

        Timer timer = new Timer()
        {
            WaitTime = 1,
            Autostart = true
        };
        timer.Connect(Timer.SignalName.Timeout, 
            Callable.From(SetStaminaLabelText));
        _staminaProgressBar.Connect(Control.SignalName.MouseExited, 
            Callable.From(timer.QueueFree));
        _staminaLabel.AddChild(timer);

        _staminaLabel.Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Hidden;
    }

    private void HideStamina()
    {
        _staminaLabel.Visible = false;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    private void SetStaminaLabelText()
    {
        string timeFormatted = StaminaManager.Instance.GetTimeFormatted();
        string text = $"{Player.Instance.Stamina}/{Player.Instance.MaxStamina}";

        text += StaminaManager.Instance.IsStaminaFull() ? "" : $" ({timeFormatted})";
        _staminaLabel.Text = $"{text}";
    }
}
