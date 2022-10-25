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

namespace ChessGame
{
    //internal interface IChessPiece
    //{
    //    //int X { get; set; }
    //    Image image { get; set; } // image of a chess
    //    bool isWhite { get; set; }
    //    bool Rule();
    //}
    abstract class ChessPiece
    {
        public static double Size = 100; // image size
        public bool isWhite;
        public Image image;

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

        private void Image_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            //throw new NotImplementedException();
        }
        protected void SetImageSource(string fileName) {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fileName, UriKind.Relative);
            bitmap.EndInit();
            this.image.Source = bitmap;
        }

        abstract public bool Rule(); // Chess move rule
    }
    class King : ChessPiece
    {
        public King(bool _isWhite) : base (_isWhite) {
            if (isWhite) {
                SetImageSource("img/wk.png"); // White King
            }
            else {
                SetImageSource("img/bk.png"); // Black King
            }
        }
        public override bool Rule() {
            return true;
        }
    }
    class Queen : ChessPiece
    {
        public Queen(bool _isWhite) : base(_isWhite) {
            if (isWhite) {
                SetImageSource("img/wq.png"); // White King
            }
            else {
                SetImageSource("img/bq.png"); // Black King
            }
        }
        public override bool Rule() {
            return true;
        }
    }
    class Rook : ChessPiece
    {
        public Rook(bool _isWhite) : base(_isWhite) {
            if (isWhite) {
                SetImageSource("img/wr.png"); // White King
            }
            else {
                SetImageSource("img/br.png"); // Black King
            }
        }
        public override bool Rule(){
            return true;
        }
    }
    class Bishop : ChessPiece
    {
        public Bishop(bool _isWhite) : base(_isWhite) {
            if (isWhite) {
                SetImageSource("img/wb.png"); // White King
            }
            else {
                SetImageSource("img/bb.png"); // Black King
            }
        }
        public override bool Rule() {
            return true;
        }
    }
    class Knight : ChessPiece
    {
        public Knight(bool _isWhite) : base(_isWhite) {
            if (isWhite) {
                SetImageSource("img/wn.png"); // White King
            }
            else {
                SetImageSource("img/bn.png"); // Black King
            }
        }
        public override bool Rule() {
            return true;
        }
    }
    class Pawn : ChessPiece
    {
        public Pawn(bool _isWhite) : base(_isWhite) {
            if (isWhite) {
                SetImageSource("img/wp.png"); // White King
            }
            else {
                SetImageSource("img/bp.png"); // Black King
            }
        }
        public override bool Rule() {
            return true;
        }
    }
}
