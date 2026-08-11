using Godot;

namespace PersonaAndPuzzles;

public partial class CompendiumSkillResistance : VBoxContainer
{
    [Export]
    private TextureRect _skillTypeIcon;

    [Export]
    private Label _resistanceLabel;

    public void Set(SkillType skillType, Resistance resistance)
    {
        Texture2D skillTexture = SkillOrbManager.GetSkillTypeTexture(skillType);
        _skillTypeIcon.Texture = skillTexture;
        _resistanceLabel.Text = GetResistanceAbbreviation(resistance);
    }

    private string GetResistanceAbbreviation(Resistance resistance) => resistance switch
    {
        Resistance.None => "-",
        Resistance.Weak => "Wk",
        Resistance.Resist => "Str",
        Resistance.Null => "Nul",
        Resistance.Repel => "Rep",
        Resistance.Drain => "Dr",
        _ => "-"
    };
}
