using Godot;
using System;

namespace PersonaAndPuzzles;

public partial class Debug : Control
{
    [Export]
    public Button _spawnSkillOrb;

    public override void _Ready()
    {
        _spawnSkillOrb.Pressed += SpawnSkillOrb;
    }

    private void SpawnSkillOrb()
    {
        
    }
}
