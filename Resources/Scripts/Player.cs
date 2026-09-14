using Godot;
using Godot.Collections;
using System;

namespace PersonaAndPuzzles;

public partial class Player : Node
{
    public static int Stamina = 100;
    private static int _MaxStamina = 100;

    public static int Level = 1;
    public static int Experience = 0;

    private static int _MaxExperience = 100;

    private static Dictionary<string, int> _Materials = new Dictionary<string, int>()
    {
        { $"{EnhancementMaterial.Memory}", 100 },
        { $"{EnhancementMaterial.Anecdote}", 100 },
        { $"{EnhancementMaterial.Legend}", 100 }
    };

    public static bool CanLevelUp()
    {
        return Experience >= _MaxExperience;
    }

    public static void LevelUp()
    {
        const int LinearGrowth = 10;
        const float ExponentialGrowth = 0.1f;

        int levels = 0;
        while (Experience >= _MaxExperience)
        {
            levels++;
            Experience -= _MaxExperience;

            _MaxExperience += LinearGrowth;
            _MaxExperience = Mathf.RoundToInt(_MaxExperience * ExponentialGrowth);
        }

        Level += levels;
    }

    public static void Reset()
    {
        Stamina = 100;
        Level = 1;
        Experience = 0;
    }

    public static void GetStamina()
    {
        // For 5 minutes (300 seconds), get 1 stamina
        const int StaminaTimeRatio = 300;
        if (Stamina >= _MaxStamina) return;

        Dictionary<string, Variant> offlineData = DataManager.GetData()[DataManager.LastPlayedKey].As<Dictionary<string, Variant>>();
        Dictionary<string, int> timeDifference = GetTimeDifference(offlineData);
    }

    // Time Keys: (year, month, day, weekday, hour, minute, second, dst)
    public static Dictionary<string, int> GetTimeDifference(Dictionary<string, Variant> offlineData)
    {
        return new Dictionary<string, int>()
        {
            { "hour", CalculateTimeDifference(offlineData, "hour") },
            { "minute", CalculateTimeDifference(offlineData, "minute") },
            { "second", CalculateTimeDifference(offlineData, "second") },
        };
    }

    private static int CalculateTimeDifference(Dictionary<string, Variant> offlineData, string keyName)
    {
        const string TimeKey = "Time";

        Dictionary currentTime = Time.GetDatetimeDictFromSystem();
        Dictionary previousTime = offlineData[TimeKey].As<Dictionary>();

        int current = currentTime[keyName].As<int>();
        int previous = previousTime[keyName].As<int>();

        int timeDifference = current - previous < 0 ? current : current - previous;
        return timeDifference;
    }

    public static int GetMaterialCount(string materialName)
    {
        return _Materials[materialName];
    }

    public static void SetMaterialCount(string materialName, int count)
    {
        _Materials[materialName] = count;
    }
}
