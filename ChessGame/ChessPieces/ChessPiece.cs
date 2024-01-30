using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Collections.Generic;

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
        /// <summary>
        /// Directions of captured move
        /// </summary>
        public Coord[] Directions { get; init; }

        protected ChessPiece(bool isWhite, string name)
        {
            IsWhite = isWhite;
            Name = name;
            Image = MediaManager.GetChessImage(isWhite, name, Size);
            Directions = new Coord[0];
        }

        public bool IsSameColor(ChessPiece chess) => IsWhite == chess.IsWhite;

        public void FollowMousePosition(Point mousePosition) => Image.Margin = new Thickness(mousePosition.X - Size / 2, mousePosition.Y - Size / 2, 0, 0);

        /// <summary>
        /// Find the valid move
        /// </summary>
        /// <param name="board"></param>
        public abstract void AddTipToBoard(ChessBoard board);

        /// <summary>
        /// Find the chesses can be captured
        /// </summary>
        public abstract IEnumerable<ChessPiece> CapturedChesses(ChessBoard board);

        public static class Dir
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
            /// Star
            /// </summary>
            public static Coord[] King() => Rook().Union(Bishop()).ToArray(); // Plus + X = Star
            /// <summary>
            /// Knight
            /// </summary>
            public static Coord[] Knight() => FourDirectios(new Coord(1, 2)).Union(FourDirectios(new Coord(2, 1))).ToArray();
            /// <summary>
            /// Pawn
            /// </summary>
            public static Coord[] UpwardCapturedPawn = new Coord[] { new Coord(-1, -1), new Coord(-1, 1) };
            public static Coord[] DownwardCapturedPawn = UpwardCapturedPawn.Select(c => c.GetRotate180()).ToArray();

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

    public interface ISpecialCapture
    {

    }
}
