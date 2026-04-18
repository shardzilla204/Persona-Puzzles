using Godot;

namespace PersonaAndPuzzles;

public partial class PackedScenes : Node
{
    private const string _SkillOrb = "uid://q7xw4c2ix7lw";

    public override void _EnterTree()
    {
        PersonaAndPuzzles.PackedScenes = this;
    }

    public SkillOrb GetSkillOrb(SkillType skillType)
    {
        PackedScene skillOrbScene = GD.Load<PackedScene>(_SkillOrb);
        SkillOrb skillOrb = skillOrbScene.Instantiate<SkillOrb>();
        skillOrb.SkillType = skillType;
        // GD.Print($"Orb Type: {skillOrb.SkillType}");
        return skillOrb;
    }
}
