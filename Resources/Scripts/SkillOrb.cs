using Godot;
using System;

namespace PersonaAndPuzzles;

public enum SkillType
{
    None = -1,
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
    public SkillType SkillType = SkillType.None;

    public bool IsMatched = false;

    private bool _isDragging = false;

    public override void _Ready()
    {
        _skillIcon.Texture = SkillOrbManager.GetSkillTypeTexture(SkillType);
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

    public void SetMatch(bool isMatched)
    {
        IsMatched = isMatched;
        SetAlpha();
    }

    private void SetAlpha()
    {
        Color translucent = Colors.White;
        translucent.A = 0.5f;
        Modulate = IsMatched ? translucent : Colors.White;
    }
}
