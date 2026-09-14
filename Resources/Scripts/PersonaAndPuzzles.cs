using System;
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
	[Export(PropertyHint.Range, "0,25,1")]
	private int _startingPersonaCount = 0;

	[Export(PropertyHint.Range, "0,9,1")]
	private int _extraLoadouts = 0;

	[Export]
	private bool _overridePersonaLevel = false;

	public static int StartingPersonaCount = 0;
	public static int ExtraLoadouts = 1;
	public static bool OverridePersonaLevel = false;

    public static Signals Signals = new Signals();

    public override void _Ready()
    {
		StartingPersonaCount = _startingPersonaCount;
		ExtraLoadouts = _extraLoadouts;
		OverridePersonaLevel = _overridePersonaLevel;
		
		PassiveSkillManager.LoadPassiveSkills();
        PersonaManager.LoadPersonas();
        PersonaManager.GetRandomPersonas();
		PersonaManager.SetRandomLoadout();
		VelvetTrialManager.LoadVelvetTrials();
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
		catch
		{
			string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
			string message = $"Couldn't Parse File - {fileName}";
			string result = "Returning Default/Null";
			PrintRich.PrintError(className, message, result);

			return default;
		}
	}
}
