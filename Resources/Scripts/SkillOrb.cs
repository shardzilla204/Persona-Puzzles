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
    private TextureRect _skillIcon;

    [Export]
    public SkillType SkillType;

    private bool _isDragging = false;

    public override void _Ready()
    {
        _skillIcon.Texture = SkillOrbManager.GetSkillTypeTexture(SkillType);
        // GD.Print($"Skill Type: {SkillType}");
    }

    public override void _Notification(int what)
    {
        if (what == NotificationDragEnd && _isDragging)
        {
            _isDragging = false;
            Modulate = Colors.White;
        }
    }


    public override Variant _GetDragData(Vector2 atPosition)
    {
        SkillOrb skillOrb = (SkillOrb) Duplicate();
        Control control = new Control()
        {
            Scale = new Vector2(1.25f, 1.25f)
        };
        control.AddChild(skillOrb);
        skillOrb.GlobalPosition = -(skillOrb.Size / 2);
        SetDragPreview(control);

        Color transparent = Colors.White;
        transparent.A = 0f;
        Modulate = transparent;

        _isDragging = true;

        return this;
    }
}
