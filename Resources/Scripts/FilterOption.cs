using Godot;

namespace PersonaAndPuzzles;

public partial class FilterOption : VBoxContainer
{
    [Signal]
    public delegate void SelectedEventHandler(FilterOption filterOption);

    [Export]
    protected CustomButton Button;

    [Export]
    private Label _label;

    public string[] States;
    
    protected string State;

    protected int Index = 0;
    protected Color OriginalModulate;
    protected const float DarkAmount = 0.2f;

    protected const string OffState = "Off";
    protected const string OnState = "On";

    public override void _Ready()
    {
        SetStates();

        State = States[Index];

        _label.Text = Name;
        OriginalModulate = SelfModulate;

        Button.Pressed += OnPressed;

        CompendiumFilter compendiumFilter = GetParent().GetOwner().GetOwner<CompendiumFilter>();
        compendiumFilter.Connect(CompendiumFilter.SignalName.Unfiltered, 
            Callable.From(Unfilter));
    }

    public void Filter()
    {
        Index = 1;
        State = States[Index];
        Modulate = OriginalModulate.Darkened(DarkAmount);
    }

    public virtual void Unfilter()
    {
        Index = 0;
        State = States[Index];
        Modulate = OriginalModulate;
    }

    protected virtual void OnPressed()
    {
        SetState();

        if (State == OnState)
        {
            EmitSignal(SignalName.Selected, this);
            Modulate = OriginalModulate.Darkened(DarkAmount);
        }
        else if (State == OffState)
        {
            Name = _label.Text;
            Modulate = OriginalModulate;
        }
    }

    // Increase the index and set the state
    protected virtual void SetState()
    {
        Index++;
        if (Index > States.Length - 1)
        {
            Index = 0;
        }

        State = States[Index];
    }

    protected virtual void SetStates()
    {
        States = [ OffState, OnState ];
    }
}
