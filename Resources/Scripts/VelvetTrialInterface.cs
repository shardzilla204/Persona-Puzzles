using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using GC = Godot.Collections;

namespace PersonaAndPuzzles;

public partial class VelvetTrialInterface : Control
{
    [Signal]
    public delegate void TrialCompletedEventHandler();

    [Export]
    private SkillOrbGrid _skillOrbGrid;

    [Export]
    private VelvetTrialSlotContainer _velvetTrialSlotContainer;

    [Export]
    private PersonaEnemyContainer _personaEnemyContainer;

    [Export]
    private PlayerHealthBar _playerHealthBar;

    [Export]
    private CustomButton _exitButton;

    [Export]
    private TextureRect _captureTexture;

    [Export]
    private Label _captureCount;

    public VelvetTrial VelvetTrial;
    private int _waveIndex = 0;

    private bool _hasWon;

    public override void _EnterTree()
    {
        SetSkillTypePool();
        SetCaptureCounter();
    }

    public override void _Ready()
    {
        _skillOrbGrid.SkillTypeIncreasedBySkillOrbs += IncreasePowerBySkillOrbs;
        _skillOrbGrid.SkillTypeIncreasedByCombo += IncreasePowerByCombo;
        _skillOrbGrid.FinishedCombos += StartCombatAsync;
        _playerHealthBar.Depleted += ShowStatus;
        _exitButton.Pressed += ShowConfirmation;

        VelvetTrialWave firstWave = VelvetTrial.Waves[_waveIndex];
        _personaEnemyContainer.AddEnemies(firstWave);
    }

    private void IncreasePowerBySkillOrbs(SkillType skillType, int skillOrbCount)
    {
        List<VelvetTrialSlot> slots = _velvetTrialSlotContainer.FindSlots(skillType);
        foreach (VelvetTrialSlot slot in slots)
        {
            slot.IncreasePowerBySkillOrbs(skillOrbCount);
        }
    }

    private void IncreasePowerByCombo()
    {
        foreach (VelvetTrialSlot personaSlot in _velvetTrialSlotContainer.Slots)
        {
            personaSlot.IncreasePowerByCombo();
        }
    }

    private async void StartCombatAsync()
    {
        PrintRich.Print("Starting Combat...", TextColor.Yellow);
        bool hasEnemies = await GiveEnemiesDamageAsync();
        if (!hasEnemies) 
        {
            // Go to next wave if all enemies in the current wave are defeated
            NextWave();
            return;
        }

        await _personaEnemyContainer.TweenAttackAsync();

        TakeEnemyDamage();
    }

    // ? Order the damage from highest to lowest first
    // Returns a bool for if enemies are still alive
    private async Task<bool> GiveEnemiesDamageAsync()
    {
        List<PersonaEnemy> targets = [.._personaEnemyContainer.Enemies];

        // Check if the damage already exceeds the enemies health, remove the enemy from the list of targets
        GC.Dictionary<PersonaEnemy, int> damageDictionary = _personaEnemyContainer.GetDamageDictionary();
        foreach (VelvetTrialSlot slot in _velvetTrialSlotContainer.Slots)
        {
            await slot.TweenAttackAsync();
            
            PersonaEnemy personaEnemy = _personaEnemyContainer.GetRandomEnemy(slot.Persona.SkillType, targets);
            if (damageDictionary[personaEnemy] >= personaEnemy.GetHealth() && targets.Count > 1)
            {
                targets.Remove(personaEnemy);
                personaEnemy = _personaEnemyContainer.GetRandomEnemy(slot.Persona.SkillType, targets);
            }

            damageDictionary[personaEnemy] += GetDamage(slot, personaEnemy);
        }

        List<Task> damageTasks = GetDamageTasks(damageDictionary);
        await Task.WhenAll(damageTasks);
        
        List<PersonaEnemy> enemies = [.._personaEnemyContainer.Enemies];
        List<Task> postDamageTasks = GetPostDamageTasks(enemies);
        await Task.WhenAll(postDamageTasks);

        SetCaptureCounter();

        return _personaEnemyContainer.Enemies.Count > 0;
    }

    private List<Task> GetDamageTasks(GC.Dictionary<PersonaEnemy, int> damageDictionary)
    {
        List<Task> damageTasks = new List<Task>();
        foreach (PersonaEnemy personaEnemy in damageDictionary.Keys)
        {
            int damage = damageDictionary[personaEnemy];
            Task damageTask = personaEnemy.TakeDamageAsync(damage);
            damageTasks.Add(damageTask);
        }
        return damageTasks;
    }

    // Removes enemy from enemy pool and either show capture or defeat tween
    private List<Task> GetPostDamageTasks(List<PersonaEnemy> enemies)
    {
        List<Task> postDamageTasks = new List<Task>();
        foreach (PersonaEnemy enemy in enemies)
        {
            bool isDefeated = enemy.HasDefeated();
            if (!isDefeated) continue;

            PrintRich.PrintLine($"Defeated {enemy.Persona.Name}", TextColor.Yellow);
            _personaEnemyContainer.Enemies.Remove(enemy);

            bool hasCaptured = enemy.HasCaptured();
            if (hasCaptured)
            {
                PrintRich.PrintLine($"Capturing {enemy.Persona.Name}", TextColor.Yellow);
                Task captureTask = enemy.CaptureAsync();
                postDamageTasks.Add(captureTask);
            }
            else
            {
                Task defeatTask = enemy.DefeatAsync();
                postDamageTasks.Add(defeatTask);
            }
        }
        return postDamageTasks;
    }

    private void TakeEnemyDamage()
    {
        int damage = _personaEnemyContainer.GetDamage();
        _ = _playerHealthBar.TakeDamageAsync(damage);
    }

