namespace GodotChess.Core;

public readonly struct GameState
{
    public readonly int enPassantFile;
    public readonly int castlingRights;
    public readonly int fiftyMoveRule;

    public const int WhiteKingSideMask = 0b1000;
    public const int WhiteQueenSideMask = 0b0100;
    public const int BlackKingSideMask = 0b0010;
    public const int BlackQueenSideMask = 0b0001;

    public GameState(int enPassantFile, int castlingRights)
    {
        this.enPassantFile = enPassantFile;
        this.castlingRights = castlingRights;
    }

    public bool HasKingSideCastleRight(bool isWhite)
    {
        int mask = isWhite ? 8 : 2;
        return (castlingRights & mask) != 0;
    }

    public bool HasQueenSideCastleRight(bool isWhite)
    {
        int mask = isWhite ? 4 : 1;
        return (castlingRights & mask) != 0;
    }
}