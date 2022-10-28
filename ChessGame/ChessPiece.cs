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
using System.Drawing;

namespace ChessGame
{
    internal abstract class ChessPiece
    {
        public ChessPiece(bool _isWhite, string _name) {
            isWhite = _isWhite;
            image = new Image() {
                //Stretch = Stretch.Fill,
                Width = Size,
                Height = Size,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            name = _name;
            SetImageSource();
        }

        public static double Size = 100; // image size
        public bool isWhite; // Whit or black
        public Image image; // Image of chess
        public string? name; // Abbreviation name
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

        abstract public void Rule(ChessBoard board); // Find the valid move
    }

    class King : ChessPiece
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
            foreach (var bias in dirs) {
                board.AddTip(board.pickUpCoord + bias);
            }
        }
    }

    class Queen : ChessPiece
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

        public override void Rule(ChessBoard board) {
            foreach (var bias in dirs) {
                Coords coord = board.pickUpCoord + bias;
                while (board.AddTip(coord)) {
                    coord += bias;
                }
            }
        }
    }

    class Rook : ChessPiece
    {
        public Rook(bool _isWhite) : base(_isWhite, "R") {
            dirs = new Coords[4];
            dirs[0] = new Coords(0, 1);
            for (int i = 0; i < 3; i++) {
                dirs[i + 1] = dirs[i].GetRotate90();
            }
        }

        public override void Rule(ChessBoard board) {
            foreach (var bias in dirs) {
                Coords coord = board.pickUpCoord + bias;
                while (board.AddTip(coord)) {
                    coord += bias;
                }
            }
        }
    }

    class Bishop : ChessPiece
    {
        public Bishop(bool _isWhite) : base(_isWhite, "B") {
            dirs = new Coords[4];
            dirs[0] = new Coords(1, 1);
            for (int i = 0; i < 3; i++) {
                dirs[i + 1] = dirs[i].GetRotate90();
            }
        }
        public override void Rule(ChessBoard board) {
            foreach (var bias in dirs) {
                Coords coord = board.pickUpCoord + bias;
                while (board.AddTip(coord)) {
                    coord += bias;
                }
            }
        }
    }

    class Knight : ChessPiece
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

        public override void Rule(ChessBoard board) {
            foreach (var bias in dirs) {
                board.AddTip(board.pickUpCoord + bias);
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
            coord += dirs[0];
            targetChess = board.currentSituation[coord.row, coord.col];
            if (targetChess == null) { // No Chess there
                board.tipIcon[coord.row, coord.col].Visibility = Visibility.Visible;
            }

            // Forward, first move
            bool isFirstMove = board.pickUpCoord.row == 1 && !isWhite || board.pickUpCoord.row == 6 && isWhite;
            if (isFirstMove) {
                coord += dirs[0];
                targetChess = board.currentSituation[coord.row, coord.col];
                if (targetChess == null) { // No Chess there
                    board.tipIcon[coord.row, coord.col].Visibility = Visibility.Visible;
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

        }
    }
}
