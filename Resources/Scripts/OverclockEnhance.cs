using Godot;
using System.Collections.Generic;

namespace PersonaAndPuzzles;

public partial class OverclockEnhance : TextureRect
{
    [Signal]
    public delegate void OverclockedEventHandler();
    
    [Export]
    private Container _enhanceSlotContainer;

    [Export]
    private CustomButton _overclockButton;

    public Persona Persona;

    private CompendiumEnhanceSlot _selectedSlot;

    public override void _Ready()
    {
        _overclockButton.Pressed += OnOverclockButtonPressed;

        AddEnhanceSlots();
    }

    private void AddEnhanceSlots()
    {
        List<Persona> personas = PersonaManager.Personas.FindAll(persona => persona.Name == Persona.Name && persona.ID != Persona.ID);

        const string UID = "uid://c3ax22hnrv1lm";
        foreach (Persona persona in personas)
        {
            CompendiumEnhanceSlot enhanceSlot = GD.Load<PackedScene>(UID).Instantiate<CompendiumEnhanceSlot>();
            enhanceSlot.SetPersona(persona);
            _enhanceSlotContainer.AddChild(enhanceSlot);
            enhanceSlot.Connect(CompendiumEnhanceSlot.SignalName.Toggled, new Callable(this, MethodName.ChangeEnhanceSlots));
        }
    }

    private void ChangeEnhanceSlots(CompendiumEnhanceSlot targetSlot, bool isToggled)
    {
        foreach (CompendiumEnhanceSlot slot in _enhanceSlotContainer.GetChildren())
        {
            if (targetSlot == slot) continue;
            slot.Toggle(false);
        }
        _selectedSlot = targetSlot;
    }

    private void OnOverclockButtonPressed()
    {
        if (_selectedSlot == null) return;

        PersonaManager.Personas.Remove(_selectedSlot.Persona);
        _selectedSlot.QueueFree();

        Persona.Overclock++;

        EmitSignal(SignalName.Overclocked);
        QueueFree();
    }
}
