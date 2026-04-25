using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonaAndPuzzles;

public partial class SkillOrbGrid : Control
{
    [Export]
    private int _columns;

    [Export]
    private int _rows;

    [Export]
    private int _refillYOffset = 1;

    [Export]
    private Control _comboLabelContainer;

    [Export]
    private Timer _refillTimer;

    private const int _Offset = 64;
    private const float _ComboIncrement = 0.1f;

    private Vector2 _firstTouch = new Vector2();
    private Vector2 _finalTouch = new Vector2();
    private bool _isDragging = false;

    private List<List<SkillOrb>> _skillOrbs = new List<List<SkillOrb>>();
    private List<ComboLabel> _comboLabels = new List<ComboLabel>();

    private Container _skillOrbContainer;

    // Keys
    private string _indexesKey = "Indexes";
    private string _typeKey = "Type";
    private string _positionKey = "Position";
    private string _countKey = "Count";

    public override void _Ready()
    {
        _refillTimer.Timeout += RefillTimerTimeout;

        CreateGrid();
    }

    public override void _Process(double delta)
    {
        TouchInput();
        SwapInput();
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (data.As<SkillOrb>() is SkillOrb skillOrb)
        {
            skillOrb.Modulate = Colors.White;
        }
    }

    private void CreateGrid()
    {
        for (int i = 0; i < _columns; i++)
        {
            List<SkillOrb> row = new List<SkillOrb>();
            for (int j = 0; j < _rows; j++)
            {
                SkillOrb skillOrb = null;
                row.Add(skillOrb);
            }
            _skillOrbs.Add(row);
        }

        FillGrid();
    }

    private void FillGrid()
    {
        for (int i = 0; i < _columns; i++)
        {
            for (int j = 0; j < _rows; j++)
            {
                SkillOrb skillOrb = SkillOrbManager.GetRandomSkillOrb();
                int loops = 0;
                while (HasMatch(i, j, skillOrb.SkillType) && loops < 100)
                {
                    loops++;
                    skillOrb = SkillOrbManager.GetRandomSkillOrb();
                }

                AddChild(skillOrb);
                _skillOrbs[i][j] = skillOrb;
                skillOrb.Position = GridToPixel(i, j);
            }
        }
    }

    private bool HasMatch(int x, int y, SkillType skillType)
    {
        SkillOrb secondOrb = null;
        SkillOrb thirdOrb = null;
        if (x > 1)
        {
            secondOrb = _skillOrbs[x - 1][y];
            thirdOrb = _skillOrbs[x - 2][y];
        } 
        
        if (y > 1)
        {
            secondOrb = _skillOrbs[x][y - 1];
            thirdOrb = _skillOrbs[x][y - 2];
        }

        if (secondOrb == null || thirdOrb == null) return false;
        if (secondOrb.SkillType != skillType || thirdOrb.SkillType != skillType) return false;

        return true;
    }

    private SkillOrb GetSkillOrb(Vector2I coordinate)
    {
        return _skillOrbs[coordinate.X][coordinate.Y];
    }

    private bool IsInGrid(float pixelX, float pixelY)
    {
        return pixelX >= 0 && 
            pixelX < _columns &&
            pixelY >= 0 && 
            pixelY < _rows;
    }

    private Vector2I GridToPixel(int column, int row)
    {
        int newX = _Offset * column;
        int newY = _Offset * row;

        Vector2I newPosition = new Vector2I(newX, newY);
        return newPosition;
    }

    private Vector2I PixelToGrid(float pixelX, float pixelY)
    {
        int newX = Mathf.RoundToInt(Mathf.FloorToInt(pixelX / _Offset));
        int newY = Mathf.RoundToInt(Mathf.FloorToInt(pixelY / _Offset));

        Vector2I newPosition = new Vector2I(newX, newY);
        return newPosition;
    }

    private void TouchInput()
    {
        if (Input.IsActionJustPressed("Touch"))
        {
            _firstTouch = GetLocalMousePosition();
            Vector2I position = PixelToGrid(_firstTouch.X, _firstTouch.Y);

            bool isInGrid = IsInGrid(position.X, position.Y);
            if (!isInGrid) return;
            
            _isDragging = true;
        }
        else if (Input.IsActionJustReleased("Touch"))
        {
            ReleasedTouch();
        }
    }

