using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.ChessPieces
{
    abstract class AnyGridChessPiece : ChessPiece // Can move any number of grids. ex. Queen, Rook. Bishop 
    {
        protected AnyGridChessPiece(bool isWhite, string name) : base(isWhite, name) { }

        public override void AddTipToBoard(ChessBoard board)
        {
            foreach (var bias in Directions)
            {
                Coord coord = Coord + bias;
                while (board.AddTip(coord, this))
                    coord += bias;
            }
        }

        public override IEnumerable<ChessPiece> CapturedChesses(ChessBoard board) => Directions
            .Select(c => board.GetSeenChess(Coord, c))
            .NonNull()
            .Where(seenChess => !IsSameColor(seenChess));
    }

    class Queen : AnyGridChessPiece
    {
        public Queen(bool isWhite) : base(isWhite, "Q")
        {
            Directions = Dir.King();
        }
    }

    class Rook : AnyGridChessPiece, ISpecialMovePiece
    {
        public bool HasMoved { get; set; }
        public bool IsShortSide { get; init; }

        public Rook(bool isWhite) : base(isWhite, "R")
        {
            Directions = Dir.Rook();
        }
    }

    class Bishop : AnyGridChessPiece
    {
        public Bishop(bool isWhite) : base(isWhite, "B")
        {
            Directions = Dir.Bishop();
        }
    }
}
