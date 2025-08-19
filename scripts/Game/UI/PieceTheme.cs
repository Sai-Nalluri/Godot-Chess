using Godot;
using GodotChess.Core;

namespace GodotChess.UI;

public class PieceTheme
{
    public PieceTextures whitePieces;
    public PieceTextures blackPieces;

    public PieceTheme()
    {
        whitePieces = new();
        blackPieces = new();
        LoadTextures();
    }

    public void LoadTextures()
    {
        whitePieces.pawn = GD.Load<Texture2D>("res://chess pieces/white_pawn.png");
        whitePieces.knight = GD.Load<Texture2D>("res://chess pieces/white_knight.png");
        whitePieces.bishop = GD.Load<Texture2D>("res://chess pieces/white_bishop.png");
        whitePieces.rook = GD.Load<Texture2D>("res://chess pieces/white_rook.png");
        whitePieces.queen = GD.Load<Texture2D>("res://chess pieces/white_queen.png");
        whitePieces.king = GD.Load<Texture2D>("res://chess pieces/white_king.png");

        blackPieces.pawn = GD.Load<Texture2D>("res://chess pieces/black_pawn.png");
        blackPieces.knight = GD.Load<Texture2D>("res://chess pieces/black_knight.png");
        blackPieces.bishop = GD.Load<Texture2D>("res://chess pieces/black_bishop.png");
        blackPieces.rook = GD.Load<Texture2D>("res://chess pieces/black_rook.png");
        blackPieces.queen = GD.Load<Texture2D>("res://chess pieces/black_queen.png");
        blackPieces.king = GD.Load<Texture2D>("res://chess pieces/black_king.png");
    }

    public Texture2D GetPieceTexture(int piece)
    {
        PieceTextures pieceTextures = Piece.IsColor(piece, Piece.White) ? whitePieces : blackPieces;

        switch (Piece.PieceType(piece))
        {
            case Piece.Pawn:
                return pieceTextures.pawn;
            case Piece.Rook:
                return pieceTextures.rook;
            case Piece.Knight:
                return pieceTextures.knight;
            case Piece.Bishop:
                return pieceTextures.bishop;
            case Piece.Queen:
                return pieceTextures.queen;
            case Piece.King:
                return pieceTextures.king;
            default:
                return null;
        }
    }

    public class PieceTextures
    {
        public Texture2D pawn, knight, bishop, rook, queen, king;
    }
}