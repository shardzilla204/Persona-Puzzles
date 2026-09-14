using Godot;

namespace PersonaAndPuzzles;

public partial class CompendiumEnhanceSlot : PersonaSlot
{
    [Signal]
    public delegate void ToggledEventHandler(CompendiumEnhanceSlot slot, bool isToggled);

    [Export]
    private Label _personaLevelLabel;

    [Export]
    private Button _button;

    private bool _isHovering;
    private bool _isMousePressed;

    public override void _Ready()
    {
        base._Ready();
        _button.Toggled += OnButtonToggled;
    }

    public override void SetPersona(Persona persona)
    {
        base.SetPersona(persona);
        _personaLevelLabel.Text = $"Lvl. {persona.Level}";
    }

    private void OnButtonToggled(bool isToggled)
    {
        Toggle(isToggled);
        EmitSignal(SignalName.Toggled, this, isToggled);
    }

    public void Toggle(bool isToggled)
    {
        Modulate = isToggled ? new Color("bfbfbf") : Colors.White;
    }
}
