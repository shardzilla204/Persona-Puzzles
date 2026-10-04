using Godot;
using System.Collections.Generic;

namespace PersonaAndPuzzles;

public partial class VelvetTrial : Node
{
    public int ID;
    public List<VelvetTrialWave> Waves = new List<VelvetTrialWave>();
    public int Experience;
    public int Stamina;
    public int KonpakuGems;
}
