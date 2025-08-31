using Godot;
using System;

namespace GodotChess.UIScripts;

public partial class Square : Node2D
{
    [Export(PropertyHint.Range, "0, 7, 1")]
    public int Rank { get; private set; }
    [Export(PropertyHint.Range, "0, 7, 1")]
    public int File { get; private set; }
    private ColorRect _background;
    private Sprite2D _pieceSprite;

    public override void _Ready()
    {
        base._Ready();

        _background = GetNode<ColorRect>("Background");
        _pieceSprite = GetNode<Sprite2D>("PieceSprite");
    }

    public void Initialize(int rank, int file)
    {
        Rank = rank;
        File = file;
    }

    public void SetBackgroundColor(Color color)
    {
        _background.Color = color;
    }

}
