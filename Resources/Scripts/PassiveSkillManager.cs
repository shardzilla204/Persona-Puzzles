using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PersonaAndPuzzles;

public partial class PassiveSkillManager : Node
{
    public static List<PassiveSkill> PassiveSkills = new List<PassiveSkill>();

    public static string[] _offensiveSkills = [ "Boost", "Amp", "Apt Pupil", "Snipe", "Cripple", "Magic Ability", "Trigger Happy" ];
    public static string[] _defensiveSkills = [ "Dodge", "Evade", "Sharp Student", "Angelic Grace" ];

    // Keys
    private const string _NameKey = "Name";
    private const string _EffectKey = "Effect";

    private const string _PassiveSkillFileName = "PassiveSkills";

    public static void LoadPassiveSkills()
    {
        GC.Dictionary<string, Variant> passiveSkillData = PersonaAndPuzzles.LoadFile(_PassiveSkillFileName, "Skills").As<GC.Dictionary<string, Variant>>();
        List<GC.Dictionary<string, Variant>> passiveSkillDictionaries = passiveSkillData[_PassiveSkillFileName].As<GC.Array<GC.Dictionary<string, Variant>>>().ToList();
        foreach (GC.Dictionary<string, Variant> passiveSkillDictionary in passiveSkillDictionaries)
        {
            PassiveSkill passiveSkill = new PassiveSkill()
            {
                Name = passiveSkillDictionary[_NameKey].As<string>(),
                Effect = passiveSkillDictionary[_EffectKey].As<string>()
            };
            PassiveSkills.Add(passiveSkill);
        }
    }

    public static List<PassiveSkill> GetOffensiveSkills(Persona persona)
    {
        List<PassiveSkill> offensiveSkills = new List<PassiveSkill>();
        foreach (PassiveSkill passiveSkill in persona.PassiveSkills)
        {
            if (_offensiveSkills.Any(skill => passiveSkill.Name.Contains(skill)))
            {
                offensiveSkills.Add(passiveSkill);
            }
        }
        return offensiveSkills;
    }

    public static bool CanApplyOffensiveSkill(Persona persona, PassiveSkill passiveSkill)
    {
        if (passiveSkill.Name.Contains($"{persona.SkillType}") ||
            _offensiveSkills.Any(skill => skill.Contains(passiveSkill.Name)))
        {
            return true;
        }

        return false;
    }

    public static List<PassiveSkill> GetDefensiveSkills(Persona persona)
    {
        List<PassiveSkill> defensiveSkills = new List<PassiveSkill>();
        foreach (PassiveSkill passiveSkill in persona.PassiveSkills)
        {
            if (_defensiveSkills.Any(skill => passiveSkill.Name.Contains(skill)))
            {
                defensiveSkills.Add(passiveSkill);
            }
        }
        return defensiveSkills;
    }

    public static bool CanApplyDefensiveSkill(PassiveSkill passiveSkill)
    {
        return _defensiveSkills.Any(skill => skill.Contains(passiveSkill.Name));
    }

    public static float GetOffensiveSkill(PassiveSkill passiveSkill)
    {
        if (passiveSkill.Name.Contains("Boost"))
        {
            return 1.25f;
        }
        else if (passiveSkill.Name.Contains("Amp"))
        {
            return 1.5f;
        }
        return 1f;
    }

    public static float GetDefensiveSkill(PassiveSkill passiveSkill)
    {
        if (passiveSkill.Name.Contains("Dodge"))
        {
            return 2f;
        }
        else if (passiveSkill.Name.Contains("Evade"))
        {
            return 3f;
        }
        return 2f;
    }

    public static PassiveSkill FindPassiveSkill(string targetPassiveSkill)
    {
        PassiveSkill passiveSkill = PassiveSkills.Find(skill => skill.Name == targetPassiveSkill);
        return passiveSkill;
    }
}
