namespace GodotChess.Core;


public static class Piece
{
    // Piece Types
    public const int None = 0;
    public const int Pawn = 1;
    public const int Knight = 2;
    public const int Bishop = 3;
    public const int Rook = 4;
    public const int Queen = 5;
    public const int King = 6;

    // Piece colors
    public const int White = 0;
    public const int Black = 8;

    // Pieces
    public const int WhitePawn = White | Pawn;
    public const int WhiteKnight = White | Knight;
    public const int WhiteBishop = White | Bishop;
    public const int WhiteRook = White | Rook;
    public const int WhiteQueen = White | Queen;
    public const int WhiteKing = White | King;

    public const int BlackPawn = Black | Pawn;
    public const int BlackKnight = Black | Knight;
    public const int BlackBishop = Black | Bishop;
    public const int BlackRook = Black | Rook;
    public const int BlackQueen = Black | Queen;
    public const int BlackKing = Black | King;

    // This is the highest piece int any piece can be
    public const int MaxPieceIndex = BlackKing;

    // Bits masks
    const int TypeMask = 0b111;
    const int ColorMask = 0b1000;

    public static int MakePiece(int pieceType, int pieceColor) => pieceType | pieceColor;
    public static int MakePiece(int pieceType, bool isWhitePiece) => MakePiece(pieceType, isWhitePiece ? White : Black);
    public static bool IsColor(int piece, int color) => (piece & ColorMask) == color && piece != 0;
    public static bool IsWhite(int piece) => IsColor(piece, White);
    public static int PieceType(int piece) => piece & TypeMask;
    public static int PieceColor(int piece) => piece & ColorMask;
    public static bool IsSlidingPiece(int piece) => PieceType(piece) is Queen or Rook or Bishop;
    public static bool IsOthogonalSlidingPiece(int piece) => PieceType(piece) is Queen or Rook;
    public static bool IsDiagonalSlidingPiece(int piece) => PieceType(piece) is Queen or Bishop;
}