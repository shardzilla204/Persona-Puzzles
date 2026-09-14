using Godot;

namespace PersonaAndPuzzles;

public partial class MainMenuControls : TextureRect
{
    [Signal]
    public delegate void ButtonPressedEventHandler(Control canvas, CanvasType canvasType);
    [Export]
    private CustomButton _velvetTrialsButton;

    [Export]
    private CustomButton _compendiumButton;

    [Export]
    private CustomButton _fusionButton;

    private Control _mainMenuOverlay;
    private Control _mainMenu;

    public override void _EnterTree()
    {
        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.CompendiumScrollStarted,
            Callable.From(OnCompendiumScrollStarted));
        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.CompendiumScrollEnded,
            Callable.From(OnCompendiumScrollEnded));
        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.MoveOverlay,
            Callable.From(MoveOverlayToFront));
        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.ChangedCanvas,
            Callable.From(OnChangedCanvas));
    }

    public override void _Ready()
    {
        _velvetTrialsButton.Pressed += ShowVelvetTrialsCanvas;
        _compendiumButton.Pressed += ShowCompendiumCanvas;
        _fusionButton.Pressed += ShowFusionCanvas;

        CallDeferred(MethodName.ShowVelvetTrialsCanvas);

        _mainMenuOverlay = GetParent<Control>();
        _mainMenu = _mainMenuOverlay.GetParent<Control>();
    }

    private void OnCompendiumScrollStarted()
    {
        _velvetTrialsButton.MouseFilter = MouseFilterEnum.Ignore;
        _compendiumButton.MouseFilter = MouseFilterEnum.Ignore;
        _fusionButton.MouseFilter = MouseFilterEnum.Ignore;
    }

    private void OnCompendiumScrollEnded()
    {
        _velvetTrialsButton.MouseFilter = MouseFilterEnum.Pass;
        _compendiumButton.MouseFilter = MouseFilterEnum.Pass;
        _fusionButton.MouseFilter = MouseFilterEnum.Pass;
    }

    public void ShowVelvetTrialsCanvas()
    {
        PersonaAndPuzzles.Signals.EmitSignal(Signals.SignalName.ChangedCanvas);

        const string UID = "uid://yt0i4u74spq3";
        VelvetTrialSelection velvetTrialSelection = GD.Load<PackedScene>(UID).Instantiate<VelvetTrialSelection>();
        velvetTrialSelection.Connect(VelvetTrialSelection.SignalName.Started, 
            new Callable(this, MethodName.ShowTrialInterface));
        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.ChangedCanvas, 
            Callable.From(velvetTrialSelection.QueueFree));

        EmitSignal(SignalName.ButtonPressed, velvetTrialSelection, (int) CanvasType.VelvetTrials);

        _velvetTrialsButton.Toggle(true);
    }

    public void ShowCompendiumCanvas()
    {
        PersonaAndPuzzles.Signals.EmitSignal(Signals.SignalName.ChangedCanvas);

        const string UID = "uid://33kln8bdgqr5";
        Compendium compendium = GD.Load<PackedScene>(UID).Instantiate<Compendium>();
        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.ChangedCanvas, 
            Callable.From(compendium.QueueFree));

        EmitSignal(SignalName.ButtonPressed, compendium, (int) CanvasType.Compendium);

        _compendiumButton.Toggle(true);
    }

    public void ShowFusionCanvas()
    {
        PersonaAndPuzzles.Signals.EmitSignal(Signals.SignalName.ChangedCanvas);
        // const string UID = "";

        // Connect(SignalName.ChangedCanvas, Callable.From(compendium.QueueFree));
        _fusionButton.Toggle(true);
    }

    private void MoveOverlayToFront()
    {
        _mainMenuOverlay.MoveToFront();
    }

    private void OnChangedCanvas()
    {
        _velvetTrialsButton.Toggle(false);
        _compendiumButton.Toggle(false);
        _fusionButton.Toggle(false);
    }

    private void ShowTrialInterface(VelvetTrial velvetTrial)
    {
        _mainMenu.QueueFree();
        
        const string UID = "uid://ojbiqd2cianf";
        VelvetTrialInterface velvetTrialInterface = GD.Load<PackedScene>(UID).Instantiate<VelvetTrialInterface>();
        velvetTrialInterface.VelvetTrial = velvetTrial;
        GetTree().Root.AddChild(velvetTrialInterface);
    }
}
