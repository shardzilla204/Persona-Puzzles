using Godot;
using System;
using System.Collections.Generic;

namespace PersonaAndPuzzles;

public partial class SkillOrbGrid : Control
{
    [Export]
    private int _columns;

    [Export]
    private int _rows;

    [Export]
    private Timer _destroyTimer;

    [Export]
    private Timer _collapseTimer;

    [Export]
    private Timer _refillTimer;

    private Vector2 _offset;
    private float _swapMargin;

    private Vector2 _firstTouch = new Vector2();
    private Vector2 _finalTouch = new Vector2();
    private bool _isDragging = false;

    private List<List<SkillOrb>> _skillOrbs = new List<List<SkillOrb>>();

    private Container _skillOrbContainer;

    public override void _Ready()
    {
        _destroyTimer.Timeout += DestroyTimerTimeout;
        _collapseTimer.Timeout += CollapseTimerTimeout;
        _refillTimer.Timeout += RefillTimerTimeout;

        SkillOrb skillOrb = SkillOrbManager.GetRandomSkillOrb();
        _offset = (Vector2I) skillOrb.Size;
        _swapMargin = skillOrb.Size.X / 2;

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
        int newX = (int) (_offset.X * column);
        int newY = (int) (_offset.Y * row);

        Vector2I newPosition = new Vector2I(newX, newY);
        return newPosition;
    }

    private Vector2I PixelToGrid(float pixelX, float pixelY)
    {
        int newX = Mathf.FloorToInt(pixelX / _offset.X);
        int newY = Mathf.FloorToInt(pixelY / _offset.Y);

        Vector2I newPosition = new Vector2I(newX, newY);
        return newPosition;
    }

    private void TouchInput()
    {
        if (Input.IsActionJustPressed("Touch"))
        {
            _firstTouch = GetLocalMousePosition();
            Vector2I currentPos = PixelToGrid(_firstTouch.X, _firstTouch.Y);

            bool isInGrid = IsInGrid(currentPos.X, currentPos.Y);
            if (!isInGrid) return;
            
            _isDragging = true;
        }
        else if (Input.IsActionJustReleased("Touch"))
        {
            _finalTouch = GetLocalMousePosition();

            if (!_isDragging) return;
            
            _isDragging = false;

            FindMatches();
        }
    }

    private void SwapInput()
    {
        if (!_isDragging) return;

        Vector2 mousePos = GetLocalMousePosition();
        Vector2 mouseGridPos = PixelToGrid(mousePos.X, mousePos.Y);
        bool isMouseInGrid = IsInGrid(mouseGridPos.X, mouseGridPos.Y);

        if (!isMouseInGrid) 
        {
            GetViewport().GuiCancelDrag();
            _finalTouch = GetLocalMousePosition();
            _isDragging = false;
            return;
        }

        Vector2I currentPos = PixelToGrid(_firstTouch.X, _firstTouch.Y);
        Vector2I targetPos = PixelToGrid(mousePos.X, mousePos.Y);

        if (currentPos == targetPos) return;

        SwapOrbs(currentPos, targetPos);
        TweenSwap(currentPos, targetPos);

        _firstTouch = GetLocalMousePosition();
    }

    private void SwapOrbs(Vector2I currentPos, Vector2I targetPos)
    {
        SkillOrb currentOrb = GetSkillOrb(currentPos);
        SkillOrb targetOrb = GetSkillOrb(targetPos);

        if (currentOrb == null || targetOrb == null) return;

        _skillOrbs[targetPos.X][targetPos.Y] = currentOrb;
        _skillOrbs[currentPos.X][currentPos.Y] = targetOrb;

        PrintRich.Print($"Swapped {currentOrb.SkillType} ({currentPos}) with {targetOrb.SkillType} ({targetPos})", TextColor.Yellow);
    }

    private void TweenSwap(Vector2I currentPos, Vector2I targetPos)
    {
        SkillOrb currentOrb = GetSkillOrb(currentPos);
        SkillOrb targetOrb = GetSkillOrb(targetPos);

        if (currentOrb == null || targetOrb == null) return;

        Vector2I currentPixelPos = GridToPixel(currentPos.X, currentPos.Y);
        Vector2I targetPixelPos = GridToPixel(targetPos.X, targetPos.Y);

        TweenPosition(currentOrb, currentPixelPos);
        TweenPosition(targetOrb, targetPixelPos);
    }

    private void TweenPosition(SkillOrb orb, Vector2 targetPos)
    {
        float duration = 0.35f;
        Tween tween = CreateTween().SetParallel().SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(orb, "position", targetPos, duration);
    }

    private void SwapPositions(Vector2I currentPos, Vector2I currentPixelPos, Vector2I targetPos, Vector2I targetPixelPos)
    {
        _skillOrbs[targetPos.X][targetPos.Y].Position = targetPixelPos;
        _skillOrbs[currentPos.X][currentPos.Y].Position = currentPixelPos;
    }

    private void FindMatches()
    {
        for (int i = 0; i < _columns; i++)
        {
            for (int j = 0; j < _rows; j++)
            {
                SkillOrb currentOrb = _skillOrbs[i][j];
                if (currentOrb == null) continue;
                
                if (i > 0 && i < _columns - 1)
                {
                    SkillOrb previousOrb = _skillOrbs[i - 1][j];
                    SkillOrb nextOrb = _skillOrbs[i + 1][j];

                    if (previousOrb != null && nextOrb != null && 
                        previousOrb.SkillType == currentOrb.SkillType && nextOrb.SkillType == currentOrb.SkillType)
                    {
                        previousOrb.SetMatch(true);
                        currentOrb.SetMatch(true);
                        nextOrb.SetMatch(true);   
                    }
                }
                
                if (j > 0 && j < _rows - 1)
                {
                    SkillOrb previousOrb = _skillOrbs[i][j - 1];
                    SkillOrb nextOrb = _skillOrbs[i][j + 1];

                    if (previousOrb != null && nextOrb != null && 
                        previousOrb.SkillType == currentOrb.SkillType && nextOrb.SkillType == currentOrb.SkillType)
                    {
                        previousOrb.SetMatch(true);
                        currentOrb.SetMatch(true);
                        nextOrb.SetMatch(true);   
                    }
                }
            }
        }

        _destroyTimer.Start();
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

        _collapseTimer.Start();
    }

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
                _skillOrbs[i][j] = randomSkillOrb;
                randomSkillOrb.Position = GridToPixel(i, j);
            }
        }

        PrintRich.Print("Columns Refilled", TextColor.Yellow);
    }

    private void DestroyTimerTimeout()
    {
        DestroyMatched();
    }

    private void CollapseTimerTimeout()
    {
        CollapseColumns();
    }

    private void RefillTimerTimeout()
    {
        RefillColumns();
    }
}
