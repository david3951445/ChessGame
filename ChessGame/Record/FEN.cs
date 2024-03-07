using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame
{
    /// <summary>
    /// Forsyth–Edwards Notation
    /// </summary>
    public class FEN
    {
        public static FEN Default => new FEN();
        public string PiecePlacementData { get; private set; } = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
        //public char ActiveColor { get; private set; }
        //public string CastlingAvailability { get; private set; }
        //public string EnPassantTargetSquare { get; private set; }
        //public int HalfmoveClock { get; private set; }
        //public int FullmoveNumber { get; private set; }

        public FEN() { }

        public FEN(string value)
        {
            var items = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            PiecePlacementData = items[0];
        }
    }
}
