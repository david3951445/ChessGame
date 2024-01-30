using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            if (!board.IsOutOfBound(coord))
            {
                targetChess = board.GetChessOn(coord);
                if (targetChess == null) // No Chess there
                {
                    board.TipIcon[coord.Row, coord.Col].Visibility = Visibility.Visible;

                    // Forward, the first move can go one more grid
                    bool isFirstMove = coord.Row == 1 && !IsWhite || coord.Row == 6 && IsWhite; // white or black pawn is in the initial position
                    if (isFirstMove)
                    {
                        coord += _foward; // Move one more grid
                        targetChess = board.GetChessOn(coord);
                        if (targetChess == null) // No Chess there
                            board.TipIcon[coord.Row, coord.Col].Visibility = Visibility.Visible;
                    }
                }
            }

            // Both sides
            base.AddTipToBoard(board);

            // En passant (In passing pawn)
        }
    }
}
