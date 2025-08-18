using System;
using System.Diagnostics;
using Godot;

namespace GodotChess.Core;

public class Board
{
    public const int WhiteIndex = 0;
    public const int BlackIndex = 1;

    // Side to move info
    public bool isWhiteToMove;
    public int moveColor => isWhiteToMove ? Piece.White : Piece.Black;
    public int opponentColor => isWhiteToMove ? Piece.Black : Piece.White;
    public int moveColorIndex => isWhiteToMove ? 0 : 1;
    public int opponentColorIndex => isWhiteToMove ? 1 : 0;

    // Variable to check what color the player is
    public int playerColor = Piece.White;

    // Store the piece ints for the entire board in a array
    public int[] Square = new int[64];
    // Square indexes of the white and black king
    public int[] KingSquare = new int[2];

    // Piece lists
    public PieceList[] Pawns = new PieceList[2];
    public PieceList[] Knights = new PieceList[2];
    public PieceList[] Bishops = new PieceList[2];
    public PieceList[] Rooks = new PieceList[2];
    public PieceList[] Queens = new PieceList[2];
    PieceList[] pieceLists = new PieceList[Piece.MaxPieceIndex + 1];


    // Bitboards
    // Array of every single bitboard
    public ulong[] pieceBitboards = new ulong[Piece.MaxPieceIndex + 1];
    // Based on color
    public ulong[] colorBitboards = new ulong[2];
    public ulong allPiecesBitboard;
    // Where sliding pieces are for legal move generation
    public ulong friendlyOrthogonalSliders;
    public ulong friendlyDiagonalSliders;

    // Total move count
    public int plyCount;
    public int fiftyMoveRuleCounter;

    public GameState currentGameState;

    void MovePiece(int piece, int startSquare, int targetSquare)
    {
        // Update the bitboards
        BitBoardUtility.ToggleSquare(ref pieceBitboards[piece], startSquare, targetSquare);
        BitBoardUtility.ToggleSquare(ref colorBitboards[moveColorIndex], startSquare, targetSquare);

        // Update the piece list
        pieceLists[piece].MovePiece(startSquare, targetSquare);

        // Update the array
        Square[startSquare] = Piece.None;
        Square[targetSquare] = piece;
    }

    // Make a move on the board
    public void MakeMove(Move move, bool inSearch = false)
    {
        int startSquare = move.startSquare;
        int targetSquare = move.targetSquare;
        int flag = move.flag;

        int movedPiece = Square[startSquare];
        int movedPieceType = Piece.PieceType(movedPiece);
        int capturedPiece = Square[targetSquare];
        int capturedPieceType = Piece.PieceType(capturedPiece);

        int previousCastleRight = currentGameState.castlingRights;
        int previousEnPassantFile = currentGameState.enPassantFile;
        int newCastleRights = previousCastleRight;
        int newEnPassantFile = -1;

        // Update the bitboards, arrays and piece lists. The special cases are handled after
        MovePiece(movedPiece, startSquare, targetSquare);

        // Handle captures
        if (capturedPieceType != Piece.None)
        {
            int captureSquare = targetSquare;

            // Skip piece list removal for kings since they use KingSquare array instead
            if (capturedPieceType != Piece.King)
            {
                pieceLists[capturedPiece].RemovePieceAtSquare(captureSquare);
            }
            BitBoardUtility.ToggleSquare(ref pieceBitboards[capturedPiece], captureSquare);
            BitBoardUtility.ToggleSquare(ref colorBitboards[opponentColorIndex], captureSquare);
        }

        // Handle king movement
        if (movedPieceType == Piece.King)
        {
            KingSquare[moveColorIndex] = targetSquare;
        }

        // If a pawn has moved up two squares, then that square in en passantable
        if (flag == Move.PawnTwoUpFlag)
        {
            // Since we set new enpassant square to 0, this should not be zero
            int file = BoardHelper.FileIndex(startSquare);
            newEnPassantFile = file;
        }

        // Update the castle rights
        if (previousCastleRight != 0)
        {
            // Any piece moving to/from rook squares means no castling rights that side
            // Since if anything moving from, rook move and moving to means captured
            if (targetSquare == BoardHelper.H1 || startSquare == BoardHelper.H1)
            {
                newCastleRights &= GameState.WhiteKingSideMask;
            }
            else if (targetSquare == BoardHelper.A1 || startSquare == BoardHelper.A1)
            {
                newCastleRights &= GameState.WhiteQueenSideMask;
            }
            else if (targetSquare == BoardHelper.H8 || startSquare == BoardHelper.H8)
            {
                newCastleRights &= GameState.BlackKingSideMask;
            }
            else if (targetSquare == BoardHelper.A8 || startSquare == BoardHelper.A8)
            {
                newCastleRights &= GameState.BlackQueenSideMask;
            }
        }

        // Change side to move
        isWhiteToMove = !isWhiteToMove;
        plyCount++;

        GameState newGameState = new GameState(newEnPassantFile, newCastleRights);
        currentGameState = newGameState;

        // Update the piece bitboards
        allPiecesBitboard = colorBitboards[WhiteIndex] | colorBitboards[BlackIndex];
        UpdateSliderBitboards();
    }

