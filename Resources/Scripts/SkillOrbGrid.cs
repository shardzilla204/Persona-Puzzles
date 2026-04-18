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
    private bool _isDragging = false;

    private List<List<SkillOrb>> _skillOrbs = new List<List<SkillOrb>>();

    private Container _skillOrbContainer;

    public override void _Ready()
    {
        SkillOrb skillOrb = SkillOrbManager.GetRandomSkillOrb();
        _offset = (Vector2I) skillOrb.Size;
        _swapMargin = skillOrb.Size.X / 2;
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

        _skillOrbs[targetPos.X][targetPos.Y] = currentOrb;
        _skillOrbs[currentPos.X][currentPos.Y] = targetOrb;

        PrintRich.Print($"Swapped {currentOrb.SkillType} ({currentPos}) with {targetOrb.SkillType} ({targetPos})", TextColor.Yellow);
    }

    private void TweenSwap(Vector2I currentPos, Vector2I targetPos)
    {
        SkillOrb currentOrb = GetSkillOrb(currentPos);
        SkillOrb targetOrb = GetSkillOrb(targetPos);

        Vector2 currentPixelPos = GridToPixel(currentPos.X, currentPos.Y);
        Vector2 targetPixelPos = GridToPixel(targetPos.X, targetPos.Y);

        float duration = 0.25f;
        Tween tween = CreateTween().SetParallel().SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(currentOrb, "position", currentPixelPos, duration);
        tween.TweenProperty(targetOrb, "position", targetPixelPos, duration);
    }

    private void SwapPositions(Vector2I currentPos, Vector2I currentPixelPos, Vector2I targetPos, Vector2I targetPixelPos)
    {
        _skillOrbs[targetPos.X][targetPos.Y].Position = targetPixelPos;
        _skillOrbs[currentPos.X][currentPos.Y].Position = currentPixelPos;
    }
}
