using Godot;
using Godot.Collections;

namespace PersonaAndPuzzles;

public partial class VelvetTrialWave : Node
{
    public VelvetTrialWave(Array<Dictionary<string, Variant>> personaDictionaries)
    {
        PersonaDictionaries = personaDictionaries;
    }
    public Array<Dictionary<string, Variant>> PersonaDictionaries = new Array<Dictionary<string, Variant>>();
}
