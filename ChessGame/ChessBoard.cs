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

namespace ChessGame
{
    internal class ChessBoard
    {
        public ChessBoard() {
            this.currentSituation = new ChessPiece[8, 8];
        }

        public ChessPiece[,] currentSituation;
        public ChessPiece holdChess;
        public Grid grid; // corresponding grid item in MainWindow
        public void PickUp(Grid air, Point mousePosition) { // Take the chess piece from the board into the air
            //currentSituation[row, col] = null;
            Image img = holdChess.image;

            // Remove from board
            grid.Children.Remove(img);

            // Add to air
            air.Children.Add(img);
            double x = mousePosition.X - ChessPiece.Size / 2;
            double y = mousePosition.Y - ChessPiece.Size / 2;
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
            currentSituation[row, col] = chess;
        }
        public bool isOutOfBound(Point pos) {
            int d = 10; // redundant
            return pos.X <= d || pos.X >= grid.Width - d || pos.Y <= d || pos.Y >= grid.Height - d;
        }
    }
}
