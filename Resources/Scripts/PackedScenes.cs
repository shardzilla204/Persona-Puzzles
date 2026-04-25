using Godot;

namespace PersonaAndPuzzles;

public partial class PackedScenes : Node
{
    private const string _SkillOrb = "uid://q7xw4c2ix7lw";
    private const string _ComboLabel = "uid://dg41dcug83ks";

    public override void _EnterTree()
    {
        PersonaAndPuzzles.PackedScenes = this;
    }

    public SkillOrb GetSkillOrb(SkillType skillType)
    {
        PackedScene skillOrbScene = GD.Load<PackedScene>(_SkillOrb);
        SkillOrb skillOrb = skillOrbScene.Instantiate<SkillOrb>();
        skillOrb.SkillType = skillType;
        return skillOrb;
    }

    public ComboLabel GetComboLabel(int comboCount)
    {
        PackedScene comboLabelScene = GD.Load<PackedScene>(_ComboLabel);
        ComboLabel comboLabel = comboLabelScene.Instantiate<ComboLabel>();
        comboLabel.Text = $"Combo {comboCount}";
        return comboLabel;
    }
}
