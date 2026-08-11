using Godot;
using System.Collections.Generic;
using System.Linq;

namespace PersonaAndPuzzles;

public partial class VelvetTrialPersona : NinePatchRect
{
    [Export]
    private TextureRect _personaIcon;

    [Export]
    private Container _weaknessContainer;

    public void SetPersona(Persona persona)
    {
        _personaIcon.Texture = PersonaManager.GetPersonaImage(persona);
        DisplayWeaknesses(persona);
    }

    public void DisplayWeaknesses(Persona persona)
    {
        const int Size = 15;
        List<SkillType> weaknesses = persona.Resistances.Keys.Where(key => persona.Resistances[key] == Resistance.Weak).ToList();
        foreach (SkillType weakness in weaknesses)
        {
            TextureRect weaknessTexture = new TextureRect()
            {
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                CustomMinimumSize = new Vector2(Size, Size),
                GrowVertical = GrowDirection.End,
                Texture = SkillOrbManager.GetSkillTypeTexture(weakness)
            };
            _weaknessContainer.AddChild(weaknessTexture);
        }
    }
}
