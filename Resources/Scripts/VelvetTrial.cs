using Godot;
using System;
using System.Collections.Generic;

namespace PersonaAndPuzzles;

public partial class VelvetTrial : Node
{
    public int ID;
    public List<VelvetTrialWave> Waves = new List<VelvetTrialWave>();
}