    // Load the start position
    public void LoadStartPosition()
    {
        LoadPosition(FenUtility.StartingPositionFen);
    }

    public void LoadPosition(string fen)
    {
        FenUtility.PositionInfo posInfo = FenUtility.PositionFromFen(fen);
        LoadPosition(posInfo);
    }

    // Load custom position
    public void LoadPosition(FenUtility.PositionInfo posInfo)
    {
        Initialize();

        // Load pieces into board array and piece lists
        for (int squareIndex = 0; squareIndex < 64; squareIndex++)
        {
            int piece = posInfo.squares[squareIndex];
            int pieceType = Piece.PieceType(piece);
            int colorIndex = Piece.PieceColor(piece) == Piece.White ? 0 : 1;
            Square[squareIndex] = piece;

            if (piece != Piece.None)
            {
                // Set the bitboards
                BitBoardUtility.SetSquare(ref pieceBitboards[piece], squareIndex);
                BitBoardUtility.SetSquare(ref colorBitboards[colorIndex], squareIndex);

                if (pieceType == Piece.King)
                {
                    KingSquare[colorIndex] = squareIndex;
                }
                else
                {
                    pieceLists[piece].AddPieceAtSquare(squareIndex);
                }
            }
        }
        // Side to move
        isWhiteToMove = posInfo.whiteToMove;

        // Set extra bitboard
        allPiecesBitboard = colorBitboards[WhiteIndex] | colorBitboards[BlackIndex];

        // Set up the gamestate
        int whiteCastleRights = (posInfo.whiteCastleKingside ? 1 >> 3 : 0) | (posInfo.whiteCastleQueenside ? 1 >> 2 : 0);
        int blackCastleRights = (posInfo.blackCastleKingside ? 1 >> 1 : 0) | (posInfo.blackCastleQueenside ? 1 : 0);
        currentGameState = new GameState(posInfo.epFile, whiteCastleRights | blackCastleRights);
    }

    void UpdateSliderBitboards()
    {
        int friendlyRook = Piece.MakePiece(Piece.Rook, Piece.White);
        int friendlyBishop = Piece.MakePiece(Piece.Bishop, Piece.White);
        int friendlyQueen = Piece.MakePiece(Piece.Queen, Piece.White);
        friendlyOrthogonalSliders = pieceBitboards[friendlyRook] | pieceBitboards[friendlyQueen];
        friendlyDiagonalSliders = pieceBitboards[friendlyBishop] | pieceBitboards[friendlyQueen];
    }

    void Initialize()
    {
        Square = new int[64];
        KingSquare = new int[2];

        plyCount = 0;
        fiftyMoveRuleCounter = 0;
        playerColor = Piece.White;

        Pawns = new PieceList[] { new PieceList(8), new PieceList(8) };
        Knights = new PieceList[] { new PieceList(10), new PieceList(10) };
        Bishops = new PieceList[] { new PieceList(10), new PieceList(10) };
        Rooks = new PieceList[] { new PieceList(10), new PieceList(10) };
        Queens = new PieceList[] { new PieceList(9), new PieceList(9) };

        pieceLists = new PieceList[Piece.MaxPieceIndex + 1];
        // Initialize all entries to avoid null reference
        for (int i = 0; i <= Piece.MaxPieceIndex; i++)
        {
            pieceLists[i] = new PieceList(1);
        }
        pieceLists[Piece.WhitePawn] = Pawns[WhiteIndex];
        pieceLists[Piece.WhiteKnight] = Knights[WhiteIndex];
        pieceLists[Piece.WhiteBishop] = Bishops[WhiteIndex];
        pieceLists[Piece.WhiteRook] = Rooks[WhiteIndex];
        pieceLists[Piece.Queen] = Queens[WhiteIndex];
        pieceLists[Piece.WhiteKing] = new PieceList(1);

        pieceLists[Piece.BlackPawn] = Pawns[BlackIndex];
        pieceLists[Piece.BlackKnight] = Knights[BlackIndex];
        pieceLists[Piece.BlackBishop] = Bishops[BlackIndex];
        pieceLists[Piece.BlackRook] = Rooks[BlackIndex];
        pieceLists[Piece.BlackQueen] = Queens[BlackIndex];
        pieceLists[Piece.BlackKing] = new PieceList(1);

        pieceBitboards = new ulong[Piece.MaxPieceIndex + 1];
        colorBitboards = new ulong[2];
        allPiecesBitboard = 0;
        friendlyOrthogonalSliders = 0;
        friendlyDiagonalSliders = 0;
    }
}