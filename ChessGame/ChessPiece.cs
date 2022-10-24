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
        public ChessPiece() {
            image = new Image() {
                //Stretch = Stretch.Fill,
                Width = Size,
                Height = Size,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            image.MouseRightButtonDown += Image_MouseRightButtonDown;
            isWhite = true;
        }

        private void Image_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            throw new NotImplementedException();
        }

        public static int Size = 100; // image size
        public Image image;
        public bool isWhite;

        public void Move() {

        }
        protected void SetImageSource(string fileName) {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fileName, UriKind.Relative);
            bitmap.EndInit();
            this.image.Source = bitmap;
        }
        abstract public bool Rule();
    }
    class King : ChessPiece
    {
        public King() {
            string fileName = "img/bk.png"; // Black King
            if (isWhite) {
                fileName = "img/wk.png"; // White King
            }
            this.SetImageSource(fileName);
        }

        //public int X { get; set; }

        public override bool Rule() {
            return true;
        }
    }
}
