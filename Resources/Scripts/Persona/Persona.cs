using Godot;
using GC = Godot.Collections;
using System;
using System.Collections.Generic;

namespace PersonaAndPuzzles;

public enum Race
{
    Fool,
    Magician,
    Priestess,
    Empress,
    Emperor,
    Hierophant,
    Lovers,
    Chariot,
    Justice,
    Hermit,
    Fortune,
    Strength,
    Hanged,
    Death,
    Temperance,
    Devil,
    Tower,
    Star,
    Moon,
    Sun,
    Judgement,
    Faith,
    Councillor
}

public enum Resistance
{
    None,
    Weak,
    Resist,
    Null,
    Repel,
    Drain
}

public class Persona
{
    public Persona(){}

    public Persona(Persona persona)
    {
        Name = persona.Name;
        Image = persona.Image;
        Race = persona.Race;
        SkillType = persona.SkillType;
        Level = persona.Level;

        Strength = persona.Strength;
        Magic = persona.Magic;
        Endurance = persona.Endurance;
        Agility = persona.Agility;
        Luck = persona.Luck;

        PassiveSkills = persona.PassiveSkills;
        Resistances = persona.Resistances;
    }

    public string ID;
    public string Name;
    public string Image;

    public Race Race;
    public SkillType SkillType;
    public int Level;

    public int Strength;
    public int Magic;
    public int Endurance;
    public int Agility;
    public int Luck;

    public bool IsFavorite;

    public List<PassiveSkill> PassiveSkills = new List<PassiveSkill>();
    public GC.Dictionary<SkillType, Resistance> Resistances = new GC.Dictionary<SkillType, Resistance>()
    {
        { SkillType.Physical, Resistance.None },
        { SkillType.Pierce, Resistance.None },
        { SkillType.Fire, Resistance.None },
        { SkillType.Ice, Resistance.None },
        { SkillType.Electric, Resistance.None },
        { SkillType.Wind, Resistance.None },
        { SkillType.Psychic, Resistance.None },
        { SkillType.Nuclear, Resistance.None },
        { SkillType.Bless, Resistance.None },
        { SkillType.Curse, Resistance.None },
        { SkillType.Almighty, Resistance.None }
    };
}
