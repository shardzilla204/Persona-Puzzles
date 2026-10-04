using Godot;
using System;

namespace PersonaAndPuzzles;

public partial class StaminaManager : Node
{
    private readonly static Lazy<StaminaManager> s_lazy = new Lazy<StaminaManager>(() => new StaminaManager());
    public static StaminaManager Instance => s_lazy.Value;

    private StaminaManager() {}
}
