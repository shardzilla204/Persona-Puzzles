using Godot;
using System;

namespace PersonaAndPuzzles;

public partial class Signals : Node
{
    [Signal]
    public delegate void MouseReleasedEventHandler();

    [Signal]
    public delegate void CompendiumScrollStartedEventHandler();

    [Signal]
    public delegate void CompendiumScrollEndedEventHandler();

    [Signal]
    public delegate void CompendiumSlotLoadedEventHandler();

    [Signal]
    public delegate void LoadingBarFillingEventHandler();
}
