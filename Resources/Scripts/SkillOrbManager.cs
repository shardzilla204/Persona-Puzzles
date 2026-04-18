using Godot;
using GC = Godot.Collections;
using System;
using System.Collections.Generic;

namespace PersonaAndPuzzles;

public partial class SkillOrbManager : Node
{
    private static GC.Dictionary<SkillType, string> _skillIconTextures = new GC.Dictionary<SkillType, string>()
    {
        { SkillType.Physical, "uid://cqni26flnijsp" },
        { SkillType.Pierce, "uid://bvu4wxouwdsy2" },
        { SkillType.Fire, "uid://cawvxxk61k81b" },
        { SkillType.Ice, "uid://b0hjqsi5kt557" },
        { SkillType.Wind, "uid://7xdb76ljgitr" },
        { SkillType.Thunder, "uid://g25nt0c76mqs" },
        { SkillType.Bless, "uid://ifejkslor7c5" },
        { SkillType.Curse, "uid://dpl1xkw7tpvj2" },
        { SkillType.Nuclear, "uid://dkfkxabl8uujs" },
        { SkillType.Psychic, "uid://biykvkgohicy1" },
        { SkillType.Almighty, "uid://c12n424vwjulu" },
    };

    public static SkillOrb GetRandomSkillOrb()
    {
        RandomNumberGenerator RNG = new RandomNumberGenerator();
        SkillType skillType = (SkillType) RNG.RandiRange(0, Enum.GetNames(typeof(SkillType)).Length - 1);
        // GD.Print($"Type: {skillType}");
        SkillOrb skillOrb = PersonaAndPuzzles.PackedScenes.GetSkillOrb(skillType);
        return skillOrb;
    }

    public static Texture2D GetSkillTypeTexture(SkillType skillType)
    {
        try
        {
            // GD.Print($"Type: {skillType}");
            string skillTypeTextureUID = _skillIconTextures[skillType];
            // GD.Print($"Type UID: {skillTypeTextureUID}");
            return GD.Load<Texture2D>(skillTypeTextureUID);  
        }
        catch (KeyNotFoundException)
        {
            GD.Print(skillType);
            GD.Print(Enum.GetName(typeof(SkillType), skillType));
            return null;
        }

    }
}
