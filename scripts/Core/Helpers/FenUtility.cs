namespace GodotChess.Core;


public class FenUtility
{
    public const string StartingPositionFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    // Load position from fen string
    public static PositionInfo PositionFromFen(string fen)
    {
        PositionInfo loadedPositionInfo = new PositionInfo();
        string[] sections = fen.Split(" ");

        // Load the board
        int file = 0;
        int rank = 0;

        foreach (char symbol in sections[0])
        {
            if (symbol == '/')
            {
                file = 0;
                rank++;
            }
            else
            {
                if (char.IsDigit(symbol))
                {
                    file += (int)char.GetNumericValue(symbol);
                }
                else
                {
                    int pieceColor = char.IsUpper(symbol) ? Piece.White : Piece.Black;
                    int pieceType = char.ToLower(symbol) switch
                    {
                        'p' => Piece.Pawn,
                        'n' => Piece.Knight,
                        'b' => Piece.Bishop,
                        'r' => Piece.Rook,
                        'q' => Piece.Queen,
                        'k' => Piece.King,
                        _ => Piece.None
                    };

                    loadedPositionInfo.squares[rank * 8 + file] = pieceType | pieceColor;
                    file++;
                }
            }
        }

        loadedPositionInfo.whiteToMove = sections[1] == "w";

        string castlingRights = sections[2];
        loadedPositionInfo.whiteCastleKingside = castlingRights.Contains("K");
        loadedPositionInfo.whiteCastleQueenside = castlingRights.Contains("Q");
        loadedPositionInfo.blackCastleKingside = castlingRights.Contains("k");
        loadedPositionInfo.blackCastleQueenside = castlingRights.Contains("q");

        if (sections.Length > 3)
        {
            string enPassantFileName = sections[3][0].ToString();
            // TODO
            // if statment to check if file name is valid
            loadedPositionInfo.epFile = ' ';
        }

        // Half move clock
        if (sections.Length > 4)
        {
            int.TryParse(sections[4], out loadedPositionInfo.fiftyMovePlyCount);
        }

        // Full move clock
        if (sections.Length > 5)
        {
            int.TryParse(sections[5], out loadedPositionInfo.moveCount);
        }
        return loadedPositionInfo;
    }

    public class PositionInfo
    {
        public int[] squares = new int[64];
        public bool whiteToMove;
        // Castling rights
        public bool whiteCastleKingside;
        public bool whiteCastleQueenside;
        public bool blackCastleKingside;
        public bool blackCastleQueenside;
        // En passantable file (1 is a, 8 is h 0 is none)
        public int epFile;
        // Number of half moves since a pawn move or capture
        // (0, then +1 every time a side moves)
        public int fiftyMovePlyCount;
        // Total number of moves played in the game
        // (starts at 1 and increments after black move)
        public int moveCount;

        public PositionInfo()
        {
            squares = new int[64];
        }
    }
}