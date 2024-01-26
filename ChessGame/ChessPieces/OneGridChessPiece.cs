using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.ChessPieces
{
    abstract class OneGridChessPiece : ChessPiece // Can move one number of grids. ex. King, Knight
    {
        protected OneGridChessPiece(bool isWhite, string name) : base(isWhite, name) { }

        public override void AddTipToBoard(ChessBoard board)
        {
            foreach (var bias in Directions)
                board.AddTip(board.PickUpCoord + bias, this);
        }
    }

    class King : OneGridChessPiece, ISpecialMovePiece
    {
        public bool HasMoved { get; set; }
        public Coord LongCastlingCoord { get; }
        public Coord ShortCastlingCoord { get; }

        public King(bool isWhite) : base(isWhite, "K")
        {
            Directions = Dir.King();
            LongCastlingCoord = IsWhite ? new Coord(7, 2) : new Coord(0, 5);
            ShortCastlingCoord = IsWhite ? new Coord(7, 6) : new Coord(0, 1);
        }

        public override void AddTipToBoard(ChessBoard board)
        {
            base.AddTipToBoard(board);
            // Castling
            if (board.CanShortCastling)
                board.AddTip(ShortCastlingCoord, this);
            if (board.CanLongCastling)
                board.AddTip(LongCastlingCoord, this);
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
