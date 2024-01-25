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
            // White pawn
            Directions = new Coord[] {
                new Coord(-1, 0),
                new Coord(-1, -1),
                new Coord(-1, 1),
            };
            if (!IsWhite)
            { // Black pawn move down
                Directions[0].row = Directions[1].row = Directions[2].row = 1;
            }
        }

        protected override Coord[] Directions { get; }


        public override void Rule(ChessBoard board)
        {
            Coord coord = board.pickUpCoord;
            ChessPiece? targetChess;

            // Forward
            coord += Directions[0]; // Move one grid
            if (!board.IsOutOfBound(coord))
            {
                targetChess = board.currentSituation[coord.row, coord.col];
                if (targetChess == null)
                { // No Chess there
                    board.tipIcon[coord.row, coord.col].Visibility = Visibility.Visible;

                    // Forward, the first move can go one more grid
                    bool isFirstMove = board.pickUpCoord.row == 1 && !IsWhite || board.pickUpCoord.row == 6 && IsWhite; // white or black pawn is in the initial position
                    if (isFirstMove)
                    {
                        coord += Directions[0]; // Move one more grid
                        targetChess = board.currentSituation[coord.row, coord.col];
                        if (targetChess == null)
                        { // No Chess there
                            board.tipIcon[coord.row, coord.col].Visibility = Visibility.Visible;
                        }
                    }
                }
            }

            // Both sides
            for (int i = 1; i < 3; i++)
            {
                coord = board.pickUpCoord + Directions[i];
                if (!board.IsOutOfBound(coord))
                {
                    targetChess = board.currentSituation[coord.row, coord.col];
                    if (targetChess != null && !IsSameColor(targetChess))
                    { // Is Chess there && Different color chess there
                        board.tipIcon[coord.row, coord.col].Visibility = Visibility.Visible;
                        board.tipIcon[coord.row, coord.col].Background = Brushes.Red;
                    }
                }
            }

            // En passant
        }
    }

}
