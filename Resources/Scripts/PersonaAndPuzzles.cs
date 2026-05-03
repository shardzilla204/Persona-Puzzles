using System;
using System.Reflection;
using Godot;
using GC = Godot.Collections;

namespace PersonaAndPuzzles;

public partial class PersonaAndPuzzles : Node
{
    public static PackedScenes PackedScenes;
    public static Signals Signals = new Signals();

    public override void _Ready()
    {
        PersonaManager.LoadPersonas();
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

			GC.Dictionary<string, Variant> dictionaries = (GC.Dictionary<string, Variant>) json.Data;
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
