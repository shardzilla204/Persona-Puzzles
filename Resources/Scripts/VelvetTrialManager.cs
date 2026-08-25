using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PersonaAndPuzzles;

public partial class VelvetTrialManager : Node
{
    public static List<VelvetTrial> VelvetTrials = new List<VelvetTrial>();

    public static void LoadVelvetTrials()
    {
        const string FileName = "VelvetTrials";
        const string IDKey = "ID";
        const string WaveKey = "Waves";

        GC.Dictionary<string, Variant> velvetTrialData = PersonaAndPuzzles.LoadFile(FileName).As<GC.Dictionary<string, Variant>>();
        List<GC.Dictionary<string, Variant>> velvetTrialDictionaries = velvetTrialData[FileName].As<GC.Array<GC.Dictionary<string, Variant>>>().ToList();
        foreach (GC.Dictionary<string, Variant> velvetTrialDictionary in velvetTrialDictionaries)
        {
            GC.Array<Variant> waves = velvetTrialDictionary[WaveKey].As<GC.Array<Variant>>();
            VelvetTrial velvetTrial = new VelvetTrial()
            {
                ID = velvetTrialDictionary[IDKey].As<int>(),
                Waves = GetVelvetTrialWaves(waves)
            };
            VelvetTrials.Add(velvetTrial);
        }
    }

    private static List<VelvetTrialWave> GetVelvetTrialWaves(GC.Array<Variant> waves)
    {
        List<VelvetTrialWave> velvetTrialWaves = new List<VelvetTrialWave>();
        foreach (GC.Array<GC.Dictionary<string, Variant>> wave in waves)
        {
            VelvetTrialWave velvetTrialWave = new VelvetTrialWave(wave);
            velvetTrialWaves.Add(velvetTrialWave);
        }
        return velvetTrialWaves;
    }
}
