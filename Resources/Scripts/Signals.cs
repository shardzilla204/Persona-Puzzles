using Godot;
using System;

namespace PersonaAndPuzzles;

public partial class Signals : Node
{
    private static readonly Lazy<Signals> s_lazy = new Lazy<Signals>(() => new Signals());
	public static Signals Instance => s_lazy.Value;

	private Signals() {}

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

    [Signal]
    public delegate void MoveOverlayEventHandler();

    [Signal]
    public delegate void ChangedCanvasEventHandler();

    [Signal]
    public delegate void PersonaRemovedEventHandler();
}
