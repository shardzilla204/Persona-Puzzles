using Godot;

namespace PersonaAndPuzzles;

public partial class PersonaSlot : TextureRect
{
    [Export]
    private TextureRect _personaIcon;

    [Export] 
    private TextureRect _skillTypeIcon;

    public Persona Persona;

    public override void _Ready()
    {
        Persona randomPersona = PersonaManager.GetRandomPersona();
        SetPersona(randomPersona);
    }

    public void SetPersona(Persona persona)
    {
        Persona = persona;
        _personaIcon.Texture = PersonaManager.GetPersonaImage(persona);
        SetSkillType(persona.SkillType);
    }

    private void SetSkillType(SkillType skillType)
    {
        Color skillTypeColor = SkillOrbManager.GetSkillTypeColor(skillType);
        SelfModulate = skillTypeColor;

        Texture2D skillTypeTexture = SkillOrbManager.GetSkillTypeTexture(skillType);
        _skillTypeIcon.Texture = skillTypeTexture;
    }
}
