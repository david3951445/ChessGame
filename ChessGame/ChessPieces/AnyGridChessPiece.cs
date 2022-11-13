using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.ChessPieces
{
    abstract class AnyGridChessPiece : ChessPiece // Can move any number of grids. ex. Queen, Rook. Bishop 
    {
        protected AnyGridChessPiece(bool _isWhite, string _name) : base(_isWhite, _name) { }

        public override void Rule(ChessBoard board) {
            foreach (var bias in dirs) {
                Coords coord = board.pickUpCoord + bias;
                while (board.AddTip(coord, this)) {
                    coord += bias;
                }
            }
        }
    }
    class Queen : AnyGridChessPiece
    {
        public Queen(bool _isWhite) : base(_isWhite, "Q") {
            dirs = Dir.King();
        }
    }

    class Rook : AnyGridChessPiece
    {
        public Rook(bool _isWhite) : base(_isWhite, "R") {
            dirs = Dir.Rook();
        }
    }

    class Bishop : AnyGridChessPiece
    {
        public Bishop(bool _isWhite) : base(_isWhite, "B") {
            dirs = Dir.Bishop();
        }
    }
}
