using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.ChessPieces
{
    public abstract class OneGridChessPiece : ChessPiece // Can move one number of grids. ex. King, Knight
    {
        protected OneGridChessPiece(bool isWhite, string name) : base(isWhite, name) { }

        public override void AddTipToBoard(ChessBoard board)
        {
            foreach (var bias in Directions)
                board.AddTip(board.PickUpCoord + bias, this);
        }
    }

    public class King : OneGridChessPiece, ISpecialMovePiece
    {
        public bool HasMoved { get; set; }
        public Castling LongCastling { get; }
        public Castling ShortCastling { get; }

        public King(bool isWhite) : base(isWhite, "K")
        {
            Directions = Dir.King();
            LongCastling = new Castling(IsWhite ? new Coord(7, 2) : new Coord(0, 2));
            ShortCastling = new Castling(IsWhite ? new Coord(7, 6) : new Coord(0, 6));
        }

        public override void AddTipToBoard(ChessBoard board)
        {
            base.AddTipToBoard(board);
            // Castling
            if (ShortCastling.IsValid)
                board.AddTip(ShortCastling.Coord, this);
            if (LongCastling.IsValid)
                board.AddTip(LongCastling.Coord, this);
        }

        public void DisableCastling(bool isShortSide)
        {
            if (isShortSide)
                ShortCastling.IsValid = false;
            else
                LongCastling.IsValid = false;
        }
    }

    public class Knight : OneGridChessPiece
    {
        public Knight(bool _isWhite) : base(_isWhite, "N")
        {
            Directions = Dir.Knight();
        }
    }

    public class Castling
    {
        public Coord Coord;
        public bool IsValid = true;

        public Castling(Coord coord)
        {
            Coord = coord;
        }
    }
}
