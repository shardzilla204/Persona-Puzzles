using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace PersonaAndPuzzles;

public partial class PersonaManager : Node
{
    private readonly static Lazy<PersonaManager> s_lazy = new Lazy<PersonaManager>(() => new PersonaManager());
    public static PersonaManager Instance => s_lazy.Value;

    private PersonaManager()
    {
        LoadPersonas();
        AddEmptyLoadouts();
    }

    public const int MaxStat = 99;
    public const int MaxPassiveSkills = 3;
    public const int MaxTeamSlots = 6;
    public const int MaxTeamReserves = 3;
    public const int MaxLoadouts = 10;

    private const string _NameKey = "Name";
    private const string _IDKey = "ID";
    private const string _LevelKey = "Level";
    private const string _OverclockKey = "Overclock";
    private const string _ExperienceKey = "Experience";
    private const string _IsFavoriteKey = "IsFavorite";
    private const string _PersonasKey = "Personas";
    private const string _LoadoutIndexKey = "Index";

    private List<Persona> _Personas = new List<Persona>();
    public List<Persona> Personas = new List<Persona>();

    public PersonaLoadout Loadout = new PersonaLoadout("Loadout 1", 0);
    public List<PersonaLoadout> Loadouts = new List<PersonaLoadout>();
    public int LoadoutIndex = 0;

    public int MaxPersonas = 50;

    public string SkillTypeFilter = $"{SkillTypeFilterContainer.Filter.All}";
    public string FavoriteFilter = $"{FavoriteFilterContainer.Filter.All}";
    public string StatTypeFilter = $"{StatTypeFilterContainer.Filter.All}";

    public void AddRandomPersonas()
    {
        string message = "Adding Personas...";
        PrintRich.PrintLine(message, TextColor.Blue);

        for (int i = 0; i < PersonaAndPuzzles.StartingPersonaCount; i++)
        {
            Persona persona = GetRandomPersona();
            Personas.Add(persona);

            for (int j = 0; j < PersonaAndPuzzles.PersonaCloneCount; j++)
            {
                Persona personaClone = ClonePersona(persona);
                Personas.Add(personaClone);
            }
        }
    }

    public void SetRandomLoadout()
    {
        AddRandomLoadouts();

        Loadout = CloneLoadout(0);
    }

    private void AddRandomLoadouts()
    {
        string message = "Adding Loadouts...";
        PrintRich.PrintLine(message, TextColor.Blue);

        for (int i = 0; i <= PersonaAndPuzzles.ExtraLoadouts; i++)
        {
            PersonaLoadout loadout = Loadouts[i];
            string name = $"Loadout {i + 1}";
            loadout.Name = name;

            if (Personas.Count < MaxTeamSlots)
            {
                for (int j = 0; j < Personas.Count; j++)
                {
                    Persona persona = Personas[j];
                    loadout.Personas[j] = persona;
                }
                break;
            }
            else
            {
                // Add the Personas and make sure the same Persona isn't added in
                int maxRange = Personas.Count - 1;
                for (int j = 0; j < MaxTeamSlots; j++)
                {
                    RandomNumberGenerator RNG = new RandomNumberGenerator();
                    int randomNumber = RNG.RandiRange(0, maxRange);
                    Persona persona = Personas[randomNumber];
                    while (loadout.Personas.Contains(persona))
                    {
                        randomNumber = RNG.RandiRange(0, maxRange);
                        persona = Personas[randomNumber];
                    }
                    loadout.Personas[j] = persona;
                }
            }
        }
    }

    public void AddEmptyLoadouts()
    {
        for (int i = Loadouts.Count; i < MaxLoadouts; i++)
        {
            string name = $"Loadout {i + 1}";
            PersonaLoadout Loadout = new PersonaLoadout(name, i);
            for (int j = 0; j < MaxTeamSlots; j++)
            {
                Loadout.Personas.Add(null);
            }
            Loadouts.Add(Loadout);
        }
    }

    public void SetLoadout(Persona newPersona, dynamic oldPersona)
    {
        Loadout.Swap(newPersona, oldPersona);

        PersonaLoadout targetLoadout = Loadouts.Find(loadout => loadout.Name == Loadout.Name);
        targetLoadout.Swap(newPersona, oldPersona);
    }

