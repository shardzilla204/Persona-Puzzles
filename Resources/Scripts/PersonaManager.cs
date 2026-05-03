using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

namespace PersonaAndPuzzles;

public partial class PersonaManager : Node
{
    // Keys
    public const string _NameKey = "Name";
    public const string _ImageKey = "Image";
    public const string _ArcanaKey = "Arcana";
    public const string _SkillTypeKey = "SkillType";

    public const string _PersonaFileName = "Persona";

    public static List<Persona> Personas = new List<Persona>();

    public static void LoadPersonas()
    {
        GC.Dictionary<string, Variant> personaData = (GC.Dictionary<string, Variant>) PersonaAndPuzzles.LoadFile(_PersonaFileName);
        List<GC.Dictionary<string, Variant>> personaDictionaries = personaData[_PersonaFileName].As<GC.Array<GC.Dictionary<string, Variant>>>().ToList();
        foreach (GC.Dictionary<string, Variant> personaDictionary in personaDictionaries)
        {
            Persona persona = new Persona()
            {
                Name = personaDictionary[_NameKey].As<string>(),
                Image = personaDictionary[_ImageKey].As<string>(),
                Arcana = GetArcanaFromDictionary(personaDictionary),
                SkillType = GetSkillTypeFromDictionary(personaDictionary)
            };
            Personas.Add(persona);
        }
    }

    private static Arcana GetArcanaFromDictionary(GC.Dictionary<string, Variant> personaDictionary)
    {
        string personaArcana = personaDictionary[_ArcanaKey].As<string>();
        try
        {
            Enum.Parse<Arcana>(personaArcana);
            return Enum.Parse<Arcana>(personaDictionary[_ArcanaKey].As<string>());
        }
        catch
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
			string message = $"Couldn't Parse Arcana - {personaArcana}";
			string result = "Returning Arcana.Fool";
			PrintRich.PrintError(className, message, result);
            
            return Arcana.Fool;
        }
    }

    private static SkillType GetSkillTypeFromDictionary(GC.Dictionary<string, Variant> personaDictionary)
    {
        string personaSkillType = personaDictionary[_SkillTypeKey].As<string>();
        try
        {
            Enum.Parse<SkillType>(personaSkillType);
            return Enum.Parse<SkillType>(personaDictionary[_SkillTypeKey].As<string>());
        }
        catch
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
			string message = $"Couldn't Parse SkillType - {personaSkillType}";
			string result = "Returning SkillType.None";
			PrintRich.PrintError(className, message, result);
            
            return SkillType.None;
        }
    }

    public static Texture2D GetPersonaImage(Persona persona)
    {
        try
        {
            Texture2D personaImage = GD.Load<Texture2D>(persona.Image);
            return personaImage;
        }
        catch
        {
            GD.PrintErr("Couldn't Load Persona Image");
            return null;
        }
    }

    public static Persona GetRandomPersona()
    {
        RandomNumberGenerator RNG = new RandomNumberGenerator();
        int randomNumber = RNG.RandiRange(0, Personas.Count - 1);
        return Personas[randomNumber];
    }

    public static List<Persona> GetPersonasByArcana(Arcana arcana)
    {
        return Personas.FindAll(persona => persona.Arcana == arcana);
    }

    public static List<Persona> GetPersonasBySkillType(SkillType skillType)
    {
        return Personas.FindAll(persona => persona.SkillType == skillType);
    }

    public static Persona GetPersonaByName(string personaName)
    {
        return Personas.Find(persona => persona.Name == personaName);
    }
}
