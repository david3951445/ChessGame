using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Diagnostics;
using System.ComponentModel;

namespace ChessGame
{
    internal class ChessBoard
    {
        public Grid? grid; // corresponding grid item in MainWindow
        public ChessPiece?[,] currentSituation;
        public ChessPiece? holdChess;
        public List<string> standardHistory = new List<string>();
        public List<Coords> history = new List<Coords>();

        public ChessBoard(Grid _grid) {
            this.grid = _grid;
            this.currentSituation = new ChessPiece[8, 8];
            Add(new Coords(7, 4), new King(true));
            //currentSituation[7, 4].image.IsEnabled = false;
            //InitializeCurrentSituation();
        }

        private void InitializeCurrentSituation() {
            ChessPiece.Size = grid.Width / 8;

            bool isWhite = true;
            Add(new Coords(7, 0), new Rook(isWhite));
            Add(new Coords(7, 1), new Knight(isWhite));
            Add(new Coords(7, 2), new Bishop(isWhite));
            Add(new Coords(7, 3), new Queen(isWhite));
            Add(new Coords(7, 4), new King(isWhite));
            Add(new Coords(7, 5), new Bishop(isWhite));
            Add(new Coords(7, 6), new Knight(isWhite));
            Add(new Coords(7, 7), new Rook(isWhite));
            for (int j = 0; j < 8; j++) {
                Add(new Coords(6, j), new Pawn(isWhite));
            }
            isWhite = false;
            Add(new Coords(0, 0), new Rook(isWhite));
            Add(new Coords(0, 1), new Knight(isWhite));
            Add(new Coords(0, 2), new Bishop(isWhite));
            Add(new Coords(0, 3), new Queen(isWhite));
            Add(new Coords(0, 4), new King(isWhite));
            Add(new Coords(0, 5), new Bishop(isWhite));
            Add(new Coords(0, 6), new Knight(isWhite));
            Add(new Coords(0, 7), new Rook(isWhite));
            for (int j = 0; j < 8; j++) {
                Add(new Coords(1, j), new Pawn(isWhite));
            }
        }

        public void PickUp(Grid air, Point mousePosition) { // Take the chess piece from the board into the air
            //Image img = holdChess.image;

            //// Remove from board
            //grid.Children.Remove(img);

            //// Add to air
            //double x = mousePosition.X - ChessPiece.Size / 2;
            //double y = mousePosition.Y - ChessPiece.Size / 2;
            //air.Children.Add(img);
            //img.Margin = new Thickness(x, y, 0, 0);
            //Grid.SetRow(img, 0);
            //Grid.SetColumn(img, 0);
        }

        // Add to board
        public void Add(Coords c, ChessPiece chess) { 
            Image img = chess.image;
            img.Margin = new Thickness(0, 0, 0, 0);
            grid.Children.Add(img);
            Grid.SetRow(img, c.row);
            Grid.SetColumn(img, c.col);
            this.currentSituation[c.row, c.col] = chess;
        }

        public bool isOutOfBound(Point pos) {
            int d = 0; // redundant
            return pos.X <= d || pos.X >= grid.Width - d || pos.Y <= d || pos.Y >= grid.Height - d;
        }

        /// <summary>
        /// Get the position of the mouse cursor on the chessboard coordinates
        /// </summary>
        /// <param name="mousePosition"> Relative to the top left corner of board </param>
        /// <returns> Coordinate of board. output = (row index, column index) </returns>
        public Coords GetPosition(Point mousePosition) {
            return new Coords (
                (int)(mousePosition.Y / ChessPiece.Size), // Row index
                (int)(mousePosition.X / ChessPiece.Size) // Column index
            ); 
        }
    }
}
