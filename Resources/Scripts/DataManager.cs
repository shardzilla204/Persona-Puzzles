using Godot;
using GC = Godot.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace PersonaAndPuzzles;

public partial class DataManager : Node
{
    private static string _gameFilePath = "user://savegame.sav";
    public const string LastPlayedKey = "Last Played";

    public override void _Ready()
    {
        if (FileAccess.FileExists(_gameFilePath))
		{
			LoadGame();
		}
		else
		{
			SaveGame();
		}

		GetWindow().CloseRequested += SaveGame;
		SaveInterval();
    }

    private void SaveGame()
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
			string message = $"{_gameFilePath} Is Empty";
			PrintRich.PrintError(className, message);
			
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

    public static GC.Dictionary<string, Variant> GetData()
	{
		return new GC.Dictionary<string, Variant>()
        {
            { LastPlayedKey, Time.GetDatetimeDictFromSystem() }
        };
    }
	
    private void SetData(GC.Dictionary<string, Variant> gameData)
	{
		try
		{
            
		}
		catch (KeyNotFoundException)
		{
			SaveGame();
		}
	}
}
