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
        public ChessBoard(Grid _grid) {
            this.grid = _grid;
            this.currentSituation = new ChessPiece[SIZE, SIZE];
            Add(new Coords(0, 4), new King(true));
            Add(new Coords(7, 4), new King(false));
            InitializeTipIcon();
            //InitializeCurrentSituation();
        }

        private const int SIZE = 8; // Number of grids on a side
        public bool isWhiteFirst = true;
        //public bool isHoldingChess = false;
        public Grid grid; // Corresponding grid item in MainWindow
        public TextBlock[,] tipIcon = new TextBlock[SIZE, SIZE]; // The tips when moving the chess
        public Coords currentCoord; // Current Coordinates when mouse pick up and put down a chess
        public ChessPiece?[,] currentSituation; // Current game situation of the board
        public ChessPiece? holdChess; // Current holding chess
        //public Stack<Coords> eatCoords = new Stack<Coords>(); // The coordinates where you can eat chess
        //public Stack<Coords> moveCoords = new Stack<Coords>(); // The coordinates where you can move chess

        private void InitializeTipIcon() {
            // Add tip icon
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    tipIcon[i, j] = new TextBlock() {
                        TextWrapping = TextWrapping.Wrap,
                        Opacity = 0.5,
                        
                    };
                    resetTipIcon(tipIcon[i, j]);
                    grid.Children.Add(tipIcon[i, j]);
                    Grid.SetRow(tipIcon[i, j], i);
                    Grid.SetColumn(tipIcon[i, j], j);
                }
            }
        }

        public void resetTipIcon(TextBlock t) {
            t.Background = Brushes.Black;//"#FF030315"
            t.Visibility = Visibility.Hidden;
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

        // Add the chess to board corresponding to the coordinates
        public void Add(Coords coord, ChessPiece chess) { 
            Image img = chess.image;
            img.Margin = new Thickness(0, 0, 0, 0);
            grid.Children.Add(img);
            Grid.SetRow(img, coord.row);
            Grid.SetColumn(img, coord.col);
            this.currentSituation[coord.row, coord.col] = chess;
        }

        public bool isOutOfBound(Point pos) {
            return pos.X < 0|| pos.X >= grid.Width || pos.Y < 0 || pos.Y >= grid.Height;
        }

        public bool IsOutOfBound(Coords coord) {
            return coord.col < 0 || coord.col >= SIZE || coord.row < 0 || coord.row >= SIZE;
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

        /// <summary>
        /// Set the position of the mouse cursor on the chessboard coordinates
        /// </summary>
        /// <param name="mousePosition"> Relative to the top left corner of board </param>
        public void SetPosition(Point mousePosition) {
            currentCoord.row = (int)(mousePosition.Y / ChessPiece.Size); // Row index;
            currentCoord.col = (int)(mousePosition.X / ChessPiece.Size); // Column index
        }
    }
}
