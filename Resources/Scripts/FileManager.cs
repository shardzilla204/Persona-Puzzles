using Godot;
using Godot.Collections;
using System;
using System.Reflection;

namespace PersonaAndPuzzles;

public partial class FileManager : Node
{
    private static readonly Lazy<FileManager> s_lazy = new Lazy<FileManager>(() => new FileManager());
    public static FileManager Instance => s_lazy.Value;

    private FileManager() {}

    public const string PlayerKey = "Player";
    public const string PersonaKey = "Persona";
    public const string OfflineKey = "Offline";

    private string _filePath = "user://game.save";

    private Dictionary<string, Variant> _data = new Dictionary<string, Variant>();

    public override void _EnterTree()
    {
        GetWindow().CloseRequested += SaveGame;
    }

    public override void _Ready()
    {
        if (FileAccess.FileExists(_filePath))
        {
            LoadGame();
        }
        else
        {
            SaveGame();
        }
    }

    private void SaveGame()
    {
        using FileAccess gameFile = FileAccess.Open(_filePath, FileAccess.ModeFlags.Write);
        string jsonString = Json.Stringify(GetData(), "\t");

        if (jsonString == "") return;

        gameFile.StoreLine(jsonString);

        PrintRich.PrintFileSuccess("Game File", gameFile, "Saved");
    }

    private void LoadGame()
    {
        using FileAccess gameFile = FileAccess.Open(_filePath, FileAccess.ModeFlags.Read);
        string jsonString = gameFile.GetAsText();

        if (gameFile.GetLength() == 0)
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = $"Game File Is Empty";
            PrintRich.PrintError(className, message);

            return;
        }

        Json json = new Json();

        Error result = json.Parse(jsonString);

        if (result != Error.Ok) return;

        _data = new Dictionary<string, Variant>((Dictionary) json.Data);

        PrintRich.PrintFileSuccess("Game File", gameFile, "Loaded");
    }

    public void DeleteGame()
    {
        Player.Instance.Reset();
        // PersonaManager.Reset

        SaveGame();
        LoadGame();
    }

    public Dictionary<string, Variant> GetData(string keyName)
    {
        return _data[keyName].As<Dictionary<string, Variant>>();
    }

    private Dictionary<string, Variant> GetData()
    {
        return new Dictionary<string, Variant>()
        {
            { PlayerKey, GetPlayerData() },
            { PersonaKey, GetPersonaData() },
            { OfflineKey, Time.GetDatetimeDictFromSystem() }
        };
    }

    private Dictionary<string, Variant> GetPlayerData()
    {
        return new Dictionary<string, Variant>()
        {
            { "Name", Player.Instance.Name },
            { "Stamina", Player.Instance.Stamina },
            { "MaxStamina", Player.Instance.MaxStamina },
            { "Level", Player.Instance.Level },
            { "Experience", Player.Instance.Experience },
            { "MaxExperience", Player.Instance.MaxExperience },
        };
    }

    private Array<Dictionary<string, Variant>> GetPersonaData()
    {
        Array<Dictionary<string, Variant>> personaData = new Array<Dictionary<string, Variant>>();
        foreach (Persona persona in PersonaManager.Instance.Personas)
        {
            Dictionary<string, Variant> personaDictionary = new Dictionary<string, Variant>()
            {
                { "ID", persona.ID },
                { "Name", persona.Name },
                { "Level", persona.Level },
                { "Experience", persona.Experience },
                { "Overclock", persona.Overclock },
                { "IsFavorite", persona.IsFavorite }
            };
            personaData.Add(personaDictionary);
        }
        return personaData;
    }
}