    private void ReleasedTouch()
    {
        _finalTouch = GetLocalMousePosition();

        if (!_isDragging) return;
        
        _isDragging = false;

        FindMatches();
    }

    private void SwapInput()
    {
        if (!_isDragging) return;

        Vector2 mousePosition = GetLocalMousePosition();
        Vector2 mouseGridPosition = PixelToGrid(mousePosition.X, mousePosition.Y);
        bool isMouseInGrid = IsInGrid(mouseGridPosition.X, mouseGridPosition.Y);

        if (!isMouseInGrid) 
        {
            GetViewport().GuiCancelDrag();
            ReleasedTouch();
            return;
        }

        Vector2I position = PixelToGrid(_firstTouch.X, _firstTouch.Y);
        Vector2I targetPosition = PixelToGrid(mousePosition.X, mousePosition.Y);

        if (position == targetPosition) return;

        SwapOrbs(position, targetPosition);
        TweenSwap(position, targetPosition);

        _firstTouch = GetLocalMousePosition();
    }

    private void SwapOrbs(Vector2I position, Vector2I targetPosition)
    {
        SkillOrb orb = GetSkillOrb(position);
        SkillOrb targetOrb = GetSkillOrb(targetPosition);

        if (orb == null || targetOrb == null) return;

        _skillOrbs[position.X][position.Y] = targetOrb;
        _skillOrbs[targetPosition.X][targetPosition.Y] = orb;
    }

    private void TweenSwap(Vector2I position, Vector2I targetPosition)
    {
        SkillOrb orb = GetSkillOrb(position);
        SkillOrb targetOrb = GetSkillOrb(targetPosition);

        if (orb == null || targetOrb == null) return;

        Vector2I pixelPosition = GridToPixel(position.X, position.Y);
        Vector2I targetPixelPosition = GridToPixel(targetPosition.X, targetPosition.Y);

        TweenPosition(orb, pixelPosition);
        TweenPosition(targetOrb, targetPixelPosition);
    }

    private void TweenPosition(SkillOrb orb, Vector2 targetPosition)
    {
        float duration = 0.35f;
        Tween tween = CreateTween().SetParallel().SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(orb, "position", targetPosition, duration);
    }

    private void SwapPositions(Vector2I position, Vector2I pixelPosition, Vector2I targetPosition, Vector2I targetPixelPosition)
    {
        _skillOrbs[position.X][position.Y].Position = pixelPosition;
        _skillOrbs[targetPosition.X][targetPosition.Y].Position = targetPixelPosition;
    }
    
    private async void FindMatches()
    {
        List<GC.Dictionary<string, Variant>> verticalMatches = FindVerticalMatches();
        List<GC.Dictionary<string, Variant>> horizontalMatches = FindHorizontalMatches();
        List<GC.Dictionary<string, Variant>> orbSets = CreateOrbSets(verticalMatches, horizontalMatches);

        if (orbSets.Count <= 0) 
        {
            float multiplier = 1;
            foreach (ComboLabel comboLabel in _comboLabels)
            {
                comboLabel.TweenExit();
                multiplier += _ComboIncrement;

                await ToSignal(comboLabel, Node.SignalName.TreeExited);
            }
            _comboLabels.Clear();
            return;
        }

        List<GC.Dictionary<string, Variant>> combos = MergeOrbSets(orbSets);
        combos.OrderBy(combo => combo[_indexesKey].As<GC.Array<Vector2I>>());

        await RunCombosAsync(combos);

        CollapseColumns();
    }

    private async Task RunCombosAsync(List<GC.Dictionary<string, Variant>> combos)
    {
        for (int i = 0; i < combos.Count; i++)
        {
            GC.Array<Vector2I> indexes = combos[i][_indexesKey].As<GC.Array<Vector2I>>();
            ComboLabel comboLabel = await TweenOrbSetAsync(_comboLabels.Count + 1, indexes);
            _comboLabels.Add(comboLabel);

            DestroyMatched();
        }
    }

