using Godot;
using GC = Godot.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace PersonaAndPuzzles;

public partial class DataManager : Node
{
	[Signal]
	public delegate void DataLoadedEventHandler();

	private readonly static Lazy<DataManager> s_lazy = new Lazy<DataManager>(() => new DataManager());
    public static DataManager Instance => s_lazy.Value;

	private DataManager(){}

    private string _gameFilePath = "user://savegame.sav";

	private const string _PlayerKey = "Player";
	private const string _PersonasKey = "Personas";
	private const string _LoadoutIndexKey = "LoadoutIndex";
	private const string _LoadoutsKey = "Loadouts";

    public const string LastPlayedKey = "LastPlayed";

    public override void _Ready()
    {
		GetWindow().CloseRequested += SaveGame;

        if (FileAccess.FileExists(_gameFilePath))
		{
			LoadGame();
		}
		else
		{
			SaveGame();
		}

		SaveInterval();
    }

    public void SaveGame()
	{
		using FileAccess gameFile = FileAccess.Open(_gameFilePath, FileAccess.ModeFlags.Write);
		string jsonString = Json.Stringify(GetData(), "\t");

		if (jsonString == "") return;

		gameFile.StoreLine(jsonString);

		string name = "Game File";
		string stateString = "Saved";
		PrintRich.PrintFileSuccess(name, gameFile, stateString);
	}

	private void LoadGame()
	{
		using FileAccess gameFile = FileAccess.Open(_gameFilePath, FileAccess.ModeFlags.Read);
		string jsonString = gameFile.GetAsText();

		if (gameFile.GetLength() == 0)
		{
			string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
			string message = $"{_gameFilePath} Is Empty | Saving Game";
			PrintRich.PrintError(className, message);
			
			SaveGame();
			return;
		}

		Json json = new Json();

		Error result = json.Parse(jsonString);

		if (result != Error.Ok) return;

		GC.Dictionary<string, Variant> gameData = new GC.Dictionary<string, Variant>((GC.Dictionary) json.Data);
		SetData(gameData);

		string name = "Game File";
		string stateString = "Loaded";
		PrintRich.PrintFileSuccess(name, gameFile, stateString);
	}

	private void DeleteGame()
	{
		SaveGame();
		LoadGame();
	}
	
	private async void SaveInterval()
	{
		const float TimeSeconds = 60f;
		while (true)
		{
			await ToSignal(GetTree().CreateTimer(TimeSeconds), SceneTreeTimer.SignalName.Timeout);
			SaveGame();
		}
	}

    public GC.Dictionary<string, Variant> GetData()
	{
		return new GC.Dictionary<string, Variant>()
        {
			{ _PlayerKey, Player.Instance.GetData() },
			{ _PersonasKey, PersonaManager.Instance.GetPersonasData() },
			{ _LoadoutIndexKey, PersonaManager.Instance.LoadoutIndex },
			{ _LoadoutsKey, PersonaManager.Instance.GetLoadoutsData() },
            { LastPlayedKey, Time.GetDatetimeDictFromSystem() }
        };
    }
	
    private async void SetData(GC.Dictionary<string, Variant> gameData)
	{
		try
		{
            GC.Dictionary<string, Variant> playerData = gameData[_PlayerKey].As<GC.Dictionary<string, Variant>>();
			await Player.Instance.SetData(playerData);

			GC.Array<Variant> personasData = gameData[_PersonasKey].As<GC.Array<Variant>>();
			await PersonaManager.Instance.SetPersonasData(personasData);

			int loadoutIndex = gameData[_LoadoutIndexKey].As<int>();
			GC.Array<Variant> loadoutsData = gameData[_LoadoutsKey].As<GC.Array<Variant>>();
			await PersonaManager.Instance.SetLoadoutsData(loadoutIndex, loadoutsData);

			GC.Dictionary<string, int> lastPlayedData = gameData[LastPlayedKey].As<GC.Dictionary<string, int>>();
			await StaminaManager.Instance.SetLastPlayedData(lastPlayedData);

			EmitSignal(SignalName.DataLoaded);
		}
		catch (KeyNotFoundException exception)
		{
			GD.PrintErr(exception);
			GD.Print();
			
			SaveGame();
		}
	}
}
