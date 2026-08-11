using Godot;
using System;

namespace PersonaAndPuzzles;

public partial class MainMenuControls : TextureRect
{
    [Signal]
    public delegate void ChangedInterfaceEventHandler();

    [Export]
    private CustomButton _velvetTrialsButton;

    [Export]
    private CustomButton _personaCompendiumButton;

    [Export]
    private CustomButton _personaFusionButton;

    private Control _mainMenu;

    public override void _EnterTree()
    {
        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.CompendiumScrollStarted,
            Callable.From(OnCompendiumScrollStarted));
        PersonaAndPuzzles.Signals.Connect(Signals.SignalName.CompendiumScrollEnded,
            Callable.From(OnCompendiumScrollEnded));
    }

    public override void _Ready()
    {
        ChangedInterface += OnChangedInterface;

        _velvetTrialsButton.Pressed += OnVelvetTrialsButtonPressed;
        _personaCompendiumButton.Pressed += OnPersonaCompendiumButtonPressed;
        _personaFusionButton.Pressed += OnPersonaFusionButtonPressed;

        CallDeferred("OnPersonaCompendiumButtonPressed");

        _mainMenu = GetParent<Control>().GetParent<Control>();
    }

    private void OnCompendiumScrollStarted()
    {
        _velvetTrialsButton.MouseFilter = MouseFilterEnum.Ignore;
        _personaCompendiumButton.MouseFilter = MouseFilterEnum.Ignore;
        _personaFusionButton.MouseFilter = MouseFilterEnum.Ignore;
    }

    private void OnCompendiumScrollEnded()
    {
        _velvetTrialsButton.MouseFilter = MouseFilterEnum.Pass;
        _personaCompendiumButton.MouseFilter = MouseFilterEnum.Pass;
        _personaFusionButton.MouseFilter = MouseFilterEnum.Pass;
    }

    private void OnVelvetTrialsButtonPressed()
    {
        const string UID = "uid://yt0i4u74spq3";
        VelvetTrialSelection velvetTrialSelection = GD.Load<PackedScene>(UID).Instantiate<VelvetTrialSelection>();
        _mainMenu.AddChild(velvetTrialSelection);
        _mainMenu.MoveChild(velvetTrialSelection, 0);

        velvetTrialSelection.Started += OnTrialStarted;

        EmitSignal(SignalName.ChangedInterface);
        Connect(SignalName.ChangedInterface, 
            Callable.From(velvetTrialSelection.QueueFree));

        _velvetTrialsButton.Toggle(true);
    }

    private void OnPersonaCompendiumButtonPressed()
    {
        const string UID = "uid://33kln8bdgqr5";
        Compendium compendium = GD.Load<PackedScene>(UID).Instantiate<Compendium>();
        _mainMenu.AddChild(compendium);
        _mainMenu.MoveChild(compendium, 0);

        EmitSignal(SignalName.ChangedInterface);
        Connect(SignalName.ChangedInterface, 
            Callable.From(compendium.QueueFree));

        _personaCompendiumButton.Toggle(true);
    }

    private void OnPersonaFusionButtonPressed()
    {
        // const string UID = "";

        EmitSignal(SignalName.ChangedInterface);
        // Connect(SignalName.ChangedInterface, Callable.From(compendium.QueueFree));
        _personaFusionButton.Toggle(true);
    }

    private void OnChangedInterface()
    {
        _velvetTrialsButton.Toggle(false);
        _personaCompendiumButton.Toggle(false);
        _personaFusionButton.Toggle(false);

        _mainMenu.MoveToFront();
    }

    private void OnTrialStarted(VelvetTrial velvetTrial)
    {
        _mainMenu.QueueFree();
        
        GD.Print("Started Trial");
        const string UID = "uid://ojbiqd2cianf";
        VelvetTrialInterface velvetTrialInterface = GD.Load<PackedScene>(UID).Instantiate<VelvetTrialInterface>();
        GetTree().Root.AddChild(velvetTrialInterface);

    }
}
