using Godot;

namespace PersonaAndPuzzles;

public partial class LoadoutNameEdit : Control
{
    [Signal]
    public delegate void ConfirmedEventHandler(string name);
    
    [Export]
    private Button _backdropButton;

    [Export]
    private LineEdit _nameLineEdit;

    [Export]
    private CustomButton _cancelButton;

    [Export]
    private CustomButton _confirmButton;

    public override void _Ready()
    {
        _nameLineEdit.TextChanged += OnTextChanged;
        _backdropButton.Pressed += QueueFree;
        _cancelButton.Pressed += QueueFree;
        _confirmButton.Pressed += Confirm;

        _nameLineEdit.GrabFocus(true);
    }

    public override void _Process(double delta)
    {
        if (!Input.IsKeyPressed(Key.Enter)) return;
        
        Confirm();
    }

    public void SetText(string text)
    {
        _nameLineEdit.Text = text;
    }

    private void OnTextChanged(string text)
    {
        const int MinLength = 1;
        const int MaxLength = 16;

        _confirmButton.Disabled = text.Length < MinLength || text.Length > MaxLength;
    }

    private void Confirm()
    {
        string name = _nameLineEdit.Text;
        EmitSignal(SignalName.Confirmed, name);
        QueueFree();
    }
}
