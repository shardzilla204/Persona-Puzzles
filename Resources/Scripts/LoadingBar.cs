using Godot;

namespace PersonaAndPuzzles;

public partial class LoadingBar : TextureProgressBar
{
    [Signal]
    public delegate void FilledEventHandler();

    [Export]
    private Timer _delayTimer;

    private bool _hasTimeout = false;


    public override void _Ready()
    {
        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.MouseReleased, 
            Callable.From(QueueFree));
        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.CompendiumScrollStarted, 
            Callable.From(QueueFree));

        ValueChanged += OnValueChanged;
        _delayTimer.Timeout += OnTimeout;
    }

    public override void _Process(double delta)
    {
        GlobalPosition = GetGlobalMousePosition() - Size / 2;

        if (!_hasTimeout) return;

        Value += Step;
    }

    private void OnValueChanged(double value)
    {
        if (Value < MaxValue) return;

        EmitSignal(SignalName.Filled);
        QueueFree();
    }

    private void OnTimeout()
    {
        _hasTimeout = true;
        PersonaAndPuzzles.Signals.EmitSignal(Signals.SignalName.LoadingBarFilling);
    }
}
