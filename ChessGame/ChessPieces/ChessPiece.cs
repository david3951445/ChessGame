﻿using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;

namespace ChessGame.ChessPieces
{
    enum ChessPieceType
    {
        Empty,
        King,
        Queen,
        Rook,
        Bishop,
        Knight,
        Pawn
    }

    abstract class ChessPiece
    {
        public static double Size; // image size

        public string? Name { get; } // Abbreviation name
        public bool IsWhite { get; } // White or black
        public Image Image { get; } // Image of chess
        protected Coord[] Directions { get; init; } // Directions of move

        protected ChessPiece(bool isWhite, string name)
        {
            Size = 100;
            IsWhite = isWhite;
            Image = new Image()
            {
                //Stretch = Stretch.Fill,
                Width = Size,
                Height = Size,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            Name = name;
            Directions = new Coord[0];
            SetImageSource();
        }


        private void SetImageSource()
        {
            string fileName = IsWhite ? $"img/w{Name}.png" : $"img/b{Name}.png";
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fileName, UriKind.Relative);
            bitmap.EndInit();
            Image.Source = bitmap;
        }

        public bool IsSameColor(ChessPiece chess) => IsWhite == chess.IsWhite;

        public void FollowMousePosition(Point mousePosition) => Image.Margin = new Thickness(mousePosition.X - Size / 2, mousePosition.Y - Size / 2, 0, 0);

        public abstract void Rule(ChessBoard board); // Find the valid move

        protected static class Dir
        {
            /// <summary>
            /// Plus
            /// </summary>
            public static Coord[] Rook() => FourDirectios(new Coord(0, 1));
            /// <summary>
            /// X
            /// </summary>
            public static Coord[] Bishop() => FourDirectios(new Coord(1, 1));
            /// <summary>
            /// Start
            /// </summary>
            public static Coord[] King() => Rook().Union(Bishop()).ToArray(); // Plus + X = Star
            public static Coord[] Knight() => FourDirectios(new Coord(1, 2)).Union(FourDirectios(new Coord(2, 1))).ToArray();
            // Here other directions can be defined to generate self-created chess pieces
            // private static Coords[] CustomDir1() {
            //      ...
            // }

            private static Coord[] FourDirectios(Coord c)
            { // All directions with a ninety-degree angle
                Coord[] dirs = new Coord[4];
                dirs[0] = c;
                for (int i = 0; i < 3; i++)
                    dirs[i + 1] = dirs[i].GetRotate90();
                return dirs;
            }
        }
    }

}