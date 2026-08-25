using System.Collections.Generic;
using System.Linq;
using Godot;

namespace PersonaAndPuzzles;

public partial class StatTypeFilterOption : FilterOption
{
    [Export]
    private TextureRect _orderTexture;
    
    public StatType StatType;

    public const string AscendingState = "Ascending";
    public const string DescendingState = "Descending";

    public override void _Ready()
    {
        base._Ready();
        SetOrderTexture();
    }

    protected override void OnPressed()
    {
        base.OnPressed();

        if (State == AscendingState)
        {
            Name = $"{StatType}|{AscendingState}";
            EmitSignal(FilterOption.SignalName.Selected, this);
            Modulate = OriginalModulate.Darkened(DarkAmount);
        }
        else if (State == DescendingState)
        {
            Name = $"{StatType}|{DescendingState}";
            EmitSignal(FilterOption.SignalName.Selected, this);
            Modulate = OriginalModulate.Darkened(DarkAmount);
        }

        SetOrderTexture();
    }

    protected override void SetStates()
    {
        States = [ OffState, AscendingState, DescendingState ];
    }

    protected override void SetState()
    {
        Index++;
        if (Index > States.Length - 1)
        {
            Index = 1;
        }

        State = States[Index];
    }

    public void SetState(string stateName)
    {
        List<string> states = States.ToList();
        Index = states.IndexOf(stateName);

        State = States[Index];

        if (State == AscendingState)
        {
            Name = $"{StatType}|{AscendingState}";
            EmitSignal(FilterOption.SignalName.Selected, this);
            Modulate = OriginalModulate.Darkened(DarkAmount);
        }
        else if (State == DescendingState)
        {
            Name = $"{StatType}|{DescendingState}";
            EmitSignal(FilterOption.SignalName.Selected, this);
            Modulate = OriginalModulate.Darkened(DarkAmount);
        }

        SetOrderTexture();
    }

    public override void Unfilter()
    {
        base.Unfilter();

        _orderTexture.Visible = false;
    }

    private void SetOrderTexture()
    {
        _orderTexture.Visible = State != OffState;

        bool isAscending = State == AscendingState;
        Color color = isAscending ? Color.FromHtml("6bb347") /* Green */ : Color.FromHtml("e63948"); /* Red */
        _orderTexture.SelfModulate = color;

        string textureUID = isAscending ? "uid://5q1of6biujk5" /* Up Arrow */ : "uid://bq7je4v5emsu4"; /* Down Arrow */
        Texture2D texture = GD.Load<Texture2D>(textureUID);
        _orderTexture.Texture = texture;
    }
}
