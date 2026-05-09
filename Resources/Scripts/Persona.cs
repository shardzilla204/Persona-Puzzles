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
    Absorb
}

public class Persona
{
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

    public List<Skill> Skills = new List<Skill>();
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
        { SkillType.Curse, Resistance.None }
    };
}
