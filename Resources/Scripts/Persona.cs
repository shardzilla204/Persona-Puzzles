using Godot;
using GC = Godot.Collections;
using System;
using System.Collections.Generic;

namespace PersonaAndPuzzles;

public enum Arcana
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
    Jester,
    Aeon
}

public class Persona
{
    public string Name;
    public string Image;

    public Arcana Arcana;
    public SkillType SkillType;

    public int Health;
    public int Attack;
    public int Defense;

    public List<Skill> Skills = new List<Skill>();
}
