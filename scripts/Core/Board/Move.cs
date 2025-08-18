/*
Compact (16 bits) representation to preserve memory
ffffttttttssssss

f - flag
t - target square
s - starting sqaure
*/
namespace GodotChess.Core;


public readonly struct Move
{
    readonly ushort moveValue;

    // Flags
    public const int NoFlag = 0b0000;
    public const int EnPassantCaptureFlag = 0b0001;
    public const int CastleFlag = 0b0010;
    public const int PawnTwoUpFlag = 0b0011;
    // Promotions
    public const int PromoteToQueenFlag = 0b0100;
    public const int PromoteToKnightFlag = 0b0101;
    public const int PromoteToRookFlag = 0b0110;
    public const int PromoteToBishopFlag = 0b0111;

    // Masks
    const ushort StartSquareFlag = 0b0000000000111111;
    const ushort TargetSquareFlag = 0b0000111111000000;
    const ushort FlagMask = 0b1111000000000000;

    public Move(ushort moveValue)
    {
        this.moveValue = moveValue;
    }

    public Move(int startSquare, int targetSquare)
    {
        moveValue = (ushort)(startSquare | (targetSquare << 6));
    }

    public Move(int startSquare, int targetSquare, int flag)
    {
        moveValue = (ushort)(startSquare | (targetSquare << 6) | (flag << 12));
    }

    public ushort Value => moveValue;
    public int startSquare => moveValue & StartSquareFlag;
    public int targetSquare => (moveValue & TargetSquareFlag) >> 6;
    public int flag => (moveValue & FlagMask) >> 12;
}