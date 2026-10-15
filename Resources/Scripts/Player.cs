using System;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

namespace PersonaAndPuzzles;

public partial class Player : Node
{
    private static readonly Lazy<Player> s_lazy = new Lazy<Player>(() => new Player());
    public static Player Instance => s_lazy.Value;

    private Player() {}

    public new string Name = "Shardzilla";
    
    public int Stamina = 100;
    public int MaxStamina = 100;

    public int Level = 1;
    public int Experience = 0;
    public int MaxExperience = 100;

    public int KonpakuGems = 1000;

    public int LoadoutIndex;

    private const string _NameKey = "Name";
    private const string _ExperienceKey = "Experience";
    private const string _LevelKey = "Level";
    private const string _StaminaKey = "Stamina";
    private const string _MaxStaminaKey = "MaxStamina";
    private const string _KonpakuGemsKey = "KonpakuGems";
    private const string _MaterialsKey = "Materials";
    private const string _AmountKey = "Amount";

    private Dictionary<string, int> _Materials = new Dictionary<string, int>()
    {
        { $"{EnhancementMaterial.Memory}", 1000 },
        { $"{EnhancementMaterial.Anecdote}", 1000 },
        { $"{EnhancementMaterial.Legend}", 1000 }
    };

    public bool CanLevelUp()
    {
        return Experience >= MaxExperience;
    }

    public void LevelUp()
    {
        const int LinearGrowth = 10;
        const float ExponentialGrowth = 0.1f;

        int levels = 0;
        while (Experience >= MaxExperience)
        {
            levels++;
            Experience -= MaxExperience;

            MaxExperience += LinearGrowth;
            MaxExperience = Mathf.RoundToInt(MaxExperience * ExponentialGrowth);
        }

        Level += levels;
    }

    public void Reset()
    {
        Stamina = 100;
        Level = 1;
        Experience = 0;
    }

    public int GetMaterialCount(string materialName)
    {
        return _Materials[materialName];
    }

    public void SetMaterialCount(string materialName, int count)
    {
        _Materials[materialName] = Mathf.Max(0, count);
    }

    public string GetErrorMessage(VelvetTrial velvetTrial)
    {
        string errorMessage = "";
        if (velvetTrial != null)
        {
            if (Stamina < velvetTrial.Stamina)
            {
                errorMessage = "Not Enough Stamina";
            }
        }
        
        if (PersonaManager.Instance.IsOverPersonaCount())
        {
            errorMessage = "Too Many Personas In Compendium";
        }

        if (PersonaManager.Instance.IsLoadoutEmpty())
        {
            errorMessage = "At Least One Persona Is Needed";
        }

        return errorMessage;
    }

    public void ResetStats()
    {
        Level = 1;
        Experience = 0;
        MaxExperience = 100;
        MaxStamina = 100;
    }

    public Dictionary<string, Variant> GetData()
    {
        return new Dictionary<string, Variant>()
        {
            { _NameKey, Name },
            { _ExperienceKey, Experience },
            { _LevelKey, Level },
            { _StaminaKey, Stamina },
            { _MaxStaminaKey, MaxStamina },
            { _KonpakuGemsKey, KonpakuGems },
            { _MaterialsKey, GetMaterialsData() }
        };
    }

    private Array<Variant> GetMaterialsData()
    {
        Array<Variant> materialsData = new Array<Variant>();
        foreach (string material in _Materials.Keys)
        {
            Dictionary<string, Variant> materialData = new Dictionary<string, Variant>()
            {
                { _NameKey, material },  
                { _AmountKey, _Materials[material] }
            };
            materialsData.Add(materialData);
        }
        return materialsData;
    }

    public async Task SetData(Dictionary<string, Variant> data)
    {
        try
        {
            Name = data[_NameKey].As<string>();
            Experience = data[_ExperienceKey].As<int>();
            Level = data[_LevelKey].As<int>();
            Stamina = data[_StaminaKey].As<int>();
            MaxStamina = data[_MaxStaminaKey].As<int>();
            KonpakuGems = data[_KonpakuGemsKey].As<int>();

            Array<Variant> materialsData = data[_MaterialsKey].As<Array<Variant>>();
            SetMaterialsData(materialsData);

            string message = "Player Data Successfully Loaded";
            PrintRich.PrintLine(message, TextColor.Purple);
        }
        catch (Exception exception)
        {
            GD.PrintErr(exception);
            GD.Print();
            
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = "Player Data Couldn't Be Loaded";
            PrintRich.PrintError(className, message);
        }
    }

    private void SetMaterialsData(Array<Variant> materialsData)
    {
        foreach (Dictionary<string, Variant> materialData in materialsData)
        {
            string materialName = materialData[_NameKey].As<string>();
            int materialAmount = materialData[_AmountKey].As<int>();
            _Materials[materialName] = materialAmount;
        }
    }
}