    private void LoadPersonas()
    {
        const string FileName = "Persona";

        // Keys
        const string NameKey = "Name";
        const string ImageKey = "Image";
        const string ResistancesKey = "Resistances";
        const string PassiveSkillsKey = "PassiveSkills";
        const string LevelKey = "Level";
        const string RankKey = "Rank";

        // Stat keys
        const string StrengthKey = "Strength";
        const string MagicKey = "Magic";
        const string EnduranceKey = "Endurance";
        const string AgilityKey = "Agility";
        const string LuckKey = "Luck";

        GC.Dictionary<string, Variant> personaData = PersonaAndPuzzles.LoadFile(FileName).As<GC.Dictionary<string, Variant>>();
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
                Rank = personaDictionary[RankKey].As<int>(),

                // Stats
                Strength = personaDictionary[StrengthKey].As<int>(),
                Magic = personaDictionary[MagicKey].As<int>(),
                Endurance = personaDictionary[EnduranceKey].As<int>(),
                Agility = personaDictionary[AgilityKey].As<int>(),
                Luck = personaDictionary[LuckKey].As<int>(),
            };
            GC.Dictionary<string, Variant> resistancesDictionary = personaDictionary[ResistancesKey].As<GC.Dictionary<string, Variant>>();
            SetResistances(persona, resistancesDictionary);

            GC.Array passiveSkills = personaDictionary[PassiveSkillsKey].As<GC.Array>();
            SetPassiveSkills(persona, passiveSkills);

