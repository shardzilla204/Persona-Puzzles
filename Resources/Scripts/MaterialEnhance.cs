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

    private Dictionary<EnhancementMaterial, int> _materials = new Dictionary<EnhancementMaterial, int>()
    {
        { EnhancementMaterial.Memory, 0 },
        { EnhancementMaterial.Anecdote, 0 },
        { EnhancementMaterial.Legend, 0 }
    };

    public override void _Ready()
    {
        _clearButton.Pressed += OnClearButtonPressed;
        _enhanceButton.Pressed += OnEnhanceButtonPressed;

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
        int totalExperience = GetTotalExperience();
        EmitSignal(SignalName.EnhancedPersona, totalExperience);

        foreach (EnhancementMaterialCounter materialCounter in _materialCounters.GetChildren())
        {
            materialCounter.Reset();
            _materials[materialCounter.Material] = 0;

            string materialName = $"{materialCounter.Material}";
            int materialCount = _materials[materialCounter.Material];
            int playerMaterialCount = Player.GetMaterialCount(materialName);
            int difference = playerMaterialCount - materialCount;
            Player.SetMaterialCount(materialName, difference);
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

        int totalExperience = GetTotalExperience();
        EmitSignal(SignalName.MaterialsUpdated, totalExperience);
    }

    private int GetTotalExperience()
    {
        int totalExperience = 0;
        foreach (EnhancementMaterial material in _materials.Keys)
        {
            int amount = _materials[material];
            totalExperience += PersonaManager.GetMaterialExperience(material) * amount;
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
