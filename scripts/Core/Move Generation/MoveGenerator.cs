using System;
using System.Collections.Generic;
using System.Linq;
using static GodotChess.Core.PrecomputedMoveData;
using static GodotChess.Core.BoardHelper;

namespace GodotChess.Core;


public class MoveGenerator
{
    public const int MaxMoves = 218;

    public enum PromotionMode { All, QueenOnly, QueenAndKnight };
    public PromotionMode promotionsToGenerate = PromotionMode.All;

    bool isWhiteToMove;
    int friendlyColor;
    int opponentColor;
    int friendlyColorIndex;
    int opponentColorIndex;
    ulong friendlyPieceBitboard;
    ulong opponentPieceBitboard;
    ulong emptySquares;
    ulong enemyPieces;
    ulong allPieceBitboards;
    int currentMoveIndex;
    int friendlyKingSquare;

    Board board = new Board();

    public List<Move> GenerateMoves(Board board)
    {
        List<Move> moves = new List<Move>(MaxMoves);
        return GenerateMoves(board, moves);
    }

    public List<Move> GenerateMoves(Board board, List<Move> moves)
    {
        this.board = board;
        Init();

        GenerateKingMoves(moves);
        GeneratePawnMoves(moves);
        GenerateSlidingMoves(moves);
        GenerateKnightMoves(moves);

        return moves.GetRange(0, currentMoveIndex);
    }

    void GenerateKingMoves(List<Move> moves)
    {
        for (int index = 0; index < kingSquares[friendlyKingSquare].Count(); index++)
        {
            int targetSquare = kingSquares[friendlyKingSquare][index];
            int pieceOnTargetSquare = board.Square[targetSquare];

            if (Piece.IsColor(pieceOnTargetSquare, friendlyColorIndex))
            {
                continue;
            }

            moves.Add(new Move(friendlyKingSquare, targetSquare));
            currentMoveIndex++;
        }
    }
    void GenerateSlidingMoves(List<Move> moves)
    {
        PieceList rooks = board.Rooks[friendlyColorIndex];
        for (int index = 0; index < rooks.Count; index++)
        {
            GenerateSlidingPiecesMoves(rooks[index], 0, 4, moves);
        }

        PieceList bishops = board.Bishops[friendlyColorIndex];
        for (int index = 0; index < bishops.Count; index++)
        {
            GenerateSlidingPiecesMoves(bishops[index], 4, 8, moves);
        }

        PieceList queens = board.Queens[friendlyColorIndex];
        for (int index = 0; index < queens.Count; index++)
        {
            GenerateSlidingPiecesMoves(queens[index], 0, 8, moves);
        }
    }

    void GenerateSlidingPiecesMoves(int startSquare, int startDirectionIndex, int endDirectionIndex, List<Move> moves)
    {
        for (int directionIndex = startDirectionIndex; directionIndex < endDirectionIndex; directionIndex++)
        {
            int directionOffset = directionOffsets[directionIndex];
            for (int n = 0; n < numSquaresToEdge[startSquare][directionIndex]; n++)
            {
                int targetSquare = startSquare + directionOffset * (n + 1);
                int targetSquarePiece = board.Square[targetSquare];

                if (Piece.IsColor(targetSquarePiece, friendlyColor))
                {
                    break;
                }

                moves.Add(new Move(startSquare, targetSquare));
                currentMoveIndex++;

                bool isCapture = targetSquarePiece != Piece.None;

                if (isCapture)
                {
                    break;
                }
            }
        }
    }

    void GeneratePawnMoves(List<Move> moves)
    {
        PieceList pawns = board.Pawns[friendlyColorIndex];
        int pushDir = isWhiteToMove ? 1 : -1;
        int pushOffset = pushDir * 8;
        int startRank = isWhiteToMove ? 1 : 6;
        int finalRankBeforePromotion = isWhiteToMove ? 6 : 1;

        for (int index = 0; index < pawns.Count; index++)
        {
            int startSquare = pawns[index];
            int rank = RankIndex(startSquare);
            bool oneStepFromPromotion = rank == finalRankBeforePromotion;

            int oneSquareForward = startSquare + pushOffset;
            if (board.Square[oneSquareForward] == Piece.None)
            {
                if (oneStepFromPromotion)
                {
                    MakePromotionMove(startSquare, oneSquareForward, moves);
                }
                else
                {
                    moves.Add(new Move(startSquare, oneSquareForward));
                    currentMoveIndex++;
                }

                int twoSquareForward = oneSquareForward + pushOffset;
                if (rank == startRank && board.Square[twoSquareForward] == Piece.None)
                {
                    moves.Add(new Move(startSquare, twoSquareForward, Move.PawnTwoUpFlag));
                    currentMoveIndex++;
                }
            }

            for (int j = 0; j < 2; j++)
            {
                if (numSquaresToEdge[startSquare][pawnAttackDirections[friendlyColorIndex][j]] > 0)
                {
                    int pawnCaptureDir = directionOffsets[pawnAttackDirections[friendlyColorIndex][j]];
                    int targetSquare = startSquare + pawnCaptureDir;
                    int targetPiece = board.Square[targetSquare];

                    if (Piece.IsColor(targetPiece, opponentColor))
                    {
                        if (oneStepFromPromotion)
                        {
                            MakePromotionMove(startSquare, targetSquare, moves);
                        }
                        else
                        {
                            moves.Add(new Move(startSquare, targetSquare));
                            currentMoveIndex++;
                        }
                    }
                }
            }
        }
    }

    void MakePromotionMove(int startSquare, int targetSquare, List<Move> moves)
    {
        moves.Add(new Move(startSquare, targetSquare, Move.PromoteToQueenFlag));
        currentMoveIndex++;
        if (promotionsToGenerate == PromotionMode.All)
        {
            moves.Add(new Move(startSquare, targetSquare, Move.PromoteToBishopFlag));
            currentMoveIndex++;
            moves.Add(new Move(startSquare, targetSquare, Move.PromoteToRookFlag));
            currentMoveIndex++;
            moves.Add(new Move(startSquare, targetSquare, Move.PromoteToKnightFlag));
            currentMoveIndex++;
        }
        else if (promotionsToGenerate == PromotionMode.QueenAndKnight)
        {
            moves.Add(new Move(startSquare, targetSquare, Move.PromoteToKnightFlag));
            currentMoveIndex++;
        }
    }

    void GenerateKnightMoves(List<Move> moves)
    {
        PieceList knights = board.Knights[friendlyColorIndex];
        for (int index = 0; index < knights.Count; index++)
        {
            int startSquare = knights[index];
            foreach (int targetSquare in knightSquares[startSquare])
            {
                if (!Piece.IsColor(board.Square[targetSquare], friendlyColor))
                {
                    moves.Add(new Move(startSquare, targetSquare));
                    currentMoveIndex++;
                }
            }
        }
    }

    void Init()
    {
        currentMoveIndex = 0;

        isWhiteToMove = board.moveColor == Piece.White;
        friendlyColor = board.moveColor;
        opponentColor = board.opponentColor;
        friendlyColorIndex = board.moveColorIndex;
        opponentColorIndex = board.opponentColorIndex;
        friendlyKingSquare = board.KingSquare[friendlyColorIndex];

        opponentPieceBitboard = board.colorBitboards[opponentColorIndex];
        friendlyPieceBitboard = board.colorBitboards[friendlyColorIndex];
        allPieceBitboards = board.allPiecesBitboard;

        emptySquares = ~allPieceBitboards;
        enemyPieces = ~(emptySquares | friendlyPieceBitboard);
    }
}