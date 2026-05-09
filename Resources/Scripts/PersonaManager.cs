using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

namespace PersonaAndPuzzles;

public partial class PersonaManager : Node
{
    // Keys
    private const string _NameKey = "Name";
    private const string _ImageKey = "Image";
    private const string _RaceKey = "Race";
    private const string _SkillTypeKey = "SkillType";
    private const string _ResistancesKey = "Resistances";
    private const string _LevelKey = "Level";

    // Stat Keys
    private const string _StrengthKey = "Strength";
    private const string _MagicKey = "Magic";
    private const string _EnduranceKey = "Endurance";
    private const string _AgilityKey = "Agility";
    private const string _LuckKey = "Luck";

    private const string _PersonaFileName = "Persona";

    public const int MaxStat = 99;
    public const int MaxPassiveSkills = 3;

    public static List<Persona> Personas = new List<Persona>();

    public static void LoadPersonas()
    {
        GC.Dictionary<string, Variant> personaData = (GC.Dictionary<string, Variant>) PersonaAndPuzzles.LoadFile(_PersonaFileName);
        List<GC.Dictionary<string, Variant>> personaDictionaries = personaData[_PersonaFileName].As<GC.Array<GC.Dictionary<string, Variant>>>().ToList();
        foreach (GC.Dictionary<string, Variant> personaDictionary in personaDictionaries)
        {
            Persona persona = new Persona()
            {
                Name = personaDictionary[_NameKey].As<string>(),
                Image = personaDictionary[_ImageKey].As<string>(),
                Race = GetRaceFromDictionary(personaDictionary),
                SkillType = GetSkillTypeFromDictionary(personaDictionary),
                Level = personaDictionary[_LevelKey].As<int>(),

                // Stats
                Strength = personaDictionary[_StrengthKey].As<int>(),
                Magic = personaDictionary[_MagicKey].As<int>(),
                Endurance = personaDictionary[_EnduranceKey].As<int>(),
                Agility = personaDictionary[_AgilityKey].As<int>(),
                Luck = personaDictionary[_LuckKey].As<int>(),
            };
            GC.Dictionary<string, Variant> resistancesDictionary = personaDictionary[_ResistancesKey].As<GC.Dictionary<string, Variant>>();
            SetResistances(persona, resistancesDictionary);
            Personas.Add(persona);
        }
    }

    private static void SetResistances(Persona persona, GC.Dictionary<string, Variant> resistancesDictionary)
    {
        foreach (string key in resistancesDictionary.Keys)
        {
            SkillType skillType = Enum.Parse<SkillType>(key);
            Resistance value = Enum.Parse<Resistance>(resistancesDictionary[key].As<string>());
            persona.Resistances[skillType] = value;
        }
    }

    private static Race GetRaceFromDictionary(GC.Dictionary<string, Variant> personaDictionary)
    {
        string personaRace = personaDictionary[_RaceKey].As<string>();
        try
        {
            Enum.Parse<Race>(personaRace);
            return Enum.Parse<Race>(personaDictionary[_RaceKey].As<string>());
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
        string personaSkillType = personaDictionary[_SkillTypeKey].As<string>();
        try
        {
            Enum.Parse<SkillType>(personaSkillType);
            return Enum.Parse<SkillType>(personaDictionary[_SkillTypeKey].As<string>());
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
        int randomNumber = RNG.RandiRange(0, Personas.Count - 1);
        return Personas[randomNumber];
    }

    public static List<Persona> GetPersonasByRace(Race race)
    {
        return Personas.FindAll(persona => persona.Race == race);
    }

    public static List<Persona> GetPersonasBySkillType(SkillType skillType)
    {
        return Personas.FindAll(persona => persona.SkillType == skillType);
    }

    public static Persona GetPersonaByName(string personaName)
    {
        return Personas.Find(persona => persona.Name == personaName);
    }
}
