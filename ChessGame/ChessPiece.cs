using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Input;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.Xml.Linq;

namespace ChessGame
{
    internal abstract class ChessPiece
    {
        protected ChessPiece(bool _isWhite, string _name) {
            isWhite = _isWhite;
            image = new Image() {
                //Stretch = Stretch.Fill,
                Width = size,
                Height = size,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            name = _name;
            SetImageSource();
        }

        public static double size; // image size
        public readonly bool isWhite; // White or black
        public Image image; // Image of chess
        public readonly string? name; // Abbreviation name
        protected Coords[] dirs; // Directions of move

        private void SetImageSource() {
            string fileName;
            if (isWhite) {
                fileName = $"img/w{name}.png"; // White
            }
            else {
                fileName = $"img/b{name}.png"; // Black
            }
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fileName, UriKind.Relative);
            bitmap.EndInit();
            this.image.Source = bitmap;
        }
        public bool IsSameColor(ChessPiece chess) {
            return !(isWhite ^ chess.isWhite);
        }
        public void FollowMousePosition(Point mousePosition) {
            image.Margin = new Thickness(mousePosition.X - size / 2, mousePosition.Y - size / 2, 0, 0);
        }


        public abstract void Rule(ChessBoard board); // Find the valid move
    }

    abstract class AnyGridChessPiece : ChessPiece // Queen, Rook. Bishop can move any number of grids
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

    abstract class OneGridChessPiece : ChessPiece // Queen, Rook. Bishop can move any number of grids
    {
        protected OneGridChessPiece(bool _isWhite, string _name) : base(_isWhite, _name) { }

        public override void Rule(ChessBoard board) {
            foreach (var bias in dirs) {
                board.AddTip(board.pickUpCoord + bias, this);
            }
        }
    }

    class King : OneGridChessPiece
    {
        public King(bool _isWhite) : base(_isWhite, "K") {
            // King's 8 move
            dirs = new Coords[8];
            dirs[0] = new Coords(0, 1);
            dirs[4] = new Coords(1, 1);
            for (int i = 0; i < 3; i++) {
                dirs[i + 1] = dirs[i].GetRotate90();
                dirs[i + 5] = dirs[i + 4].GetRotate90();
            }
        }

        public override void Rule(ChessBoard board) {
            base.Rule(board);

            // Castling
        }
    }

    class Queen : AnyGridChessPiece
    {
        public Queen(bool _isWhite) : base(_isWhite, "Q") {
            // Qing's 8 direction
            dirs = new Coords[8];
            dirs[0] = new Coords(0, 1);
            dirs[4] = new Coords(1, 1);
            for (int i = 0; i < 3; i++) {
                dirs[i + 1] = dirs[i].GetRotate90();
                dirs[i + 5] = dirs[i + 4].GetRotate90();
            }
        }
    }

    class Rook : AnyGridChessPiece
    {
        public Rook(bool _isWhite) : base(_isWhite, "R") {
            dirs = new Coords[4];
            dirs[0] = new Coords(0, 1);
            for (int i = 0; i < 3; i++) {
                dirs[i + 1] = dirs[i].GetRotate90();
            }
        }
    }

    class Bishop : AnyGridChessPiece
    {
        public Bishop(bool _isWhite) : base(_isWhite, "B") {
            dirs = new Coords[4];
            dirs[0] = new Coords(1, 1);
            for (int i = 0; i < 3; i++) {
                dirs[i + 1] = dirs[i].GetRotate90();
            }
        }
    }

    class Knight : OneGridChessPiece
    {
        public Knight(bool _isWhite) : base(_isWhite, "N") {
            // Knight's 8 move
            dirs = new Coords[8];
            dirs[0] = new Coords(1, 2);
            dirs[4] = new Coords(2, 1);
            for (int i = 0; i < 3; i++) {
                dirs[i + 1] = dirs[i].GetRotate90();
                dirs[i + 5] = dirs[i + 4].GetRotate90();
            }
        }
    }

    class Pawn : ChessPiece
    {
        public Pawn(bool _isWhite) : base(_isWhite, "P") {
            dirs = new Coords[4];
            dirs[0] = new Coords(-1, 0);
            dirs[1] = new Coords(-1, -1);
            dirs[2] = new Coords(-1, 1);
            dirs[3] = new Coords(-2, 0);
            if (!isWhite) { // Black pawn move down
                dirs[0].row = dirs[1].row = dirs[2].row = 1;
                dirs[3].row = 2;
            }
        }

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
