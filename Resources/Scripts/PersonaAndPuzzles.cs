using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using GC = Godot.Collections;

namespace PersonaAndPuzzles;

public enum EnhancementMaterial
{
	Memory,
	Anecdote,
	Legend
}

public partial class PersonaAndPuzzles : Node
{
	[Export]
	private bool _getRandomPersonas = false;
	
	[Export(PropertyHint.Range, "0,25,1")]
	private int _startingPersonaCount = 0;

	[Export(PropertyHint.Range, "0,3,1")]
	private int _personaCloneCount = 0;

	[Export(PropertyHint.Range, "0,9,1")]
	private int _extraLoadouts = 0;

	[Export]
	private bool _overridePersonaLevel = false;

	[Export(PropertyHint.Range, "1,99,1")]
	private int _personaLevel = 1;

	// Max Period Count: 4
	private static string[] _Notation = ["", "K", "M", "B"];

	public static int StartingPersonaCount;
	public static int PersonaCloneCount;
	public static int ExtraLoadouts;
	public static bool OverridePersonaLevel;
	public static int PersonaLevel;

    public override void _Ready()
    {
        StartingPersonaCount = _startingPersonaCount;
		PersonaCloneCount = _personaCloneCount;
		ExtraLoadouts = _extraLoadouts;
		OverridePersonaLevel = _overridePersonaLevel;
		PersonaLevel = _personaLevel;

		if (_getRandomPersonas)
		{
			PersonaManager.Instance.AddRandomPersonas();
			PersonaManager.Instance.SetRandomLoadout();
		}
    }

    public static Variant LoadFile(string fileName, string folderName = "")
	{
		folderName = folderName != "" ? $"{folderName}/" : "";
		string filePath = $"res://JSON/{folderName}{fileName}.json";

		Json json = new Json();

		try
		{
			using FileAccess fileAccess = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
			string jsonString = fileAccess.GetAsText();

			if (json.Parse(jsonString) != Error.Ok) throw new Exception($"Couldn't Parse {fileName}");

			PrintRich.PrintJSONSuccess(fileName);

			GC.Dictionary<string, Variant> dictionaries = json.Data.As<GC.Dictionary<string, Variant>>();
			return dictionaries;
		}
		catch (Exception exception)
		{
			GD.PrintErr(exception);
            GD.Print();
			
			string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
			string message = $"Couldn't Parse File - {fileName}";
			string result = "Returning Default/Null";
			PrintRich.PrintError(className, message, result);

			return default;
		}
	}

	public static string GetNotation(int number)
	{
		List<string> periods = GetPeriods(number);

		if (periods.Count < 2) return $"{number}";

		List<char> charactersToAssemble = new List<char>();

		char[] firstPeriod = periods[0].ToCharArray();
		for (int i = 0; i < firstPeriod.Length; i++)
		{
			charactersToAssemble.Add(firstPeriod[i]);
		}

		charactersToAssemble.Add('.');

		char[] secondPeriod = periods[1].ToCharArray();
		for (int i = 0; i < 2 /* Get only two digits */; i++)
		{
			charactersToAssemble.Add(secondPeriod[i]);
		}

		string formattedNumber = string.Concat(charactersToAssemble);
		string notation = _Notation[periods.Count - 1];

		return $"{formattedNumber} {notation}";
	}

	// Periods are groups of three digits seperated by commas
	private static List<string> GetPeriods(int number)
	{
		string numberString = $"{number:N0}"; // Format string for split
		List<string> periods = numberString.Split(",").ToList();
		return periods;
	}
}
