using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

namespace PersonaAndPuzzles;

public partial class PersonaManager : Node
{
    public const int MaxStat = 99;
    public const int MaxPassiveSkills = 3;
    public const int MaxPersonas = 6;
    public const int MaxPersonaReserves = 3;
    public const int MaxLoadouts = 10;

    private static List<Persona> _Personas = new List<Persona>();
    public static List<Persona> Personas = new List<Persona>();

    public static PersonaRoster Roster = new PersonaRoster("Loadout 1", 0);
    public static List<PersonaRoster> Loadouts = new List<PersonaRoster>();

    public static void GetRandomPersonas()
    {
        for (int i = 0; i < PersonaAndPuzzles.StartingPersonaCount; i++)
        {
            Persona persona = GetRandomPersona();
            Personas.Add(persona);
        }
    }

    public static void SetRandomRoster()
    {
        for (int i = 0; i < MaxPersonas; i++)
        {
            Persona persona = GetRandomPersona();
            Roster.Personas.Add(persona);
            Personas.Add(persona);
        }

        // for (int i = 0; i < MaxPersonaReserves; i++)
        // {
        //     Persona persona = GetRandomPersona();
        //     Roster.Reserves.Add(persona);
        // }
        
        PersonaRoster rosterClone = new PersonaRoster(Roster);
        Loadouts.Add(rosterClone);

        SetRandomLoadouts();
        SetEmptyLoadouts();
    }

    private static void SetRandomLoadouts()
    {
        List<PersonaRoster> loadouts = new List<PersonaRoster>();
        for (int i = 0; i < PersonaAndPuzzles.ExtraLoadouts; i++)
        {
            string name = $"Loadout {i + 1}";
            PersonaRoster roster = new PersonaRoster(name, i);
            for (int j = 0; j < MaxPersonas; j++)
            {
                Persona persona = GetRandomPersona();
                roster.Personas.Add(persona);
            }

            // for (int j = 0; j < MaxPersonaReserves; j++)
            // {
            //     Persona persona = GetRandomPersona();
            //     roster.Reserves.Add(persona);
            // }

            loadouts.Add(roster);
        }
        Loadouts.AddRange(loadouts);
    }

    private static void SetEmptyLoadouts()
    {
        List<PersonaRoster> loadouts = new List<PersonaRoster>();
        for (int i = Loadouts.Count; i < MaxLoadouts; i++)
        {
            string name = $"Loadout {i + 1}";
            PersonaRoster roster = new PersonaRoster(name, i);
            for (int j = 0; j < MaxPersonas; j++)
            {
                roster.Personas.Add(null);
            }
            loadouts.Add(roster);
        }
        Loadouts.AddRange(loadouts);
    }

    public static void SetLoadout(Persona newPersona, int slotIndex)
    {
        Roster.Swap(newPersona, slotIndex);

        PersonaRoster targetRoster = Loadouts.Find(loadout => loadout.Name == Roster.Name);
        targetRoster.Swap(newPersona, slotIndex);
    }

    public static void SetLoadout(Persona newPersona, string oldPersonaID)
    {
        Roster.Swap(newPersona, oldPersonaID);

        PersonaRoster targetRoster = Loadouts.Find(loadout => loadout.Name == Roster.Name);
        targetRoster.Swap(newPersona, oldPersonaID);
    }

    public static void LoadPersonas()
    {
        const string FileName = "Persona";

        // Keys
        const string NameKey = "Name";
        const string ImageKey = "Image";
        const string ResistancesKey = "Resistances";
        const string PassiveSkillsKey = "PassiveSkills";
        const string LevelKey = "Level";

        // Stat keys
        const string StrengthKey = "Strength";
        const string MagicKey = "Magic";
        const string EnduranceKey = "Endurance";
        const string AgilityKey = "Agility";
        const string LuckKey = "Luck";

        GC.Dictionary<string, Variant> personaData = (GC.Dictionary<string, Variant>) PersonaAndPuzzles.LoadFile(FileName);
        List<GC.Dictionary<string, Variant>> personaDictionaries = personaData[FileName].As<GC.Array<GC.Dictionary<string, Variant>>>().ToList();
        foreach (GC.Dictionary<string, Variant> personaDictionary in personaDictionaries)
        {
            Persona persona = new Persona()
            {
                Name = personaDictionary[NameKey].As<string>(),
                Image = personaDictionary[ImageKey].As<string>(),
                Race = GetRaceFromDictionary(personaDictionary),
                SkillType = GetSkillTypeFromDictionary(personaDictionary),
                Level = personaDictionary[LevelKey].As<int>(),

                // Stats
                Strength = personaDictionary[StrengthKey].As<int>(),
                Magic = personaDictionary[MagicKey].As<int>(),
                Endurance = personaDictionary[EnduranceKey].As<int>(),
                Agility = personaDictionary[AgilityKey].As<int>(),
                Luck = personaDictionary[LuckKey].As<int>(),
            };
            GC.Dictionary<string, Variant> resistancesDictionary = personaDictionary[ResistancesKey].As<GC.Dictionary<string, Variant>>();
            SetResistances(persona, resistancesDictionary);

            GC.Array<string> passiveSkills = personaDictionary[PassiveSkillsKey].As<GC.Array<string>>();
            SetPassiveSkills(persona, passiveSkills);

            _Personas.Add(persona);
        }
    }

