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

    private Vector2 _offset;
    private float _swapMargin;

    private Vector2 _firstTouch = new Vector2();
    private Vector2 _finalTouch = new Vector2();
    private bool _isControlling = false;
    private SkillOrb _currentSkillOrb;

    private List<List<SkillOrb>> _skillOrbs = new List<List<SkillOrb>>();

    private Container _skillOrbContainer;

    public override void _Ready()
    {
        SkillOrb skillOrb = SkillOrbManager.GetRandomSkillOrb();
        _offset = (Vector2I) skillOrb.Size;
        _swapMargin = skillOrb.Size.X;
        SpawnSkillOrbs();
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

    private void SpawnSkillOrbs()
    {
        for (int i = 0; i < _columns; i++)
        {
            List<SkillOrb> row = new List<SkillOrb>();
            for (int j = 0; j < _rows; j++)
            {
                SkillOrb skillOrb = SkillOrbManager.GetRandomSkillOrb();
                AddChild(skillOrb);
                row.Add(skillOrb);
                skillOrb.Position = GridToPixel(i, j);
            }
            _skillOrbs.Add(row);
        }
    }

    private SkillOrb GetSkillOrb(int columnIndex, int rowIndex)
    {
        List<SkillOrb> row = _skillOrbs[columnIndex];
        SkillOrb skillOrb = row[rowIndex];
        return skillOrb;
    }

    private bool IsInGrid(float pixelX, float pixelY)
    {
        return pixelX >= 0 && 
            pixelX < _columns &&
            pixelY >= 0 && 
            pixelY < _rows;
    }

    private Vector2 GridToPixel(int column, int row)
    {
        int newX = (int) (_offset.X * column);
        int newY = (int) (_offset.Y * row);

        Vector2 newPosition = new Vector2(newX, newY);
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
            Vector2 gridPosition = PixelToGrid(_firstTouch.X, _firstTouch.Y);
            bool isInGrid = IsInGrid(gridPosition.X, gridPosition.Y);
            if (isInGrid) 
            {
                _isControlling = true;
                _currentSkillOrb = GetSkillOrb((int) gridPosition.X, (int) gridPosition.Y);
            }
        }
        else if (Input.IsActionJustReleased("Touch"))
        {
            _finalTouch = GetLocalMousePosition();
            Vector2 gridPosition = PixelToGrid(_finalTouch.X, _finalTouch.Y);
            if (_isControlling)
            {
                _isControlling = false;
                GD.Print("Done");
                GD.Print($"Final Position: {gridPosition}");
                _currentSkillOrb = null;
            }
        }
    }

    private void SwapInput()
    {
        Vector2 mousePosition = GetLocalMousePosition();
        Vector2 mouseGridPosition = PixelToGrid(mousePosition.X, mousePosition.Y);
        bool isMouseInGrid = IsInGrid(mouseGridPosition.X, mouseGridPosition.Y);

        Vector2 mouseDirection = GetMouseDirection();
        if (mouseDirection == Vector2.Zero || !isMouseInGrid) return;
        
        Vector2 gridPosition = PixelToGrid(_firstTouch.X, _firstTouch.Y);

        Vector2 targetPosition = Vector2.Zero;
        SkillOrb targetSkillOrb = null;

        if (mouseDirection == Vector2.Up)
        {
            targetPosition = gridPosition + Vector2.Up;
            targetSkillOrb = GetSkillOrb((int) targetPosition.X, (int) targetPosition.Y);

            SwapOrbs(gridPosition, _currentSkillOrb, targetPosition, targetSkillOrb);

            GD.Print("Moving Up");
        }
        else if (mouseDirection == Vector2.Down)
        {
            targetPosition = gridPosition + Vector2.Down;
            targetSkillOrb = GetSkillOrb((int) targetPosition.X, (int) targetPosition.Y);

            SwapOrbs(gridPosition, _currentSkillOrb, targetPosition, targetSkillOrb);

            GD.Print("Moving Down");
        }
        else if (mouseDirection == Vector2.Left)
        {
            targetPosition = gridPosition + Vector2.Left;
            targetSkillOrb = GetSkillOrb((int) targetPosition.X, (int) targetPosition.Y);

            SwapOrbs(gridPosition, _currentSkillOrb, targetPosition, targetSkillOrb);

            GD.Print("Moving Left");
        }
        else if (mouseDirection == Vector2.Right)
        {
            targetPosition = gridPosition + Vector2.Right;
            targetSkillOrb = GetSkillOrb((int) targetPosition.X, (int) targetPosition.Y);

            SwapOrbs(gridPosition, _currentSkillOrb, targetPosition, targetSkillOrb);

            GD.Print("Moving Right");
        }

        // _firstTouch = GetLocalMousePosition();

        gridPosition = GridToPixel((int) gridPosition.X, (int) gridPosition.Y);
        targetPosition = GridToPixel((int) targetPosition.X, (int) targetPosition.Y);
        TweenSwap(gridPosition, _currentSkillOrb, targetPosition, targetSkillOrb);

        GD.Print($"Current Position: {gridPosition} - {_currentSkillOrb.SkillType}");
        GD.Print($"Target Position: {targetPosition} - {targetSkillOrb.SkillType}");
    }

    private void SwapOrbs(Vector2 currentPosition, SkillOrb currentSkillOrb, Vector2 targetPosition, SkillOrb targetSkillOrb)
    {
        List<SkillOrb> currentRow = _skillOrbs[(int) currentPosition.X];
        int currentIndex = currentRow.IndexOf(currentSkillOrb);
        currentRow.RemoveAt(currentIndex);
        currentRow.Insert(currentIndex, targetSkillOrb);

        List<SkillOrb> targetRow = _skillOrbs[(int) targetPosition.X];
        int targetIndex = targetRow.IndexOf(targetSkillOrb);
        targetRow.RemoveAt(targetIndex);
        targetRow.Insert(targetIndex, currentSkillOrb);
    }

    private void PrintGrid()
    {
        for (int i = 0; i < _columns; i++)
        {
            List<SkillOrb> row = _skillOrbs[i];
            for (int j = 0; j < _rows; j++)
            {
                SkillOrb skillOrb = row[j];
                PrintSkillOrbPosition(skillOrb, i, j);
            }
        }
    }

    private void PrintSkillOrbPosition(SkillOrb skillOrb, int positionX, int positionY)
    {
        GD.Print($"Skill Orb: {skillOrb.SkillType} | [{positionX}, {positionY}]");
    }

    private void TweenSwap(Vector2 currentPosition, SkillOrb currentSkillOrb, Vector2 targetPosition, SkillOrb targetSkillOrb)
    {
        // GD.Print($"Current: {currentPosition}");
        // GD.Print($"Target: {targetPosition}");

        float duration = 0.25f;
        Tween tween = CreateTween().SetParallel().SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(currentSkillOrb, "position", targetPosition, duration);
        tween.TweenProperty(targetSkillOrb, "position", currentPosition, duration);
    }

    private Vector2 GetMouseDirection()
    {
        if (_isControlling)
        {
            Vector2 mousePosition = GetLocalMousePosition();
            Vector2 direction = _firstTouch.DirectionTo(mousePosition).Round();
            GD.Print($"[{direction.X}, {direction.Y}]");
            float distance = _firstTouch.DistanceTo(mousePosition);

            return _swapMargin <= distance ? direction : Vector2.Zero;
            // return direction;
        }

        return Vector2.Zero;
    }
}
