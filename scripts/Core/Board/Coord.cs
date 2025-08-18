namespace GodotChess.Core;

public readonly struct Coord
{
    public readonly int rankIndex;
    public readonly int fileIndex;

    public Coord(int rankIndex, int fileIndex)
    {
        this.rankIndex = rankIndex;
        this.fileIndex = fileIndex;
    }

    public Coord(int squareIndex)
    {
        this.rankIndex = BoardHelper.RankIndex(squareIndex);
        this.fileIndex = BoardHelper.FileIndex(squareIndex);
    }

    public bool IsLightSquare()
    {
        return (rankIndex + fileIndex) % 2 == 0;
    }

    public int SquareIndex => BoardHelper.IndexFromCoord(this);
}