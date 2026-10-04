using System;
using Godot;
using Godot.Collections;

namespace PersonaAndPuzzles;

public partial class OfflineProgression : Node
{
    private static readonly Lazy<OfflineProgression> s_lazy = new Lazy<OfflineProgression>(() => new OfflineProgression());
    public static OfflineProgression Instance { get { return s_lazy.Value; } }

    private OfflineProgression() {}

    public Dictionary<string, int> TimeDifference = new Dictionary<string, int>();
    private Dictionary<string, Variant> _timeDictionary = new Dictionary<string, Variant>();

    public override void _Ready()
    {
        _timeDictionary = FileManager.Instance.GetData(FileManager.OfflineKey);
    }

    // Keys:
    /*
        year
        month
        day
        weekday
        hour
        minute
        second
        dst
    */
    private void CalculateTimeDifference()
    {
        const string HourKey = "hour";
        const string MinuteKey = "minute";
        const string SecondKey = "second";

        TimeDifference = new Dictionary<string, int>()
        {
            { HourKey, GetTimeDifference(HourKey) },
            { MinuteKey, GetTimeDifference(MinuteKey) },
            { SecondKey, GetTimeDifference(SecondKey) },
        };
        // PrintRich.PrintTimeDifference(TimeDifference);
    }

    private int GetTimeDifference(string keyName)
    {
        Dictionary currentTime = Time.GetDatetimeDictFromSystem();

        int current = currentTime[keyName].As<int>();
        int previous = _timeDictionary[keyName].As<int>();

        int timeDifference = current - previous < 0 ? current : current - previous;
        return timeDifference;
    }
}