    private async Task<ComboLabel> TweenOrbSetAsync(int comboCount, GC.Array<Vector2I> indexes)
    {
        float duration = 0.5f;
        foreach (Vector2I index in indexes)
        {
            SkillOrb skillOrb = GetSkillOrb(index);
            skillOrb.IsMatched = true;

            Color targetModulate = Colors.White;
            targetModulate.A = 0;

            Tween tween = CreateTween();
            tween.TweenProperty(skillOrb, "modulate", targetModulate, duration);
        }

        // Use the "center" the pattern
        Vector2I labelPosition = indexes[1];
        ComboLabel comboLabel = GetComboLabel(comboCount, labelPosition.X, labelPosition.Y);
        _comboLabelContainer.AddChild(comboLabel);

        await ToSignal(GetTree().CreateTimer(duration), SceneTreeTimer.SignalName.Timeout);

        return comboLabel;
    }

    // Creates match indexes from match sets
    private List<GC.Dictionary<string, Variant>> CreateOrbSets(List<GC.Dictionary<string, Variant>> verticalMatches, List<GC.Dictionary<string, Variant>> horizontalMatches)
    {
        List<GC.Dictionary<string, Variant>> matchSets = new List<GC.Dictionary<string, Variant>>();

        // Create vertical match indexes
        // Uses Y as the starting position. Y = Row position
        foreach (GC.Dictionary<string, Variant> match in verticalMatches)
        {
            SkillType matchType = (SkillType) match[_typeKey].As<int>();
            Vector2I matchPosition = match[_positionKey].As<Vector2I>();
            int matchCount = match[_countKey].As<int>();

            GC.Array<Vector2I> matchIndexes = new GC.Array<Vector2I>();
            for (int i = matchPosition.Y; i < matchCount + matchPosition.Y; i++)
            {
                Vector2I matchIndex = new Vector2I(matchPosition.X, i);
                matchIndexes.Add(matchIndex);
            }

            GC.Dictionary<string, Variant> matchSet = new GC.Dictionary<string, Variant>
            {
                { _typeKey, (int) matchType },
                { _indexesKey, matchIndexes }
            };

            matchSets.Add(matchSet);
        }
        
        // Create horizontal match indexes
        // Uses X for the starting position. X = Column position
        foreach (GC.Dictionary<string, Variant> match in horizontalMatches)
        {
            SkillType matchType = (SkillType) match[_typeKey].As<int>();
            Vector2I matchPosition = match[_positionKey].As<Vector2I>();
            int matchCount = match[_countKey].As<int>();

            GC.Array<Vector2I> matchIndexes = new GC.Array<Vector2I>();
            for (int i = matchPosition.X; i < matchCount + matchPosition.X; i++)
            {
                Vector2I matchIndex = new Vector2I(i, matchPosition.Y);
                matchIndexes.Add(matchIndex);
            }

            GC.Dictionary<string, Variant> matchSet = new GC.Dictionary<string, Variant>
            {
                { _typeKey, (int) matchType },
                { _indexesKey, matchIndexes }
            };

            matchSets.Add(matchSet);
        }

        return matchSets;
    }

    private List<SkillOrb> GetColumn(int columnIndex)
    {
        List<SkillOrb> column = _skillOrbs[columnIndex];
        PrintRich.PrintOrbs($"Column ({columnIndex}) Index: ", column);
        return column;
    }

    private List<SkillOrb> GetRow(int rowIndex)
    {
        List<SkillOrb> row = new List<SkillOrb>();
        for (int i = 0; i < _columns; i++)
        {
            SkillOrb orb = _skillOrbs[i][rowIndex];
            row.Add(orb);
        }
        PrintRich.PrintOrbs($"Row ({rowIndex}) Index: ", row);
        return row;
    }

    // Goes through each column in the grid and find matches of 3+
    private List<GC.Dictionary<string, Variant>> FindVerticalMatches()
    {
        List<GC.Dictionary<string, Variant>> matches = new List<GC.Dictionary<string, Variant>>();
        int count = 1;

        for (int i = 0; i < _columns; i++)
        {
            SkillOrb previousSkillOrb = new SkillOrb();
            List<SkillOrb> skillOrbs = GetColumn(i);

            // ! DO NOT REMOVE
            // Acts as the end to the loop
            skillOrbs.Add(new SkillOrb());

            foreach (SkillOrb skillOrb in skillOrbs)
            {
                if (skillOrb.SkillType != previousSkillOrb.SkillType)
                {
                    if (count >= 3)
                    {
                        GC.Dictionary<string, Variant> match = GetMatchInformation(previousSkillOrb, count);
                        matches.Add(match);
                    }
                    previousSkillOrb = skillOrb;
                    count = 1;
                }
                else if (skillOrb.SkillType != SkillType.None)
                {
                    count++;
                }
            }
        }
        return matches;
    }

