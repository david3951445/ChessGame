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
        public Pawn(bool _isWhite) : base(_isWhite, "P") {
            // White pawn
            dirs = new Coords[] {
                new Coords(-1, 0),
                new Coords(-1, -1),
                new Coords(-1, 1),
            };
            if (!isWhite) { // Black pawn move down
                dirs[0].row = dirs[1].row = dirs[2].row = 1;
            }
        }

        protected override Coords[] dirs { get; init; }


        public override void Rule(ChessBoard board) {
            Coords coord = board.pickUpCoord;
            ChessPiece? targetChess;

            // Forward
            coord += dirs[0]; // Move one grid
            if (!board.IsOutOfBound(coord)) {
                targetChess = board.currentSituation[coord.row, coord.col];
                if (targetChess == null) { // No Chess there
                    board.tipIcon[coord.row, coord.col].Visibility = Visibility.Visible;

                    // Forward, the first move can go one more grid
                    bool isFirstMove = board.pickUpCoord.row == 1 && !isWhite || board.pickUpCoord.row == 6 && isWhite; // white or black pawn is in the initial position
                    if (isFirstMove) {
                        coord += dirs[0]; // Move one more grid
                        targetChess = board.currentSituation[coord.row, coord.col];
                        if (targetChess == null) { // No Chess there
                            board.tipIcon[coord.row, coord.col].Visibility = Visibility.Visible;
                        }
                    }
                }
            }

            // Both sides
            for (int i = 1; i < 3; i++) {
                coord = board.pickUpCoord + dirs[i];
                if (!board.IsOutOfBound(coord)) {
                    targetChess = board.currentSituation[coord.row, coord.col];
                    if (targetChess != null && !IsSameColor(targetChess)) { // Is Chess there && Different color chess there
                        board.tipIcon[coord.row, coord.col].Visibility = Visibility.Visible;
                        board.tipIcon[coord.row, coord.col].Background = Brushes.Red;
                    }
                }
            }

            // En passant
        }
    }

}
