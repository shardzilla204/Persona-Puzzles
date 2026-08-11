using Godot;
using GC = Godot.Collections;
using System;
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

        GC.Dictionary<string, Variant> velvetTrialData = (GC.Dictionary<string, Variant>) PersonaAndPuzzles.LoadFile(FileName);
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
            VelvetTrialWave velvetTrialWave = new VelvetTrialWave()
            {
                Personas = GetPersonas(wave)
            };
            velvetTrialWaves.Add(velvetTrialWave);
        }
        return velvetTrialWaves;
    }

    private static List<Persona> GetPersonas(GC.Array<GC.Dictionary<string, Variant>> wave)
    {
        const string NameKey = "Name";
        const string LevelKey = "Level";

        List<Persona> personas = new List<Persona>();
        foreach (GC.Dictionary<string, Variant> personaDictionary in wave)
        {
            string personaName = personaDictionary[NameKey].As<string>();
            int personaLevel = personaDictionary[LevelKey].As<int>();
            Persona persona = PersonaManager.GetPersonaByName(personaName);
            Persona personaClone = new Persona(persona);
            PersonaManager.SetLevel(personaClone, personaLevel);
            personas.Add(personaClone);
        }
        return personas;
    }
}
