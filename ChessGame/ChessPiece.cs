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
            image.MouseRightButtonDown += Image_MouseRightButtonDown;
        }

        public static double Size = 100; // image size
        public bool isWhite;
        public Image image;
        public string? name;

        private void Image_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            //throw new NotImplementedException();
        }
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
        protected bool IsSameColor(ChessPiece chess) {
            return true;
        }

        abstract public void Rule(ChessBoard board); // Find the valid move
    }
    class King : ChessPiece
    {
        public King(bool _isWhite) : base (_isWhite) {
            name = "K";
            SetImageSource();

            // King's 8 move
            dirs[0] = new Coords(0, 1);
            dirs[4] = new Coords(1, 1);
            for (int i = 0; i < 3; i++) {
                dirs[i + 1] = dirs[i].GetRotate90();
                dirs[i + 5] = dirs[i + 4].GetRotate90();
            }
        }

        private readonly Coords[] dirs = new Coords[8]; // directions

        public override void Rule(ChessBoard board) {           
            

            Coords origin = board.currentCoord;
            foreach (var bias in dirs) {
                Trace.WriteLine(bias);

                Coords coord = origin + bias;
                if (!board.IsOutOfBound(coord)) { // Not out of bound
                    if (board.currentSituation[coord.row, coord.col] == null) { // No Chess there
                        board.tipIcon[coord.row, coord.col].Visibility = Visibility.Visible;
                    }
                    else if (!IsSameColor(board.currentSituation[coord.row, coord.col])) { // Different color chess there
                        board.tipIcon[coord.row, coord.col].Visibility = Visibility.Visible;
                        board.tipIcon[coord.row, coord.col].Background = Brushes.Red;
                    }
                }
            }
            //board.moveCoords.Push(c);
            board.tipIcon[origin.row, origin.col].Visibility = Visibility.Visible;
            origin.col = 2;
            //board.eatCoords.Push(c);
            board.tipIcon[origin.row, origin.col].Visibility = Visibility.Visible;

        }
    }
    class Queen : ChessPiece
    {
        public Queen(bool _isWhite) : base(_isWhite) {
            name = "Q";
            SetImageSource();
        }
        public override void Rule(ChessBoard board) {
        }
    }
    class Rook : ChessPiece
    {
        public Rook(bool _isWhite) : base(_isWhite) {
            name = "R";
            SetImageSource();
        }
        public override void Rule(ChessBoard board) {
        }
    }
    class Bishop : ChessPiece
    {
        public Bishop(bool _isWhite) : base(_isWhite) {
            name = "B";
            SetImageSource();
        }
        public override void Rule(ChessBoard board) {
        }
    }
    class Knight : ChessPiece
    {
        public Knight(bool _isWhite) : base(_isWhite) {
            name = "N";
            SetImageSource();
        }
        public override void Rule(ChessBoard board) {
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
