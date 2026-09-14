using Godot;
using System;

namespace PersonaAndPuzzles;

public partial class ConfirmationInterface : ColorRect
{
    [Signal]
    public delegate void AcceptedEventHandler();

    [Signal]
    public delegate void CanceledEventHandler();

    [Export]
    private Label _warningLabel;

    [Export]
    private CustomButton _acceptButton;

    [Export]
    private CustomButton _cancelButton;

    public override void _Ready()
    {
        _acceptButton.Pressed += OnAcceptButtonPressed;
        _cancelButton.Pressed += OnCancelButtonPressed;
    }

    public void SetWarningText(string warning)
    {
        _warningLabel.Text = warning;
    }

    private void OnAcceptButtonPressed()
    {
        EmitSignal(SignalName.Accepted);
    }

    private void OnCancelButtonPressed()
    {
        EmitSignal(SignalName.Canceled);
        QueueFree();
    }
}
