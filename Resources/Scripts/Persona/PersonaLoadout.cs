using Godot;
using System.Collections.Generic;

namespace PersonaAndPuzzles;

public partial class PersonaLoadout : Node
{
    public PersonaLoadout() {}
    public PersonaLoadout(string name, int index)
    {
        Name = name;
        Index = index;
    }

    public PersonaLoadout(PersonaLoadout Loadout)
    {
        Name = Loadout.Name;
        Personas.AddRange(Loadout.Personas);
    }
    
    public int Index = 0;
    public List<Persona> Personas = new List<Persona>();
    public List<Persona> Reserves = new List<Persona>();

    public void ChangeLoadout(PersonaLoadout Loadout)
    {
        Name = Loadout.Name;
        Personas.Clear();
        Personas.AddRange(Loadout.Personas);
    }

    public void Swap(Persona newPersona, int slotIndex)
    {
        Personas.RemoveAt(slotIndex);
        Personas.Insert(slotIndex, newPersona);
    }

    public void Swap(Persona newPersona, string oldPersonaID)
    {
        int personaIndex = Personas.FindIndex(persona => persona.ID == oldPersonaID);
        Personas[personaIndex] = newPersona;
    }
}
