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
        public Stack<string> history = new Stack<string>();

        public ChessBoard(Grid _grid) {
            this.grid = _grid;
            InitializeCurrentSituation();
        }

        private void InitializeCurrentSituation() {
            this.currentSituation = new ChessPiece[8, 8];

            bool isWhite = true;
            Add(7, 0, new Rook(isWhite));
            Add(7, 1, new Knight(isWhite));
            Add(7, 2, new Bishop(isWhite));
            Add(7, 3, new Queen(isWhite));
            Add(7, 4, new King(isWhite));
            Add(7, 5, new Bishop(isWhite));
            Add(7, 6, new Knight(isWhite));
            Add(7, 7, new Rook(isWhite));
            for (int j = 0; j < 8; j++) {
                Add(6, j, new Pawn(isWhite));
            }
            isWhite = false;
            Add(0, 0, new Rook(isWhite));
            Add(0, 1, new Knight(isWhite));
            Add(0, 2, new Bishop(isWhite));
            Add(0, 3, new Queen(isWhite));
            Add(0, 4, new King(isWhite));
            Add(0, 5, new Bishop(isWhite));
            Add(0, 6, new Knight(isWhite));
            Add(0, 7, new Rook(isWhite));
            for (int j = 0; j < 8; j++) {
                Add(1, j, new Pawn(isWhite));
            }
        }

        public void PickUp(Grid air, Point mousePosition) { // Take the chess piece from the board into the air
            Image img = holdChess.image;

            // Remove from board
            grid.Children.Remove(img);

            // Add to air
            double x = mousePosition.X - ChessPiece.Size / 2;
            double y = mousePosition.Y - ChessPiece.Size / 2;
            air.Children.Add(img);
            img.Margin = new Thickness(x, y, 0, 0);
            Grid.SetRow(img, 0);
            Grid.SetColumn(img, 0);
        }
        public void PutDown(Grid air, Point mousePosition) {
            // Remove from air
            air.Children.Remove(holdChess.image);

            // Add to board
            int row = (int)(mousePosition.Y / ChessPiece.Size);
            int col = (int)(mousePosition.X / ChessPiece.Size);
            Add(row, col, holdChess);
            holdChess = null;
        }
        public void Add(int row, int col, ChessPiece chess) { // Add to board
            Image img = chess.image;
            img.Margin = new Thickness(0, 0, 0, 0);
            grid.Children.Add(img);
            Grid.SetRow(img, row);
            Grid.SetColumn(img, col);
            this.currentSituation[row, col] = chess;
        }
        public bool isOutOfBound(Point pos) {
            int d = 10; // redundant
            return pos.X <= d || pos.X >= grid.Width - d || pos.Y <= d || pos.Y >= grid.Height - d;
        }
    }
}
