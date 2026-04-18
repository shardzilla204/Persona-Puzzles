using Godot;
using System;

namespace PersonaAndPuzzles;

public partial class DraggableComponent : Control
{
    public override Variant _GetDragData(Vector2 atPosition)
    {
        Control node = GetParent<Control>();
        Control clone = (Control) node.Duplicate();
        
        Color transparent = Colors.White;
        transparent.A = 0.5f;
        Control control = new Control()
        {
            Modulate = transparent
        };
        control.AddChild(clone);
        clone.GlobalPosition = -(clone.Size / 2);
        SetDragPreview(control);
        return control;
    }
}
