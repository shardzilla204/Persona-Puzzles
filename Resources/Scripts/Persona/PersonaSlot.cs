using Godot;

namespace PersonaAndPuzzles;

public partial class PersonaSlot : TextureRect
{
    [Export]
    private TextureRect _personaIcon;

    [Export]
    private TextureRect _skillTypeIcon;

    public Persona Persona;

    public virtual void SetPersona(Persona persona)
    {
        Persona = persona;
        _personaIcon.Texture = PersonaManager.Instance.GetPersonaImage(persona);

        SetSkillType(persona.SkillType);
    }

    private void SetSkillType(SkillType skillType)
    {
        Color skillTypeColor = SkillOrbManager.Instance.GetSkillTypeColor(skillType);
        SelfModulate = skillTypeColor;

        Texture2D skillTypeTexture = SkillOrbManager.Instance.GetSkillTypeTexture(skillType);
        _skillTypeIcon.Texture = skillTypeTexture;
    }
}
