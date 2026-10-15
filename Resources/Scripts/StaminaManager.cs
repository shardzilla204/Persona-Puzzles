using Godot;
using GC = Godot.Collections;
using System;
using System.Threading.Tasks;
using System.Numerics;

namespace PersonaAndPuzzles;

public partial class StaminaManager : Node
{
    private readonly static Lazy<StaminaManager> s_lazy = new Lazy<StaminaManager>(() => new StaminaManager());
    public static StaminaManager Instance => s_lazy.Value;

    private StaminaManager()
    {
        StartTimer();
    }

    // For every 5 minutes (300 seconds), get 1 stamina
    private const int _StaminaTimeRatio = 300;
    private const string _HourKey = "hour";
    private const string _MinuteKey = "minute";
    private const string _SecondKey = "second";

    private int _secondsRemainder;
    private GC.Dictionary<string, int> _lastPlayedData = new GC.Dictionary<string, int>();

    public async void StartTimer()
    {
        while (!IsStaminaFull())
        {
            await Task.Delay(1000);
            _secondsRemainder--;

            if (_secondsRemainder <= 0)
            {
                _secondsRemainder = _StaminaTimeRatio;
                AddStamina();
            }
        }
    }

    private void AddStamina()
    {
        if (IsStaminaFull()) return;

        Player.Instance.Stamina++;
    }

    public (int, int) GetTime()
    {
        int minutes = _secondsRemainder / 60;
        int seconds = _secondsRemainder % 60;
        return (seconds, minutes);
    }

    public string GetTimeFormatted()
    {
        (int Seconds, int Minutes) time = GetTime();
        return $"{time.Minutes}:{time.Seconds:00}";
    }

    public void GetStamina()
    {
        if (IsStaminaFull()) return;

        GC.Dictionary<string, int> timeDifference = GetTimeDifference();
        BigInteger seconds = GetSeconds(timeDifference);
        int stamina = (int) (seconds / _StaminaTimeRatio);

        PrintRich.PrintStamina(stamina, timeDifference);

        Player.Instance.Stamina = Mathf.Min(Player.Instance.Stamina + stamina, Player.Instance.MaxStamina);
    }

    private int GetSecondsRemainder()
    {
        GC.Dictionary<string, int> timeDifference = GetTimeDifference();
        BigInteger seconds = GetSeconds(timeDifference);
        return (int) (seconds % _StaminaTimeRatio);
    }

    // Time Keys: (year, month, day, weekday, hour, minute, second, dst)
    public GC.Dictionary<string, int> GetTimeDifference()
    {
        return new GC.Dictionary<string, int>()
        {
            { _HourKey, CalculateTimeDifference(_HourKey) },
            { _MinuteKey, CalculateTimeDifference(_MinuteKey) },
            { _SecondKey, CalculateTimeDifference(_SecondKey) }
        };
    }

    private int CalculateTimeDifference(string keyName)
    {
        GC.Dictionary currentTime = Time.GetDatetimeDictFromSystem();

        int current = currentTime[keyName].As<int>();
        int previous = _lastPlayedData[keyName];

        int timeDifference = current - previous < 0 ? current : current - previous;
        return timeDifference;
    }

    private BigInteger GetSeconds(GC.Dictionary<string, int> timeDifference)
    {
        // 1 hour = 60 minutes
        int hours = timeDifference[_HourKey];
        int minutes = timeDifference[_MinuteKey];
        int totalMinutes = (hours * 60) + minutes;

        // 1 minute = 60 seconds
        int seconds = timeDifference[_SecondKey];
        int totalSeconds = (totalMinutes * 60) + seconds;

        return totalSeconds;
    }

    public async Task SetLastPlayedData(GC.Dictionary<string, int> lastPlayedData)
    {
        _lastPlayedData = lastPlayedData;
        _secondsRemainder = GetSecondsRemainder();

        string message = "Stamina Data Successfully Loaded";
        PrintRich.PrintLine(message, TextColor.Purple);
    }

    public bool IsStaminaFull()
    {
        return Player.Instance.Stamina >= Player.Instance.MaxStamina;
    }
}
