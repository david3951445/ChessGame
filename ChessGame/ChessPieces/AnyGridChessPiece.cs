using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

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

        public static bool CanSeeChess(ChessBoard board, Coord startCoord, Coord bias, out ChessPiece? chessPiece)
        {
            chessPiece = null;
            while (!board.IsOutOfBound(startCoord))
            {
                chessPiece = board.GetChessOn(startCoord);
                if (chessPiece == null)
                {
                    startCoord += bias;
                    continue;
                }
                return true;
            }
            return false;
        }
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
