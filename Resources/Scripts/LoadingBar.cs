using Godot;

namespace PersonaAndPuzzles;

public partial class LoadingBar : TextureProgressBar
{
    [Signal]
    public delegate void FilledEventHandler();

    [Export]
    private Timer _delayTimer;

    private bool _hasTimeout = false;
    private Vector2 _mousePosition;

    private float _margin = 5;

    public override void _EnterTree()
    {
        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.MouseReleased, 
            Callable.From(QueueFree));
        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.CompendiumScrollStarted, 
            Callable.From(QueueFree));
    }

    public override void _Ready()
    {
        ValueChanged += OnValueChanged;
        _delayTimer.Timeout += OnTimeout;
        _mousePosition = GetGlobalMousePosition();
    }

    public override void _Process(double delta)
    {
        if (_mousePosition != GetGlobalMousePosition()) QueueFree();

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
