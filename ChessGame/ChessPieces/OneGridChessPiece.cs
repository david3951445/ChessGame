using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Documents;

namespace ChessGame.ChessPieces
{
    public abstract class OneGridChessPiece : ChessPiece // Can move one number of grids. ex. King, Knight
    {
        protected OneGridChessPiece(bool isWhite, string name) : base(isWhite, name) { }

        public override void AddTipToBoard(ChessBoard board)
        {
            foreach (var bias in Directions)
                board.AddTip(Coord + bias, this);
        }

        public override IEnumerable<ChessPiece> CapturedChesses(ChessBoard board) => Directions
            .Select(coord => board.GetChessOn(coord))
            .NonNull()
            .Where(seenChess => !IsSameColor(seenChess));
    }

    public class Knight : OneGridChessPiece
    {
        public Knight(bool _isWhite) : base(_isWhite, "N")
        {
            Directions = Dir.Knight();
        }
    }
}
