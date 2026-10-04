using Godot;

namespace PersonaAndPuzzles;

public partial class CustomButton : Button
{
    [Export]
    private Control _texture;
    
    protected const float DarkAmount = 0.2f;

    protected Color _originalModulate;

    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        Toggled += OnToggled;

        _originalModulate = Modulate;

        MouseDefaultCursorShape = CursorShape.PointingHand;

        if (Disabled)
        {
            Modulate = _originalModulate.Darkened(DarkAmount);
        }
    }

    protected virtual void OnMouseEntered()
    {
        if (ButtonPressed || Disabled) return;

        Modulate = _originalModulate.Darkened(DarkAmount);
    }

    protected virtual void OnMouseExited()
    {
        if (ButtonPressed || Disabled) return;

        Modulate = _originalModulate;
    }

    protected virtual void OnToggled(bool isToggled)
    {
        Modulate = isToggled ? _originalModulate.Darkened(DarkAmount) : _originalModulate;
        ButtonPressed = isToggled;
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
