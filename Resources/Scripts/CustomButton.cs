using Godot;

namespace PersonaAndPuzzles;

public partial class CustomButton : Button
{
    [Export]
    private Control _texture;
    
    const float DarkAmount = 0.2f;

    private Color _originalModulate;

    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        Toggled += OnToggled;

        _originalModulate = Modulate;

        MouseDefaultCursorShape = CursorShape.PointingHand;
    }

    private void OnMouseEntered()
    {
        if (ButtonPressed || Disabled) return;

        Modulate = _originalModulate.Darkened(DarkAmount);
    }

    private void OnMouseExited()
    {
        if (ButtonPressed || Disabled) return;

        Modulate = _originalModulate;
    }

    private void OnToggled(bool isToggled)
    {
        ButtonPressed = isToggled;
        Modulate = isToggled ? _originalModulate.Darkened(DarkAmount) : _originalModulate;
    }

    public void Toggle(bool isToggled)
    {
        OnToggled(isToggled);
    }

    public new void SetModulate(Color color)
    {
        _texture.SelfModulate = color;
    }
}