            _Personas.Add(persona);
        }
    }

    public int GetOverclockCount(int rank) => rank switch
    {
        >= 4 and <= 6 => 2,
        >= 7 and <= 8 => 3,
        (>= 1 and <= 3) or _ => 1
    };

    public int GetBonusLevels(Persona persona)
    {
        int bonusLevels = 0;
        for (int i = 0; i < persona.Overclock; i++)
        {
            bonusLevels += 5;
        }
        if (persona.Overclock >= GetOverclockCount(persona.Rank))
        {
            bonusLevels += persona.Rank == 8 ? 4 : 0;
        }
        return bonusLevels;
    }

    public int GetBaseMaxLevel(int rank) => rank switch
    {
        2 => 30,
        3 => 40,
        4 => 50,
        5 => 60,
        6 => 70,
        7 or 8 => 80,
        1 or _ => 20
    };

    public int GetMaxLevel(Persona persona)
    {
        return GetBaseMaxLevel(persona.Rank) + GetBonusLevels(persona);
    }

    public int GetMaterialExperience(EnhancementMaterial material) => material switch
    {
        EnhancementMaterial.Memory => 100,
        EnhancementMaterial.Anecdote => 500,
        EnhancementMaterial.Legend => 2000,
        _ => 0
    };

    public int GetMaterialCost(EnhancementMaterial material) => material switch
    {
        EnhancementMaterial.Memory => 25,
        EnhancementMaterial.Anecdote => 150,
        EnhancementMaterial.Legend => 500,
        _ => 0  
    };

    public int GetOverclockCost(int rank) => rank switch
    {
        1 => 1000,
        2 => 1500,
        3 or 4 => 2000,
        5 or 6 => 4000,
        7 => 6000,
        8 => 10000,
        _ => 0
    };

    private void SetResistances(Persona persona, GC.Dictionary<string, Variant> resistances)
    {
        foreach (string key in resistances.Keys)
        {
            SkillType skillType = Enum.Parse<SkillType>(key);
            Resistance resistance = Enum.Parse<Resistance>(resistances[key].As<string>());
            persona.Resistances[skillType] = resistance;
        }
    }

    private void SetPassiveSkills(Persona persona, GC.Array passiveSkills)
    {
        foreach (string passiveSkillName in passiveSkills)
        {
            PassiveSkill passiveSkill = PassiveSkillManager.Instance.FindPassiveSkill(passiveSkillName);
            persona.PassiveSkills.Add(passiveSkill);
        }
    }

    private Race GetRaceFromDictionary(GC.Dictionary<string, Variant> personaDictionary)
    {
        const string RaceKey = "Race";
        string personaRace = personaDictionary[RaceKey].As<string>();
        try
        {
            Enum.Parse<Race>(personaRace);
            return Enum.Parse<Race>(personaDictionary[RaceKey].As<string>());
        }
        catch (Exception exception)
        {
            GD.PrintErr(exception);
            GD.Print();

            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
			string message = $"Couldn't Parse Race - {personaRace}";
			string result = "Returning Race.Fool";
			PrintRich.PrintError(className, message, result);
            
            return Race.Fool;
        }
    }

    private SkillType GetSkillTypeFromDictionary(GC.Dictionary<string, Variant> personaDictionary)
    {
        const string SkillTypeKey = "SkillType";
        string personaSkillType = personaDictionary[SkillTypeKey].As<string>();
        try
        {
            Enum.Parse<SkillType>(personaSkillType);
            return Enum.Parse<SkillType>(personaDictionary[SkillTypeKey].As<string>());
        }
        catch (Exception exception)
        {
            GD.PrintErr(exception);
            GD.Print();

            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
			string message = $"Couldn't Parse SkillType - {personaSkillType}";
			string result = "Returning SkillType.None";
			PrintRich.PrintError(className, message, result);
            
            return SkillType.None;
        }
    }

    public Texture2D GetPersonaImage(Persona persona)
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

    public Persona GetRandomPersona()
    {
        RandomNumberGenerator RNG = new RandomNumberGenerator();
        int randomNumber = RNG.RandiRange(0, _Personas.Count - 1);
        Persona persona = _Personas[randomNumber];
        Persona personaClone = ClonePersona(persona);

        int targetLevel = PersonaAndPuzzles.OverridePersonaLevel ? PersonaAndPuzzles.PersonaLevel : 1;
        SetLevel(personaClone, targetLevel);

        return personaClone;
    }

    private Persona ClonePersona(Persona persona)
    {
        if (persona == null) 
        {
            GD.PrintErr("Couldn't Clone Persona | Returning null");
            return null;
        }

        Persona personaClone = new Persona(persona)
        {
            ID = $"{Guid.NewGuid():N}"
        };
        return personaClone;
    }

    public List<Persona> FindPersonasByRace(Race race)
    {
        return _Personas.FindAll(persona => persona.Race == race);
    }

    public List<Persona> FindPersonasBySkillType(SkillType skillType)
    {
        return _Personas.FindAll(persona => persona.SkillType == skillType);
    }

    public Persona FindPersonaByName(string personaName)
    {
        return _Personas.Find(persona => persona.Name == personaName);
    }

    public Persona FindPersonaByID(string personaID)
    {
        return Personas.Find(persona => persona.ID == personaID);
    }

    public void SetLevel(Persona persona, int targetLevel)
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

    public void RemovePersona(Persona targetPersona)
    {
        Personas.Remove(targetPersona);

        int personaIndex = Loadout.Personas.FindIndex(persona => persona == targetPersona);
        if (personaIndex != -1)
        {
            Loadout.Personas[personaIndex] = null;
        }

        foreach (PersonaLoadout loadout in Loadouts)
        {
            personaIndex = Loadout.Personas.FindIndex(persona => persona == targetPersona);
            if (personaIndex != -1)
            {
                Loadout.Personas[personaIndex] = null;
            }
        }

        Signals.Instance.EmitSignal(Signals.SignalName.PersonaRemoved);
    }

    public int GetLevels(Persona persona, int experience)
    {
        int levels = 0;
        int maxExperience = GetMaxExperience(persona.Level);
        while (experience >= maxExperience)
        {
            levels++;
            experience -= maxExperience;
            maxExperience = GetMaxExperience(persona.Level + levels);
        }
        return levels;
    }

    public int GetMaxExperience(int level)
    {
        int maxExperience = 0;
        const int LinearGrowth = 100;
        const float ExponentialGrowth = 0.05f;
        for (int i = 0; i < level; i++)
        {
            maxExperience += LinearGrowth;
            maxExperience += Mathf.RoundToInt(maxExperience * ExponentialGrowth);
        }
        return maxExperience;
    }

    public bool IsOverPersonaCount()
    {
        return Personas.Count > MaxPersonas;
    }

    public bool IsLoadoutEmpty()
    {
        foreach (Persona persona in Loadout.Personas)
        {
            if (persona != null) return false;
        }
        return true;
    }

    public GC.Array<Variant> GetPersonasData()
    {
        string loadMessage = $"Saving {Personas.Count} Personas...";
        PrintRich.PrintLine(loadMessage, TextColor.Blue);

        GC.Array<Variant> personasData = new GC.Array<Variant>();
        foreach (Persona persona in Personas)
        {
            GC.Dictionary<string, Variant> personaData = new GC.Dictionary<string, Variant>()
            {
                { _NameKey, persona.Name },
                { _IDKey, persona.ID },
                { _LevelKey, persona.Level },
                { _ExperienceKey, persona.Experience },
                { _OverclockKey, persona.Overclock },
                { _IsFavoriteKey, persona.IsFavorite }
            };
            personasData.Add(personaData);
        }
        return personasData;
    }

    public GC.Array<Variant> GetLoadoutsData()
    {
        GC.Array<Variant> loadoutsData = new GC.Array<Variant>();
        for (int i = 0; i < Loadouts.Count; i++)
        {
            PersonaLoadout loadout = Loadouts[i];
            GC.Dictionary<string, Variant> loadoutData = new GC.Dictionary<string, Variant>()
            {
                { _NameKey, loadout.Name },
                { _LoadoutIndexKey, loadout.Index },
                { _PersonasKey, GetPersonasData(loadout) }
            };
            loadoutsData.Add(loadoutData);
        }
        return loadoutsData;
    }

    private GC.Array<string> GetPersonasData(PersonaLoadout loadout)
    {
        GC.Array<string> personasData = new GC.Array<string>();
        foreach (Persona persona in loadout.Personas)
        {
            string id = persona == null ? "" : persona.ID;
            personasData.Add(id);
        }
        return personasData;
    }

    public async Task SetPersonasData(GC.Array<Variant> personasData)
    {
        try
        {
            string loadMessage = $"Loading {personasData.Count} Personas...";
            PrintRich.PrintLine(loadMessage, TextColor.Blue);

            foreach (GC.Dictionary<string, Variant> personaData in personasData)
            {
                string personaName = personaData[_NameKey].As<string>();
                Persona persona = FindPersonaByName(personaName);

                // Create a clone to prevent mutation
                Persona personaClone = new Persona(persona);

                string id = personaData[_IDKey].As<string>();
                personaClone.ID = id;

                int experience = personaData[_ExperienceKey].As<int>();
                personaClone.Experience = experience;

                int level = personaData[_LevelKey].As<int>();
                SetLevel(personaClone, level);

                int overclock = personaData[_OverclockKey].As<int>();
                personaClone.Overclock = overclock;

                bool isFavorite = personaData[_IsFavoriteKey].As<bool>();
                personaClone.IsFavorite = isFavorite;

                Personas.Add(personaClone);
            }

            string message = "Personas Data Successfully Loaded";
            PrintRich.PrintLine(message, TextColor.Purple);
        }
        catch (Exception exception)
        {
            GD.PrintErr(exception);
            GD.Print();

            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = "Personas Data Couldn't Be Loaded";
            PrintRich.PrintError(className, message);
        }
    }

    public async Task SetLoadoutsData(int loadoutIndex, GC.Array<Variant> loadoutsData)
    {
        try
        { 
            LoadoutIndex = loadoutIndex;

            for (int i = 0; i < loadoutsData.Count; i++)
            {
                GC.Dictionary<string, Variant> loadoutData = loadoutsData[i].As<GC.Dictionary<string, Variant>>();
                string name = loadoutData[_NameKey].As<string>();
                int index = loadoutData[_LoadoutIndexKey].As<int>();
                PersonaLoadout loadout = Loadouts[i];
                loadout.Name = name;
                loadout.Index = index;
                
                GC.Array<string> personaIDs = loadoutData[_PersonasKey].As<GC.Array<string>>();
                for (int j = 0; j < personaIDs.Count; j++)
                {
                    string personaID = personaIDs[j];
                    Persona persona = FindPersonaByID(personaID);
                    loadout.Personas[j] = persona;
                }
            }

            Loadout = CloneLoadout(LoadoutIndex);

            string message = "Loadouts Data Successfully Loaded";
            PrintRich.PrintLine(message, TextColor.Purple);
        }
        catch (Exception exception)
        {
            GD.PrintErr(exception);
            GD.Print();

            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = "Loadouts Data Couldn't Be Loaded";
            PrintRich.PrintError(className, message);
        }
    }

    private PersonaLoadout CloneLoadout(int index)
    {
        return new PersonaLoadout(Loadouts[index]);
    }
}

