using Godot;
using System;

namespace PersonaAndPuzzles;

public enum SkillType
{
    Physical,
    Pierce,
    Fire,
    Ice,
    Wind,
    Thunder,
    Bless,
    Curse,
    Nuclear,
    Psychic,
    Almighty
}

public partial class SkillOrb : TextureRect
{
    [Export]
    private DraggableComponent _draggableComponent;

    [Export]
    private TextureRect _skillIcon;

    [Export]
    public SkillType SkillType;

    public override void _Ready()
    {
        _skillIcon.Texture = SkillOrbManager.GetSkillTypeTexture(SkillType);
        // GD.Print($"Skill Type: {SkillType}");
    }

    public override Variant _GetDragData(Vector2 atPosition)
    {
        SkillOrb skillOrb = (SkillOrb) Duplicate();
        Control control = new Control();
        control.AddChild(skillOrb);
        skillOrb.GlobalPosition = -(skillOrb.Size / 2);
        SetDragPreview(control);

        Color transparent = Colors.White;
        transparent.A = 0;
        Modulate = transparent;
        return this;
    }
}
