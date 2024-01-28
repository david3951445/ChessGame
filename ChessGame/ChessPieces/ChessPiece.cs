using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;

namespace ChessGame.ChessPieces
{
    public enum ChessPieceType
    {
        Empty,
        King,
        Queen,
        Rook,
        Bishop,
        Knight,
        Pawn
    }

    public abstract class ChessPiece
    {
        public static double Size = 100; // image size

        public Coord Coord { get; set; }
        public bool IsWhite { get; } // White or black
        public string Name { get; } // Abbreviation name
        public Image Image { get; } // Image of chess
        protected Coord[] Directions { get; init; } // Directions of move

        protected ChessPiece(bool isWhite, string name)
        {
            IsWhite = isWhite;
            Name = name;
            Image = MediaManager.GetChessImage(isWhite, name, Size);
            Directions = new Coord[0];
        }

        public bool IsSameColor(ChessPiece chess) => IsWhite == chess.IsWhite;

        public void FollowMousePosition(Point mousePosition) => Image.Margin = new Thickness(mousePosition.X - Size / 2, mousePosition.Y - Size / 2, 0, 0);

        public abstract void AddTipToBoard(ChessBoard board); // Find the valid move

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
