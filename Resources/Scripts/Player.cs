using System;
using Godot;
using Godot.Collections;

namespace PersonaAndPuzzles;

public partial class Player : Node
{
    private static readonly Lazy<Player> s_lazy = new Lazy<Player>(() => new Player());
    public static Player Instance => s_lazy.Value;

    private Player() {}

    public new string Name = "Shardzilla";
    
    public int Stamina = 100;
    public int MaxStamina = 100;

    public int Level = 1;
    public int Experience = 0;
    public int MaxExperience = 100;

    public int KonpakuGems = 1000;

    private Dictionary<string, int> _Materials = new Dictionary<string, int>()
    {
        { $"{EnhancementMaterial.Memory}", 1000 },
        { $"{EnhancementMaterial.Anecdote}", 1000 },
        { $"{EnhancementMaterial.Legend}", 1000 }
    };

    public bool CanLevelUp()
    {
        return Experience >= MaxExperience;
    }

    public void LevelUp()
    {
        const int LinearGrowth = 10;
        const float ExponentialGrowth = 0.1f;

        int levels = 0;
        while (Experience >= MaxExperience)
        {
            levels++;
            Experience -= MaxExperience;

            MaxExperience += LinearGrowth;
            MaxExperience = Mathf.RoundToInt(MaxExperience * ExponentialGrowth);
        }

        Level += levels;
    }

    public void Reset()
    {
        Stamina = 100;
        Level = 1;
        Experience = 0;
    }

    public void GetStamina()
    {
        // For 5 minutes (300 seconds), get 1 stamina
        const int StaminaTimeRatio = 300;
        if (Stamina >= MaxStamina) return;

        Dictionary<string, Variant> offlineData = DataManager.GetData()[DataManager.LastPlayedKey].As<Dictionary<string, Variant>>();
        Dictionary<string, int> timeDifference = GetTimeDifference(offlineData);
    }

    // Time Keys: (year, month, day, weekday, hour, minute, second, dst)
    public Dictionary<string, int> GetTimeDifference(Dictionary<string, Variant> offlineData)
    {
        return new Dictionary<string, int>()
        {
            { "hour", CalculateTimeDifference(offlineData, "hour") },
            { "minute", CalculateTimeDifference(offlineData, "minute") },
            { "second", CalculateTimeDifference(offlineData, "second") },
        };
    }

    private int CalculateTimeDifference(Dictionary<string, Variant> offlineData, string keyName)
    {
        const string TimeKey = "Time";

        Dictionary currentTime = Time.GetDatetimeDictFromSystem();
        Dictionary previousTime = offlineData[TimeKey].As<Dictionary>();

        int current = currentTime[keyName].As<int>();
        int previous = previousTime[keyName].As<int>();

        int timeDifference = current - previous < 0 ? current : current - previous;
        return timeDifference;
    }

    public int GetMaterialCount(string materialName)
    {
        return _Materials[materialName];
    }

    public void SetMaterialCount(string materialName, int count)
    {
        _Materials[materialName] = Mathf.Max(0, count);
    }

    public string GetErrorMessage(VelvetTrial velvetTrial)
    {
        string errorMessage = "";
        if (velvetTrial != null)
        {
            if (Stamina < velvetTrial.Stamina)
            {
                errorMessage = "Not Enough Stamina";
            }
        }
        
        if (PersonaManager.Instance.IsOverPersonaCount())
        {
            errorMessage = "Too Many Personas In Compendium";
        }

        if (PersonaManager.Instance.IsLoadoutEmpty())
        {
            errorMessage = "At Least One Persona Is Needed";
        }

        return errorMessage;
    }

    public void ResetStats()
    {
        Level = 1;
        Experience = 0;
        MaxExperience = 100;
        MaxStamina = 100;
    }
}
