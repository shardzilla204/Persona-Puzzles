using Godot;
using System;

namespace PersonaAndPuzzles;

public enum Ailment
{
    
}

public class Skill
{
    public string Name;
    public string Effect;
    public int Power;
    public int Accuracy;
    public int Critical;
    public bool IsTransferable = true;
}
