using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace ChessGame.ChessPieces
{
    class Pawn : ChessPiece
    {
        public Pawn(bool _isWhite) : base(_isWhite, "P")
        {
            // White pawn move up
            Directions = new Coord[] 
            {
                new Coord(-1, 0),
                new Coord(-1, -1),
                new Coord(-1, 1),
            };
            // Black pawn move down
            if (!IsWhite) 
                Directions[0].Row = Directions[1].Row = Directions[2].Row = 1;
        }

        public override void AddTipToBoard(ChessBoard board)
        {
            Coord coord = Coord;
            ChessPiece? targetChess;

            // Forward
            coord += Directions[0]; // Move one grid
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
                        coord += Directions[0]; // Move one more grid
                        targetChess = board.GetChessOn(coord);
                        if (targetChess == null) // No Chess there
                            board.TipIcon[coord.Row, coord.Col].Visibility = Visibility.Visible;
                    }
                }
            }

            // Both sides
            for (int i = 1; i < 3; i++)
            {
                coord = Coord + Directions[i];
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
