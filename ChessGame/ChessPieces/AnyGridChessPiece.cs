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

        public override void Rule(ChessBoard board)
        {
            foreach (var bias in Directions)
            {
                Coord coord = board.pickUpCoord + bias;
                while (board.AddTip(coord, this))
                    coord += bias;
            }
        }
    }

    class Queen : AnyGridChessPiece
    {
        public Queen(bool isWhite) : base(isWhite, "Q")
        {
            Directions = Dir.King();
        }
        protected override Coord[] Directions { get; }
    }

    class Rook : AnyGridChessPiece
    {
        public Rook(bool isWhite) : base(isWhite, "R")
        {
            Directions = Dir.Rook();
        }
        protected override Coord[] Directions { get; }
    }

    class Bishop : AnyGridChessPiece
    {
        public Bishop(bool isWhite) : base(isWhite, "B")
        {
            Directions = Dir.Bishop();
        }
        protected override Coord[] Directions { get; }
    }
}
