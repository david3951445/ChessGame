using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ChessGame.ChessPieces
{
    class Pawn : OneGridChessPiece
    {
        private Coord _foward;

        public Pawn(bool _isWhite) : base(_isWhite, "P")
        {
            // White pawn move up
            _foward = _isWhite ? new Coord(-1, 0) : new Coord(1, 0);
            Directions = _isWhite ? Dir.UpwardCapturedPawn : Dir.DownwardCapturedPawn;
        }

        public override void AddTipToBoard(ChessBoard board)
        {
            Coord coord = Coord;
            ChessPiece? targetChess;

            // Forward
            coord += _foward; // Move one grid
            targetChess = board.GetChessOn(coord);
            if (targetChess == null) // No Chess there
            {
                board.TipIcon[coord.Row, coord.Col].Visibility = Visibility.Visible;

                // Forward, the first move can go one more grid
                bool isFirstMove = coord.Row == 2 && !IsWhite || coord.Row == 5 && IsWhite; // white or black pawn is in the initial position
                if (isFirstMove)
                {
                    coord += _foward; // Move one more grid
                    targetChess = board.GetChessOn(coord);
                    if (targetChess == null) // No Chess there
                        board.TipIcon[coord.Row, coord.Col].Visibility = Visibility.Visible;
                }
            }

            // Both sides
            foreach (var bias in Directions) 
            {
                coord = Coord + bias;
                if (!board.IsOutOfBound(coord))
                {
                    targetChess = board.GetChessOn(coord);
                    if (targetChess != null && !IsSameColor(targetChess))
                    { // Is Chess there && Different color chess there
                        board.TipIcon[coord.Row, coord.Col].Visibility = Visibility.Visible;
                        board.TipIcon[coord.Row, coord.Col].Background = Brushes.Red;
                    }
                }
            }

            // En passant (In passing pawn)
        }
    }
}