    private int GetDamage(VelvetTrialSlot personaSlot, PersonaEnemy personaEnemy)
    {
        SkillType skillType = personaSlot.Persona.SkillType;
        Resistance resistance = personaEnemy.Persona.Resistances[skillType];
        float multiplier = GetResistanceMultiplier(resistance);
        int damage = Mathf.RoundToInt(personaSlot.Power * multiplier);

        return resistance != Resistance.Drain ? damage : -damage;
    }
    
    private float GetResistanceMultiplier(Resistance resistance) => resistance switch
    {
        Resistance.Weak => 1.5f,
        Resistance.Resist => 0.5f,
        Resistance.Null => 0,
        Resistance.Repel => 0,
        _ => 1
    };

    private void SetSkillTypePool()
    {
        List<SkillType> skillTypes = new List<SkillType>();
        foreach (Persona persona in PersonaManager.Instance.Loadout.Personas)
        {
            if (persona == null) continue;
            
            SkillType skillType = persona.SkillType;
            skillTypes.Add(skillType);
        }

        List<SkillType> distinctSkillTypes = skillTypes.Distinct().ToList();
        List<SkillType> unusedSkillTypes = new List<SkillType>();

        int skillTypeCount = Enum.GetValues<SkillType>().Count() - 1;
        for (int i = 0; i < skillTypeCount; i++)
        {
            SkillType skillType = (SkillType) i;
            if (!distinctSkillTypes.Contains(skillType))
            {
                unusedSkillTypes.Add(skillType);
            }
        }

        RandomNumberGenerator RNG = new RandomNumberGenerator();
        while (distinctSkillTypes.Count <= SkillOrbGrid.MaxSkillTypes)
        {
            int unusedSkillTypeIndex = RNG.RandiRange(0, unusedSkillTypes.Count - 1);
            SkillType unusedSkillType = unusedSkillTypes[unusedSkillTypeIndex];
            unusedSkillTypes.RemoveAt(unusedSkillTypeIndex);
            distinctSkillTypes.Add(unusedSkillType);
        }

        _skillOrbGrid.SkillTypePool = distinctSkillTypes;
    }

    private void GivePlayerRewards()
    {
        _personaEnemyContainer.GetCapturedPersonas();
        Player.Instance.KonpakuGems += VelvetTrial.KonpakuGems;
        GivePlayerExperience();
    }

    private void GivePlayerExperience()
    {
        Player.Instance.Experience += VelvetTrial.Experience;

        if (!Player.Instance.CanLevelUp()) return;
        
        Player.Instance.LevelUp();
    }

    public void NextWave()
    {
        _waveIndex++;
        if (_waveIndex >= VelvetTrial.Waves.Count)
        {
            _hasWon = true;
            EmitSignal(SignalName.TrialCompleted);

            GivePlayerRewards();
            ShowStatus();

            return;
        }

        VelvetTrialWave wave = VelvetTrial.Waves[_waveIndex];
        _personaEnemyContainer.AddEnemies(wave);
    }
    
    private void ShowStatus()
    {
        string status = _hasWon ? "You Win!" : "You Lose!";
        string errorMessage = Player.Instance.GetErrorMessage(VelvetTrial);

        const string UID = "uid://bvuid83fm3r8s";
        VelvetTrialResult velvetTrialResult = GD.Load<PackedScene>(UID).Instantiate<VelvetTrialResult>();
        velvetTrialResult.VelvetTrial = VelvetTrial;
        velvetTrialResult.DisableRestartButton(errorMessage);
        velvetTrialResult.Restarted += RestartVelvetTrial;
        velvetTrialResult.Exited += ExitVelvetTrial;
        AddChild(velvetTrialResult);

        velvetTrialResult.SetStatusLabel(status);
        if (_hasWon)
        {
            velvetTrialResult.AddCapturedPersonas(_personaEnemyContainer.CapturedPersona);
        }
        else
        {
            velvetTrialResult.SetLoss();
        }

        _exitButton.Visible = false;
        _skillOrbGrid.IsGamePaused = true;
    }

    private void ShowConfirmation()
    {
        const string Warning = "All progress will be lost";
        const string UID = "uid://cn6k7obbk8cx4";
        ConfirmationInterface confirmationInterface = GD.Load<PackedScene>(UID).Instantiate<ConfirmationInterface>();
        confirmationInterface.Accepted += ExitVelvetTrial;
        confirmationInterface.Canceled += () => _skillOrbGrid.IsGamePaused = false;
        confirmationInterface.SetWarningText(Warning);
        AddChild(confirmationInterface);

        _skillOrbGrid.IsGamePaused = true;
    }

    private void ExitVelvetTrial()
    {
        const string UID = "uid://bu33y3ibiqvpj";
        Control mainMenu = GD.Load<PackedScene>(UID).Instantiate<Control>();
        GetTree().Root.AddChild(mainMenu);

        QueueFree();
    }

    private void RestartVelvetTrial()
    {
        const string UID = "uid://ojbiqd2cianf";
        VelvetTrialInterface velvetTrialInterface = GD.Load<PackedScene>(UID).Instantiate<VelvetTrialInterface>();
        velvetTrialInterface.VelvetTrial = VelvetTrial;
        GetTree().Root.AddChild(velvetTrialInterface);

        Player.Instance.Stamina -= VelvetTrial.Stamina;

        QueueFree();
    }

    private void SetCaptureCounter()
    {
        _captureTexture.Visible = _personaEnemyContainer.CapturedPersona.Count != 0;
        _captureCount.Text = $"{_personaEnemyContainer.CapturedPersona.Count}";
    }
}
