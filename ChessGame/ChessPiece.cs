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

namespace ChessGame
{
    //internal interface IChessPiece
    //{
    //    //int X { get; set; }
    //    Image image { get; set; } // image of a chess
    //    bool isWhite { get; set; }
    //    void Rule();
    //}
    internal abstract class ChessPiece
    {
        public ChessPiece(bool _isWhite) {
            isWhite = _isWhite;
            image = new Image() {
                //Stretch = Stretch.Fill,
                Width = Size,
                Height = Size,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
        }

        public static double Size = 100; // image size
        public bool isWhite;
        public Image image; // Image of chess
        public string? name; // Abbreviation name
        protected Coords[] dirs; // Directions of move

        protected void SetImageSource() {
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
        public King(bool _isWhite) : base (_isWhite) {
            name = "K";
            SetImageSource();

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
                board.AddTip(board.currentCoord + bias);
            }
        }
    }

    class Queen : ChessPiece
    {
        public Queen(bool _isWhite) : base(_isWhite) {
            name = "Q";
            SetImageSource();

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
                Coords coord = board.currentCoord + bias;
                while (board.AddTip(coord)) {
                    coord += bias;
                }
            }
        }
    }

    class Rook : ChessPiece
    {
        public Rook(bool _isWhite) : base(_isWhite) {
            name = "R";
            SetImageSource();

            dirs = new Coords[4];
            dirs[0] = new Coords(0, 1);
            for (int i = 0; i < 3; i++) {
                dirs[i + 1] = dirs[i].GetRotate90();
            }
        }

        public override void Rule(ChessBoard board) {
            foreach (var bias in dirs) {
                Coords coord = board.currentCoord + bias;
                while (board.AddTip(coord)) {
                    coord += bias;
                }
            }
        }
    }

    class Bishop : ChessPiece
    {
        public Bishop(bool _isWhite) : base(_isWhite) {
            name = "B";
            SetImageSource();

            dirs = new Coords[4];
            dirs[0] = new Coords(1, 1);
            for (int i = 0; i < 3; i++) {
                dirs[i + 1] = dirs[i].GetRotate90();
            }
        }
        public override void Rule(ChessBoard board) {
            foreach (var bias in dirs) {
                Coords coord = board.currentCoord + bias;
                while (board.AddTip(coord)) {
                    coord += bias;
                }
            }
        }
    }

    class Knight : ChessPiece
    {
        public Knight(bool _isWhite) : base(_isWhite) {
            name = "N";
            SetImageSource();

            // Knight's 8 move
            dirs = new Coords[8];
            dirs[0] = new Coords(1, 2);
            dirs[4] = new Coords(2, 1);
            for (int i = 0; i < 3; i++) {
                dirs[i + 1] = dirs[i].GetRotate90();
                dirs[i + 5] = dirs[i + 4].GetRotate90();
            }
        }

        private readonly Coords[] dirs = new Coords[8]; // directions

        public override void Rule(ChessBoard board) {
            foreach (var bias in dirs) {
                board.AddTip(board.currentCoord + bias);
            }
        }
    }
    class Pawn : ChessPiece
    {
        public Pawn(bool _isWhite) : base(_isWhite) {
            name = "";
            SetImageSource();
        }
        public override void Rule(ChessBoard board) {
        }
    }
}