    // Goes through each row in the grid and find matches of 3+
    private List<GC.Dictionary<string, Variant>> FindHorizontalMatches()
    {
        List<GC.Dictionary<string, Variant>> matches = new List<GC.Dictionary<string, Variant>>();
        int count = 1;

        for (int i = 0; i < _rows; i++)
        {
            SkillOrb previousSkillOrb = new SkillOrb();
            List<SkillOrb> skillOrbs = GetRow(i);

            // ! DO NOT REMOVE
            // Acts as the end to the loop
            skillOrbs.Add(new SkillOrb());

            foreach (SkillOrb skillOrb in skillOrbs)
            {
                if (skillOrb.SkillType != previousSkillOrb.SkillType)
                {
                    if (count >= 3)
                    {
                        GC.Dictionary<string, Variant> match = GetMatchInformation(previousSkillOrb, count);
                        matches.Add(match);
                    }
                    previousSkillOrb = skillOrb;
                    count = 1;
                }
                else if (skillOrb.SkillType != SkillType.None)
                {
                    count++;
                }
            }
        }
        return matches;
    }

    private GC.Dictionary<string, Variant> GetMatchInformation(SkillOrb skillOrb, int count)
    {
        Vector2I gridPosition = PixelToGrid(skillOrb.Position.X, skillOrb.Position.Y);
        GC.Dictionary<string, Variant> match = new GC.Dictionary<string, Variant>
        {
            { _typeKey, (int) skillOrb.SkillType },
            { _positionKey, gridPosition },
            { _countKey, count }
        };
        return match;
    }

    private List<GC.Dictionary<string, Variant>> MergeOrbSets(List<GC.Dictionary<string, Variant>> orbSets)
    {
        List<GC.Dictionary<string, Variant>> mergedOrbSets = new List<GC.Dictionary<string, Variant>>();

        GC.Dictionary<string, Variant> currentOrbSet;
        while (orbSets.Count > 0)
        {
            currentOrbSet = orbSets.First();
            orbSets.Remove(currentOrbSet);
            (GC.Dictionary<string, Variant> MergedOrbSet, List<GC.Dictionary<string, Variant>> OrbSets) result = FindSameTypeOrbSet(currentOrbSet, orbSets);
            mergedOrbSets.Add(result.MergedOrbSet);
            orbSets = result.OrbSets;
        }

        return mergedOrbSets;
    }

    private (GC.Dictionary<string, Variant>, List<GC.Dictionary<string, Variant>>) FindSameTypeOrbSet(GC.Dictionary<string, Variant> currentOrbSet, List<GC.Dictionary<string, Variant>> orbSets)
    {
        List<GC.Dictionary<string, Variant>> newOrbSets = new List<GC.Dictionary<string, Variant>>();
        foreach (GC.Dictionary<string, Variant> orbSet in orbSets)
        {
            bool areNeigboringSameTypeBallSets = AreNeigboringSameTypeOrbSets(currentOrbSet, orbSet);
            if (areNeigboringSameTypeBallSets)
            {
                GC.Array<Vector2I> currentOrbSetIndexes = currentOrbSet[_indexesKey].As<GC.Array<Vector2I>>();
                GC.Array<Vector2I> orbSetIndexes = orbSet[_indexesKey].As<GC.Array<Vector2I>>();
                foreach (Vector2I index in orbSetIndexes)
                {
                    if (!currentOrbSetIndexes.Contains(index)) currentOrbSetIndexes.Add(index);
                }
            }
            else
            {
                newOrbSets.Add(orbSet);
            }
        }

        return (currentOrbSet, newOrbSets);
    }
    
