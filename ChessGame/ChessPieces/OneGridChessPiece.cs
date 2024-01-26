using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.ChessPieces
{
    abstract class OneGridChessPiece : ChessPiece // Can move one number of grids. ex. King, Knight
    {
        protected OneGridChessPiece(bool isWhite, string name) : base(isWhite, name) { }

        public override void Rule(ChessBoard board)
        {
            foreach (var bias in Directions)
            {
                board.AddTip(board.PickUpCoord + bias, this);
            }
        }
    }

    class King : OneGridChessPiece
    {
        public King(bool isWhite) : base(isWhite, "K")
        {
            Directions = Dir.King();
        }

        public override void Rule(ChessBoard board)
        {
            base.Rule(board);

            // Castling
        }
    }

    class Knight : OneGridChessPiece
    {
        public Knight(bool _isWhite) : base(_isWhite, "N")
        {
            Directions = Dir.Knight();
        }
    }
}
