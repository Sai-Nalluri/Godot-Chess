using Godot;
using GodotChess.UI;
using System;

namespace GodotChess.UIScripts;

public partial class ChessBoard : Node2D
{
    [Export] private PackedScene squareScene;

    private Square[,] squares = new Square[8, 8];

    BoardTheme boardTheme;

    public override void _Ready()
    {
        base._Ready();
        boardTheme = new();
        CreateChessBoard();
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
                square.Position = new Vector2(file * 80 + (windowSize.X / 2 - (8 * 40)), rank * 80 + (windowSize.Y / 2 - (8 * 40)));

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
}
