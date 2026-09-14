using Godot;

namespace PersonaAndPuzzles;

public partial class EnhancementMaterialCounter : VBoxContainer
{
    [Signal]
    public delegate void AmountChangedEventHandler(EnhancementMaterial material, int amount);

    [Export]
    private TextureRect _materialTextureRect;

    [Export]
    private CustomButton _increaseButton;

    [Export]
    private CustomButton _decreaseButton;

    [Export]
    private Label _amountLabel;

    public new EnhancementMaterial Material;
    private int _amount;

    private const int _MinCount = 0;

    public override void _Ready()
    {
        _increaseButton.Pressed += () => OnButtonPressed(true);
        _decreaseButton.Pressed += () => OnButtonPressed(false);

        ToggleButtons();
    }

    private void OnButtonPressed(bool isIncreasing)
    {
        _amount += isIncreasing ? 1 : -1;

        SetAmount(_amount);
        ToggleButtons();

        EmitSignal(SignalName.AmountChanged, (int) Material, _amount);

        SetAmountText();
    }

    private bool IsPersonaMaxLevel()
    {
        GetParent().GetOwner();
        return true;
    }

    private void SetAmount(int amount)
    {
        int maxCount = Player.GetMaterialCount($"{Material}");
        _amount = Mathf.Clamp(amount, _MinCount, maxCount);
    }

    private void ToggleButtons()
    {
        int maxCount = Player.GetMaterialCount($"{Material}");

        ToggleButton(_increaseButton, false);
        ToggleButton(_decreaseButton, false);

        if (_amount == _MinCount)
        {
            ToggleButton(_decreaseButton, true);
        }
        else if (_amount == maxCount)
        {
            ToggleButton(_increaseButton, true);
        }
    }

    private void SetAmountText()
    {
        _amountLabel.Text = $"{_amount}";
    }

    public void SetMaterial(EnhancementMaterial material)
    {
        Material = material;

        string materialUID = GetMaterialUID(material);
        Texture2D materialTexture = GD.Load<Texture2D>(materialUID);
        _materialTextureRect.Texture = materialTexture;
    }

    private string GetMaterialUID(EnhancementMaterial material) => material switch
    {
        EnhancementMaterial.Memory => "uid://dufbbmx321h60",
        EnhancementMaterial.Anecdote => "uid://bus1xbyhilr2q",
        EnhancementMaterial.Legend => "uid://ckh5v7hkh7adm",
        _ => ""
    };

    public void Reset()
    {
        _amount = 0;
        SetAmountText();
        ToggleButtons();
    }

    public void Toggle(bool isToggled)
    {
        ToggleButton(_increaseButton, isToggled);
    }

    private void ToggleButton(Button button, bool isToggled)
    {
        button.Disabled = isToggled;

        Color targetModulate = isToggled ? new Color("bfbfbf") : Colors.White;
        button.Modulate = targetModulate;
    }
}
