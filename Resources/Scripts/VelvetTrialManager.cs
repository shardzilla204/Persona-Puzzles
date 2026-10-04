using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PersonaAndPuzzles;

public partial class VelvetTrialManager : Node
{
    private readonly static Lazy<VelvetTrialManager> s_lazy = new Lazy<VelvetTrialManager>(() => new VelvetTrialManager());
    public static VelvetTrialManager Instance => s_lazy.Value;

    private VelvetTrialManager()
    {
        LoadVelvetTrials();
    }

    public List<VelvetTrial> VelvetTrials = new List<VelvetTrial>();

    private void LoadVelvetTrials()
    {
        const string FileName = "VelvetTrials";
        const string IDKey = "ID";
        const string WaveKey = "Waves";
        const string ExperienceKey = "Experience";
        const string StaminaKey = "Stamina";
        const string KonpakuGemsKey = "KonpakuGems";

        GC.Dictionary<string, Variant> velvetTrialData = PersonaAndPuzzles.LoadFile(FileName).As<GC.Dictionary<string, Variant>>();
        List<GC.Dictionary<string, Variant>> velvetTrialDictionaries = velvetTrialData[FileName].As<GC.Array<GC.Dictionary<string, Variant>>>().ToList();
        foreach (GC.Dictionary<string, Variant> velvetTrialDictionary in velvetTrialDictionaries)
        {
            GC.Array<Variant> waves = velvetTrialDictionary[WaveKey].As<GC.Array<Variant>>();
            VelvetTrial velvetTrial = new VelvetTrial()
            {
                ID = velvetTrialDictionary[IDKey].As<int>(),
                Waves = GetVelvetTrialWaves(waves),
                Experience = velvetTrialDictionary[ExperienceKey].As<int>(),
                Stamina = velvetTrialDictionary[StaminaKey].As<int>(),
                KonpakuGems = velvetTrialDictionary[KonpakuGemsKey].As<int>()
            };
            VelvetTrials.Add(velvetTrial);
        }
    }

    private List<VelvetTrialWave> GetVelvetTrialWaves(GC.Array<Variant> waves)
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
