using System;
using Godot;

namespace PersonaAndPuzzles;

public partial class SkillTypeFilterOption : FilterOption
{
    [Export]
    private TextureRect _skillTypeIcon;

    public SkillType SkillType;

    public override void _Ready()
    {
        base._Ready();

        SkillType = Enum.Parse<SkillType>(Name);
        _skillTypeIcon.Texture = SkillOrbManager.GetSkillTypeTexture(SkillType);
    }
}
