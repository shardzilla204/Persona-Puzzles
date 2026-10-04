using Godot;
using Godot.Collections;
using System;

namespace PersonaAndPuzzles;

public partial class MaterialEnhance : TextureRect
{
    [Signal]
    public delegate void MaterialsClearedEventHandler();

    [Signal]
    public delegate void EnhancedPersonaEventHandler(int totalExperience);

    [Signal]
    public delegate void MaterialsUpdatedEventHandler(int totalExperience);

    [Export]
    private Container _materialCounters;

    [Export]
    private CustomButton _clearButton;

    [Export]
    private CustomButton _enhanceButton;

    [Export]
    private CustomButton _maxButton;

    public Persona Persona;

    private Dictionary<EnhancementMaterial, int> _materials = new Dictionary<EnhancementMaterial, int>()
    {
        { EnhancementMaterial.Legend, 0 },
        { EnhancementMaterial.Anecdote, 0 },
        { EnhancementMaterial.Memory, 0 }
    };

    public override void _Ready()
    {
        _clearButton.Pressed += OnClearButtonPressed;
        _enhanceButton.Pressed += OnEnhanceButtonPressed;
        _maxButton.Pressed += OnMaxButtonPressed;

        AddMaterialCounters();
    }

    private void OnClearButtonPressed()
    {
        foreach (EnhancementMaterialCounter materialCounter in _materialCounters.GetChildren())
        {
            materialCounter.Reset();
            _materials[materialCounter.Material] = 0;
        }

        EmitSignal(SignalName.MaterialsCleared);
    }

    private void OnEnhanceButtonPressed()
    {
        int totalExperience = GetTotalExperience(_materials);
        EmitSignal(SignalName.EnhancedPersona, totalExperience);

        foreach (EnhancementMaterialCounter materialCounter in _materialCounters.GetChildren())
        {
            string materialName = $"{materialCounter.Material}";
            int materialCount = _materials[materialCounter.Material];
            int playerMaterialCount = Player.Instance.GetMaterialCount(materialName);
            int difference = playerMaterialCount - materialCount;
            Player.Instance.SetMaterialCount(materialName, difference);

            materialCounter.Reset();
            _materials[materialCounter.Material] = 0;
        }

        foreach (EnhancementMaterialCounter materialCounter in _materialCounters.GetChildren())
        {
            int materialCount = Player.Instance.GetMaterialCount($"{materialCounter.Material}");
            GD.Print($"Material Count: {materialCounter.Material} {materialCount}");
        }
    }

    private void OnMaxButtonPressed()
    {
        bool isMaxed = false;
        Dictionary<EnhancementMaterial, int> materials = new Dictionary<EnhancementMaterial, int>(_materials);
        foreach (EnhancementMaterial material in materials.Keys)
        {
            int materialCount = Player.Instance.GetMaterialCount($"{material}");
            while (++materials[material] <= materialCount || 
                materialCount - materials[material] == 0)
            {
                int experience = GetTotalExperience(materials);
                int levels = PersonaManager.Instance.GetLevels(Persona, experience);
                if (Persona.Level + levels >= PersonaManager.Instance.GetMaxLevel(Persona))
                {
                    isMaxed = true;
                    break;
                }
            }
            if (isMaxed) break;
        }
        
        foreach (EnhancementMaterial material in materials.Keys)
        {
            int amount = materials[material];
            SetMaterialAmount(material, amount);
        }

        foreach (EnhancementMaterialCounter materialCounter in _materialCounters.GetChildren())
        {
            int amount = materials[materialCounter.Material];
            materialCounter.SetAmount(amount);
        }
    }

    public void OnPersonaLevelMaxed(bool isMaxed)
    {
        foreach (EnhancementMaterialCounter materialCounter in _materialCounters.GetChildren())
        {
            materialCounter.Toggle(isMaxed);
        }
    }

    private void AddMaterialCounters()
    {
        const string UID = "uid://cfjltkvetl7m2";

        string[] materialNames = Enum.GetNames<EnhancementMaterial>();
        foreach (string materialName in materialNames)
        {
            EnhancementMaterial material = Enum.Parse<EnhancementMaterial>(materialName);
            EnhancementMaterialCounter materialCounter = GD.Load<PackedScene>(UID).Instantiate<EnhancementMaterialCounter>();
            materialCounter.SetMaterial(material);
            materialCounter.Connect(EnhancementMaterialCounter.SignalName.AmountChanged, 
                new Callable(this, MethodName.SetMaterialAmount));
            _materialCounters.AddChild(materialCounter);
        }
    }

    private void SetMaterialAmount(EnhancementMaterial material, int amount)
    {
        _materials[material] = amount;

        int totalExperience = GetTotalExperience(_materials);
        EmitSignal(SignalName.MaterialsUpdated, totalExperience);
    }

    private int GetTotalExperience(Dictionary<EnhancementMaterial, int> materials)
    {
        int totalExperience = 0;
        foreach (EnhancementMaterial material in materials.Keys)
        {
            int amount = materials[material];
            totalExperience += PersonaManager.Instance.GetMaterialExperience(material) * amount;
        }
        return totalExperience;
    }

    public void AddConvertedMaterials(Dictionary<EnhancementMaterial, int> materials)
    {
        foreach (EnhancementMaterial material in materials.Keys)
        {
            GD.Print($"Added {materials[material]} {material} Cubes");
            _materials[material] += materials[material];
        }
    }
}
