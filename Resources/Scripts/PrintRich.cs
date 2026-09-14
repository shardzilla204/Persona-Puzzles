using Godot;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PersonaAndPuzzles;

public enum TextColor
{
	Red, // In-Game Error
	Orange, // Action
	Yellow, // General Event
	Green, // In-Game Success
	Blue,
	Purple,
   	Brown,
}

public partial class PrintRich : Node
{
   [Export]
   private bool _isConsoleEnabled = true;

   [Export]
   private bool _areFileMessagesEnabled = false;

   [Export]
   private bool _areFilePathsVisible = false;

   public static bool IsConsoleEnabled;
   public static bool AreFileMessagesEnabled;
   public static bool AreFilePathsVisible;

    public override void _EnterTree()
    {
        IsConsoleEnabled = _isConsoleEnabled;
        AreFileMessagesEnabled = _areFileMessagesEnabled;
        AreFilePathsVisible = _areFilePathsVisible;
    }

    public static string GetColorHex(TextColor textColor) => textColor switch
	{
		TextColor.Red => "B80F0A",
		TextColor.Orange => "FC6B02",
		TextColor.Yellow => "E9D66B",
		TextColor.Green => "76CD26",
		TextColor.Blue => "6495ED",
		TextColor.Purple => "CA9BF7",
		TextColor.Brown => "483C32",
		_ => "FFFFFF",
	};

    public static void Print(string text, TextColor textColor)
    {
        string textColorString = GetColorHex(textColor);
        GD.PrintRich($"[color={textColorString}]{text}[/color]");
    }

    // Adds new lines for readability
    public static void PrintLine(string text, TextColor textColor)
    {
        if (!IsConsoleEnabled) return;

        string textColorString = GetColorHex(textColor);
        GD.PrintRich($"[color={textColorString}]{text}[/color]");
        GD.Print(); // Spacing
    }

    // For loading data for objects
    public static void PrintJSONSuccess(string fileName, TextColor textColor = TextColor.Green)
    {
        string successMessage = $"{fileName} Successfully Loaded";
        PrintLine(successMessage, textColor);
    }

    // For player's save files
    public static void PrintFileSuccess(string name, FileAccess file, string state, TextColor textColor = TextColor.Green)
    {
        string successMessage = $"{name} Successfully {state}";
        if (AreFilePathsVisible)
        {
            successMessage += $" At {file.GetPathAbsolute()}";
        }
        PrintLine(successMessage, textColor);
    }

    public static void PrintError(string className, string message, string result = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
    {
        result = string.IsNullOrEmpty(result) ? "" : $"| {result}";
        string errorMessage = $"{className}.cs | {memberName} (Line {lineNumber}) | {message} {result}";
        GD.PrintErr(errorMessage);
    }

    public static void PrintOrbs(string text, List<SkillOrb> skillOrbs, TextColor textColor = TextColor.Yellow)
    {
        string skillOrbText = "[";
        foreach (SkillOrb skillOrb in skillOrbs)
        {
            skillOrbText += $"{skillOrb.SkillType} ";
        }
        skillOrbText = skillOrbText.TrimEnd();
        skillOrbText += "]";

        Print(text + skillOrbText, textColor);
    }

    public static void PrintOrbs(List<List<SkillOrb>> columns, TextColor textColor = TextColor.Yellow)
    {
        foreach (List<SkillOrb> rows in columns)
        {
            string skillOrbText = "[";
            foreach (SkillOrb skillOrb in rows)
            {
                if (skillOrb == null)
                {
                    GD.Print("Skill Orb is Null");
                    continue;
                }
                skillOrbText += $"{skillOrb.SkillType} ";
            }
            skillOrbText = skillOrbText.TrimEnd();
            skillOrbText += "]";

            Print(skillOrbText, textColor);
        }
    }

    public static void PrintPersona(Persona persona)
    {
        string personaID = $"ID: {persona.ID}\n";
        string personaName = $"Name: {persona.Name}\n";
        string personaSkillType = $"Skill Type: {persona.SkillType}\n";
        string personaLevel = $"Level: {persona.Level}\n";
        string personaRank = $"Rank: {persona.Rank}\n";

        string personaStrength = $"Strength: {persona.Strength}\n";
        string personaMagic = $"Magic: {persona.Magic}\n";
        string personaEndurance = $"Endurance: {persona.Endurance}\n";
        string personaAgility = $"Agility: {persona.Agility}\n";
        string personaLuck = $"Luck: {persona.Luck}";
        string personaStats = $"{personaStrength}{personaMagic}{personaEndurance}{personaAgility}{personaLuck}";

        string personaString = $"{personaID}{personaName}{personaSkillType}{personaLevel}{personaRank}{personaStats}";
        Print(personaString, TextColor.Yellow);
        GD.Print(); // Spacing
    }
}