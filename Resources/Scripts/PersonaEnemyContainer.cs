using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonaAndPuzzles;

public partial class PersonaEnemyContainer : Container
{
    public List<PersonaEnemy> Enemies = new List<PersonaEnemy>();
    public List<Persona> CapturedPersona = new List<Persona>();

    private PersonaEnemy _targetedEnemy;

    private int _waveIndex = 0;

    public void AddEnemies(VelvetTrialWave wave)
    {
        const string NameKey = "Name";
        const string LevelKey = "Level";
        const string CaptureRateKey = "CaptureRate";

        const string UID = "uid://mr8t4ljkmwyv";
        foreach (GC.Dictionary<string, Variant> personaDictionary in wave.PersonaDictionaries)
        {
            string personaName = personaDictionary[NameKey].As<string>();
            int personaLevel = personaDictionary[LevelKey].As<int>();
            Variant captureRate = personaDictionary.TryGetValue(CaptureRateKey, out _);

            // Create the Persona with set level
            Persona persona = PersonaManager.GetPersonaByName(personaName);
            Persona personaClone = new Persona(persona);
            PersonaManager.SetLevel(personaClone, personaLevel);

            PersonaEnemy personaEnemy = GD.Load<PackedScene>(UID).Instantiate<PersonaEnemy>();
            personaEnemy.Persona = personaClone;
            personaEnemy.CaptureRate = captureRate.As<float>();
            personaEnemy.Connect(PersonaEnemy.SignalName.Captured, 
                new Callable(this, MethodName.OnEnemyCaptured));
            personaEnemy.Connect(PersonaEnemy.SignalName.Targeted, 
                new Callable(this, MethodName.OnEnemyTargeted));

            Enemies.Add(personaEnemy);
            AddChild(personaEnemy);

            PrintRich.PrintPersona(personaEnemy.Persona);
        }
    }

    private void OnEnemyTargeted(PersonaEnemy enemy)
    {
        if (_targetedEnemy != null) _targetedEnemy.ToggleButton(false);
        _targetedEnemy = _targetedEnemy != enemy ? enemy : null;
    }
    
    private void OnEnemyCaptured(PersonaEnemy enemy)
    {
        CapturedPersona.Add(enemy.Persona);
    }

    public void GetCapturedPersonas()
    {
        foreach (Persona persona in CapturedPersona)
        {
            PersonaManager.Personas.Add(persona);
        }
    }

    private void Clear()
    {
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }
    }

    public PersonaEnemy GetRandomEnemy(SkillType skillType, List<PersonaEnemy> targets)
    {
        List<PersonaEnemy> enemies = FindEnemies(skillType, targets);

        RandomNumberGenerator RNG = new RandomNumberGenerator();
        int personaIndex = RNG.RandiRange(0, enemies.Count - 1);
        return enemies[personaIndex];
    }

    public GC.Dictionary<PersonaEnemy, int> GetDamageDictionary()
    {
        GC.Dictionary<PersonaEnemy, int> damageDictionary = new GC.Dictionary<PersonaEnemy, int>();
        foreach (PersonaEnemy enemy in Enemies)
        {
            damageDictionary.Add(enemy, 0);
        }
        return damageDictionary;
    }

    private List<PersonaEnemy> FindEnemies(SkillType skillType, List<PersonaEnemy> targets)
    {
        List<PersonaEnemy> enemies = new List<PersonaEnemy>();

        Resistance[] searchOrder = [ 
            Resistance.Weak, 
            Resistance.None, 
            Resistance.Resist, 
            Resistance.Null, 
            Resistance.Drain, 
            Resistance.Repel 
        ];

        foreach (Resistance resistance in searchOrder)
        {
            enemies = targets.FindAll(enemy => enemy.Persona.Resistances[skillType] == resistance);
            if (enemies.Count > 0) break;
        }

        return enemies;
    }

    public async Task TweenAttackAsync()
    {
        foreach (PersonaEnemy enemy in Enemies)
        {
            enemy.TweenAttack();
        }
    }

    public int GetDamage()
    {
        const float Multiplier = 5;

        int damage = 0;
        foreach (PersonaEnemy enemy in Enemies)
        {
            if (enemy.Persona.SkillType == SkillType.Physical || 
            enemy.Persona.SkillType == SkillType.Pierce)
            {
                damage += enemy.Persona.Strength;
            }
            else
            {
                damage += enemy.Persona.Magic;
            }
        }
        return Mathf.RoundToInt(damage * Multiplier);
    }
}