    private bool AreNeigboringSameTypeOrbSets(GC.Dictionary<string, Variant> orbSetA, GC.Dictionary<string, Variant> orbSetB)
    {
        SkillType orbSetTypeA = (SkillType) orbSetA[_typeKey].As<int>();
        SkillType orbSetTypeB = (SkillType) orbSetB[_typeKey].As<int>();

        if (orbSetTypeA != orbSetTypeB) return false;

        GC.Array<Vector2I> orbSetAIndexes = orbSetA[_indexesKey].As<GC.Array<Vector2I>>();
        GC.Array<Vector2I> orbSetBIndexes = orbSetB[_indexesKey].As<GC.Array<Vector2I>>();

        for (int i = 0; i < orbSetAIndexes.Count; i++)
        {
            for (int j = 0; j < orbSetBIndexes.Count; j++)
            {
                Vector2I orbSetAIndex = orbSetAIndexes[i];
                Vector2I orbSetBIndex = orbSetBIndexes[j];

                float distance = orbSetAIndex.DistanceTo(orbSetBIndex);
                if (distance <= 1) return true;
            }
        }
        return false;
    }

    private void SetMatches(SkillOrb orb, SkillOrb previousOrb, SkillOrb nextOrb)
    {
        if (previousOrb != null && nextOrb != null && 
            previousOrb.SkillType == orb.SkillType && nextOrb.SkillType == orb.SkillType)
        {
            orb.SetMatch(true);
            previousOrb.SetMatch(true);
            nextOrb.SetMatch(true);
        }
    }

    private void DestroyMatched()
    {
        for (int i = 0; i < _columns; i++)
        {
            for (int j = 0; j < _rows; j++)
            {
                SkillOrb skillOrb = _skillOrbs[i][j];
                if (_skillOrbs[i][j] == null || !skillOrb.IsMatched) continue;

                skillOrb.QueueFree();
                _skillOrbs[i][j] = null;
            }
        }

        PrintRich.Print("Destroyed Matches", TextColor.Yellow);
    }

    // Grid is created from top to bottom, left to right. Starting from the top left corner.
    private void CollapseColumns()
    {
        for (int i = _columns - 1; i >= 0; i--)
        {
            for (int j = _rows - 1; j >= 0; j--)
            {
                SkillOrb skillOrb = _skillOrbs[i][j];
                if (skillOrb != null) continue;

                for (int k = j - 1; k >= 0; k--)
                {
                    SkillOrb aboveSkillOrb = _skillOrbs[i][k];
                    if (aboveSkillOrb == null) continue;
                    
                    Vector2I targetPos = GridToPixel(i, j);
                    TweenPosition(aboveSkillOrb, targetPos);

                    _skillOrbs[i][j] = aboveSkillOrb;
                    _skillOrbs[i][k] = null;
 
                    break;
                }
            }
        }
        
        PrintRich.Print("Columns Collapsed", TextColor.Yellow);

        _refillTimer.Start();
    }

    private void RefillColumns()
    {
        for (int i = 0; i < _columns; i++)
        {
            for (int j = 0; j < _rows; j++)
            {
                SkillOrb skillOrb = _skillOrbs[i][j];
                if (skillOrb != null) continue;
                
                SkillOrb randomSkillOrb = SkillOrbManager.GetRandomSkillOrb();
                int loops = 0;
                while (HasMatch(i, j, randomSkillOrb.SkillType) && loops < 100)
                {
                    loops++;
                    randomSkillOrb = SkillOrbManager.GetRandomSkillOrb();
                }

                AddChild(randomSkillOrb);
                randomSkillOrb.Position = GridToPixel(i, j - _refillYOffset);
                
                _comboLabelContainer.MoveToFront();

                _skillOrbs[i][j] = randomSkillOrb;
                Vector2I targetPosition = GridToPixel(i, j);
                TweenPosition(randomSkillOrb, targetPosition);

            }
        }

        PrintRich.Print("Columns Refilled", TextColor.Yellow);

        FindMatches();
    }

    private ComboLabel GetComboLabel(int comboCount, int x, int y)
    {
        ComboLabel comboLabel = PersonaAndPuzzles.PackedScenes.GetComboLabel(comboCount);
        comboLabel.Position = GridToPixel(x, y);
        return comboLabel;
    }

    private void RefillTimerTimeout()
    {
        RefillColumns();
    }
}
