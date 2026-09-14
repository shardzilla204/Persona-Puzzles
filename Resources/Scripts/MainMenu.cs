using Godot;
using System;

namespace PersonaAndPuzzles;

public enum CanvasType
{
    VelvetTrials,
    Compendium,
    Fusion
}

public partial class MainMenu : Control
{
    [Export]
    private MainMenuControls _mainMenuControls;

    private CanvasType _previousCanvasType;

    public override void _Ready()
    {
        _mainMenuControls.ButtonPressed += ShowCanvas;
    }

    private void ShowCanvas(Control canvas, CanvasType canvasType)
    {
        AddChild(canvas);
        _previousCanvasType = canvasType;
    }

    public void ShowPreviousCanvas()
    {
        switch (_previousCanvasType)
        {
            case CanvasType.VelvetTrials:
                _mainMenuControls.ShowVelvetTrialsCanvas();
            break;
            case CanvasType.Compendium:
                _mainMenuControls.ShowCompendiumCanvas();
            break;
            case CanvasType.Fusion:
                _mainMenuControls.ShowFusionCanvas();
            break;
        }
    }
}
