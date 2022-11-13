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
using System.DirectoryServices;

namespace ChessGame.ChessPieces
{
    abstract class ChessPiece
    {
        protected ChessPiece(bool _isWhite, string _name)
        {
            size = 100;
            isWhite = _isWhite;
            image = new Image()
            {
                //Stretch = Stretch.Fill,
                Width = size,
                Height = size,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            name = _name;
            SetImageSource();
            dirs = new Coords[] { new Coords(0, 0) };
        }

        public static double size; // image size
        public readonly bool isWhite; // White or black
        public Image image; // Image of chess
        public readonly string? name; // Abbreviation name
        protected Coords[] dirs; // Directions of move

        private void SetImageSource()
        {
            string fileName;
            if (isWhite)
            {
                fileName = $"img/w{name}.png"; // White
            }
            else
            {
                fileName = $"img/b{name}.png"; // Black
            }
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fileName, UriKind.Relative);
            bitmap.EndInit();
            image.Source = bitmap;
        }
        public bool IsSameColor(ChessPiece chess)
        {
            return !(isWhite ^ chess.isWhite);
        }
        public void FollowMousePosition(Point mousePosition)
        {
            image.Margin = new Thickness(mousePosition.X - size / 2, mousePosition.Y - size / 2, 0, 0);
        }


        public abstract void Rule(ChessBoard board); // Find the valid move

        protected static class Dir
        {
            public static Coords[] Rook()
            { // Plus
                return AllDirectios(new Coords(0, 1));
            }
            public static Coords[] Bishop()
            { // X
                return AllDirectios(new Coords(1, 1));
            }
            public static Coords[] King()
            { // Star
                return Rook().Union(Bishop()).ToArray(); // Plus + X = Star
            }
            public static Coords[] Knight()
            {
                return AllDirectios(new Coords(1, 2)).Union(AllDirectios(new Coords(2, 1))).ToArray();
            }
            // Here other directions can be defined to generate self-created chess pieces
            // private static Coords[] CustomDir1() {
            //      ...
            // }

            private static Coords[] AllDirectios(Coords c)
            { // All directions with a ninety-degree angle
                Coords[] dirs = new Coords[4];
                dirs[0] = c;
                for (int i = 0; i < 3; i++)
                {
                    dirs[i + 1] = dirs[i].GetRotate90();
                }
                return dirs;
            }
        }
    }

}
