using Godot;
using System.Collections.Generic;

namespace PersonaAndPuzzles;

public partial class PersonaRoster : Node
{
    public PersonaRoster() {}
    public PersonaRoster(string name, int index)
    {
        Name = name;
        Index = index;
    }

    public PersonaRoster(PersonaRoster roster)
    {
        Name = roster.Name;
        Personas.AddRange(roster.Personas);
    }
    
    public int Index = 0;
    public List<Persona> Personas = new List<Persona>();
    public List<Persona> Reserves = new List<Persona>();

    public void ChangeRoster(PersonaRoster roster)
    {
        Name = roster.Name;
        Personas.Clear();
        Personas.AddRange(roster.Personas);
    }

    public void Swap(Persona newPersona, int slotIndex)
    {
        Personas.RemoveAt(slotIndex);
        Personas.Insert(slotIndex, newPersona);
    }

    public void Swap(Persona newPersona, string oldPersonaID)
    {
        int personaIndex = Personas.FindIndex(persona => persona.ID == oldPersonaID);
        Personas.RemoveAt(personaIndex);
        Personas.Insert(personaIndex, newPersona);
    }
}