    private static void SetResistances(Persona persona, GC.Dictionary<string, Variant> resistances)
    {
        foreach (string key in resistances.Keys)
        {
            SkillType skillType = Enum.Parse<SkillType>(key);
            Resistance resistance = Enum.Parse<Resistance>(resistances[key].As<string>());
            persona.Resistances[skillType] = resistance;
        }
    }

    private static void SetPassiveSkills(Persona persona, GC.Array<string> passiveSkills)
    {
        foreach (string passiveSkillName in passiveSkills)
        {
            PassiveSkill passiveSkill = PassiveSkillManager.FindPassiveSkill(passiveSkillName);
            persona.PassiveSkills.Add(passiveSkill);
        }
    }

    private static Race GetRaceFromDictionary(GC.Dictionary<string, Variant> personaDictionary)
    {
        const string RaceKey = "Race";
        string personaRace = personaDictionary[RaceKey].As<string>();
        try
        {
            Enum.Parse<Race>(personaRace);
            return Enum.Parse<Race>(personaDictionary[RaceKey].As<string>());
        }
        catch
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
			string message = $"Couldn't Parse Race - {personaRace}";
			string result = "Returning Race.Fool";
			PrintRich.PrintError(className, message, result);
            
            return Race.Fool;
        }
    }

    private static SkillType GetSkillTypeFromDictionary(GC.Dictionary<string, Variant> personaDictionary)
    {
        const string SkillTypeKey = "SkillType";
        string personaSkillType = personaDictionary[SkillTypeKey].As<string>();
        try
        {
            Enum.Parse<SkillType>(personaSkillType);
            return Enum.Parse<SkillType>(personaDictionary[SkillTypeKey].As<string>());
        }
        catch
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
			string message = $"Couldn't Parse SkillType - {personaSkillType}";
			string result = "Returning SkillType.None";
			PrintRich.PrintError(className, message, result);
            
            return SkillType.None;
        }
    }

    public static Texture2D GetPersonaImage(Persona persona)
    {
        if (string.IsNullOrEmpty(persona.Image) || persona.Image == "No Image")
        {
            GD.PrintErr($"Couldn't load Persona image of {persona.Name}");
            return null;
        }
        else
        {
            Texture2D personaImage = GD.Load<Texture2D>(persona.Image);
            return personaImage;
        }
    }

    public static Persona GetRandomPersona()
    {
        RandomNumberGenerator RNG = new RandomNumberGenerator();
        int randomNumber = RNG.RandiRange(0, _Personas.Count - 1);
        Persona persona = _Personas[randomNumber];
        Persona personaClone = new Persona(persona)
        {
            ID = Guid.NewGuid().ToString("N")
        };
        return personaClone;
    }

    public static List<Persona> GetPersonasByRace(Race race)
    {
        return _Personas.FindAll(persona => persona.Race == race);
    }

    public static List<Persona> GetPersonasBySkillType(SkillType skillType)
    {
        return _Personas.FindAll(persona => persona.SkillType == skillType);
    }

    public static Persona GetPersonaByName(string personaName)
    {
        return _Personas.Find(persona => persona.Name == personaName);
    }

    public static void SetLevel(Persona persona, int targetLevel)
    {
        int difference = (targetLevel - persona.Level) / 2;
        if (difference > 0)
        {
            persona.Strength = Mathf.Min(persona.Strength + difference, MaxStat);
            persona.Magic = Mathf.Min(persona.Magic + difference, MaxStat);
            persona.Endurance = Mathf.Min(persona.Endurance + difference, MaxStat);
            persona.Agility = Mathf.Min(persona.Agility + difference, MaxStat);
            persona.Luck = Mathf.Min(persona.Luck + difference, MaxStat);
        }
        else
        {
            const int MinStat = 1;
            persona.Strength = Mathf.Max(MinStat, persona.Strength + difference);
            persona.Magic = Mathf.Max(MinStat, persona.Magic + difference);
            persona.Endurance = Mathf.Max(MinStat, persona.Endurance + difference);
            persona.Agility = Mathf.Max(MinStat, persona.Agility + difference);
            persona.Luck = Mathf.Max(MinStat, persona.Luck + difference);
        }
        persona.Level = targetLevel;
    }
}

