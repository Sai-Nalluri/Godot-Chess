using Godot;
using GodotChess.UI;
using GodotChess.Core;

namespace GodotChess.UIScripts;

public partial class ChessBoard : Node2D
{
    [Export] private PackedScene squareScene;
    [Signal] public delegate void SquareClickedEventHandler(int rank, int file);

    private Square[,] squares = new Square[8, 8];

    BoardTheme boardTheme;

    public override void _Ready()
    {
        base._Ready();
        boardTheme = new();
        CreateChessBoard();

        Connect(SignalName.SquareClicked, new Callable(this, nameof(HighlightSquare)));
    }

    public void HighlightSquare(int rank, int file)
    {
        GD.Print(rank + " " + file);
        SetSquareColor(rank, file, boardTheme.lightSquares.selected, boardTheme.darkSquares.selected);
    }

    void CreateChessBoard()
    {
        for (int rank = 0; rank < 8; rank++)
        {
            for (int file = 0; file < 8; file++)
            {
                Square square = squareScene.Instantiate<Square>();
                square.Initialize(rank, file);
                var windowSize = DisplayServer.ScreenGetSize();
                square.Position = new Vector2(
                    file * 80 + (windowSize.X / 2 - (8 * 40)),
                    rank * 80 + (windowSize.Y / 2 - (8 * 40))
                );

                squares[rank, file] = square;
                AddChild(square);
            }
        }
        ResetSquareColors();
    }

    void ResetSquareColors()
    {
        for (int rank = 0; rank < 8; rank++)
        {
            for (int file = 0; file < 8; file++)
            {
                SetSquareColor(rank, file, boardTheme.lightSquares.normal, boardTheme.darkSquares.normal);
            }
        }
    }

    void SetSquareColor(int rank, int file, Color lightColor, Color darkColor)
    {
        bool isWhite = (rank + file) % 2 == 0;
        squares[rank, file].SetBackgroundColor(isWhite ? lightColor : darkColor);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                Vector2 mousePos = mouseEvent.Position;

                var windowSize = DisplayServer.ScreenGetSize();
                float boardOffsetX = windowSize.X / 2 - (8 * 40);
                float boardOffsetY = windowSize.Y / 2 - (8 * 40);
                int file = (int)((mousePos.X - boardOffsetX) / 80);
                int rank = (int)((mousePos.Y - boardOffsetY) / 80);

                if (file >= 0 && file < 8 && rank >= 0 && rank < 8)
                {
                    EmitSignal(SignalName.SquareClicked, rank, file);
                }
            }
        }
    }
}
