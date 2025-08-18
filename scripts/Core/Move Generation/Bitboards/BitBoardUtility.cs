using System;
using System.IO;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;

namespace GodotChess.Core;


public static class BitBoardUtility
{
    public const ulong Rank1 = 0b11111111;
    public const ulong Rank2 = Rank1 << 8;
    public const ulong Rank3 = Rank2 << 8;
    public const ulong Rank4 = Rank3 << 8;
    public const ulong Rank5 = Rank4 << 8;
    public const ulong Rank6 = Rank5 << 8;
    public const ulong Rank7 = Rank6 << 8;
    public const ulong Rank8 = Rank7 << 8;

    public const ulong FileA = 72340172838076673;
    public const ulong notAFile = ~FileA;
    public const ulong notHFile = ~(FileA << 7);

    public static void SetSquare(ref ulong bitboard, int squareIndex)
    {
        bitboard |= 1UL << squareIndex;
    }

    public static void ClearSquare(ref ulong bitboard, int squareIndex)
    {
        bitboard &= ~(1UL << squareIndex);
    }

    public static void ToggleSquare(ref ulong bitboard, int squareIndex)
    {
        bitboard ^= 1UL << squareIndex;
    }

    // Toggle 2 squares
    public static void ToggleSquare(ref ulong bitboard, int startSquareIndex, int targetSquareIndex)
    {
        bitboard ^= 1UL << startSquareIndex | 1UL << targetSquareIndex;
    }

    public static ulong Shift(ulong bitboard, int numToShift)
    {
        if (numToShift > 0)
        {
            return bitboard << numToShift;
        }
        else
        {
            return bitboard >> -numToShift;
        }
    }

    // Return the index of where the first 1 is set from the right
    public static int PopLSB(ref ulong bitboard)
    {
        int trailingZeroCount = BitOperations.TrailingZeroCount(bitboard);
        bitboard &= bitboard - 1;
        return trailingZeroCount;
    }

    public static bool IsSet(ulong bitboard, int squareIndex)
    {
        return (bitboard & (1UL << squareIndex)) != 0;
    }
}