using System.Formats.Asn1;

namespace GodotChess.Core;

public static class BoardHelper
{
    public const int A1 = 0;
    public const int B1 = 1;
    public const int C1 = 2;
    public const int D1 = 3;
    public const int E1 = 4;
    public const int F1 = 5;
    public const int G1 = 6;
    public const int H1 = 7;

    public const int A8 = 56;
    public const int B8 = 57;
    public const int C8 = 58;
    public const int D8 = 59;
    public const int E8 = 60;
    public const int F8 = 61;
    public const int G8 = 62;
    public const int H8 = 63;

    public static int RankIndex(int squareIndex)
    {
        return squareIndex / 8;
    }

    public static int FileIndex(int squareIndex)
    {
        return squareIndex % 8;
    }

    public static int IndexFromCoord(Coord coord)
    {
        return coord.rankIndex * 8 + coord.fileIndex;
    }
}