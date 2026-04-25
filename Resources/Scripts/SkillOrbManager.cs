using Godot;
using GC = Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

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
        SkillType skillType = (SkillType) RNG.RandiRange(0, 3);
        // SkillType skillType = (SkillType) RNG.RandiRange(0, _skillIconTextures.Count() - 1);
        SkillOrb skillOrb = PersonaAndPuzzles.PackedScenes.GetSkillOrb(skillType);
        return skillOrb;
    }

    public static Texture2D GetSkillTypeTexture(SkillType skillType)
    {
        try
        {
            string skillTypeTextureUID = _skillIconTextures[skillType];
            return GD.Load<Texture2D>(skillTypeTextureUID);  
        }
        catch (KeyNotFoundException)
        {
            return null;
        }

    }
}
