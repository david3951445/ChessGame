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
using ChessGame.ChessPieces;

namespace ChessGame
{
    /*  Coord[,]
     *    0 1 2 3 4 5 6 7
     *  0
     *  1
     *  2
     *  3
     *  4
     *  5
     *  6
     *  7
     */
    internal class ChessBoard
    {
        private const int SIZE = 8; // Number of grids on a side
        public bool IsWhiteTurn = true;
        //public bool isHoldingChess = false;
        public TextBlock[,] TipIcon = new TextBlock[SIZE, SIZE]; // The tips when moving the chess
        public Coord PickUpCoord; // Current Coordinates when mouse pick up a chess
        public ChessPiece?[,] CurrentSituation; // Current game situation of the board
        public ChessPiece? HoldChess; // Current holding chess
        private Grid _grid; // Corresponding grid item of board in MainWindow
        //public Stack<Coords> eatCoords = new Stack<Coords>(); // The coordinates where you can eat chess
        //public Stack<Coords> moveCoords = new Stack<Coords>(); // The coordinates where you can move chess

        public ChessBoard(Grid _grid)
        {
            this._grid = _grid;
            ChessPiece.Size = this._grid.Width / SIZE;
            this.CurrentSituation = new ChessPiece[SIZE, SIZE];
            //Add(new Coords(0, 4), new Knight(true));
            //Add(new Coords(7, 4), new King(false));
            InitializeTipIcon();
            InitializeChessesPosition();
        }

        private void InitializeTipIcon()
        {
            // Add tip icon
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    var textBlock = new TextBlock()
                    {
                        TextWrapping = TextWrapping.Wrap,
                        Opacity = 0.5,
                    };
                    ResetTipIcon(textBlock);
                    _grid.Children.Add(textBlock);
                    Grid.SetRow(textBlock, i);
                    Grid.SetColumn(textBlock, j);
                    TipIcon[i, j] = textBlock;
                }
            }
        }

        public void ResetTipIcon(TextBlock t)
        {
            t.Background = Brushes.Black;//"#FF030315"
            t.Visibility = Visibility.Hidden;
        }

        private void InitializeChessesPosition()
        {
            bool isWhite = true;
            Add(new Coord(7, 0), new Rook(isWhite));
            Add(new Coord(7, 1), new Knight(isWhite));
            Add(new Coord(7, 2), new Bishop(isWhite));
            Add(new Coord(7, 3), new Queen(isWhite));
            Add(new Coord(7, 4), new King(isWhite));
            Add(new Coord(7, 5), new Bishop(isWhite));
            Add(new Coord(7, 6), new Knight(isWhite));
            Add(new Coord(7, 7), new Rook(isWhite));
            for (int j = 0; j < SIZE; j++)
                Add(new Coord(6, j), new Pawn(isWhite));
            isWhite = false;
            Add(new Coord(0, 0), new Rook(isWhite));
            Add(new Coord(0, 1), new Knight(isWhite));
            Add(new Coord(0, 2), new Bishop(isWhite));
            Add(new Coord(0, 3), new Queen(isWhite));
            Add(new Coord(0, 4), new King(isWhite));
            Add(new Coord(0, 5), new Bishop(isWhite));
            Add(new Coord(0, 6), new Knight(isWhite));
            Add(new Coord(0, 7), new Rook(isWhite));
            for (int j = 0; j < SIZE; j++)
            {
                Add(new Coord(1, j), new Pawn(isWhite));
            }
        }

        // Add the chess to board corresponding to the coordinates
        public void Add(Coord coord, ChessPiece chess)
        {
            Image img = chess.Image;
            img.Margin = new Thickness(0, 0, 0, 0);
            _grid.Children.Add(img);
            Grid.SetRow(img, coord.Row);
            Grid.SetColumn(img, coord.Col);
            CurrentSituation[coord.Row, coord.Col] = chess;
        }

        public bool IsOutOfBound(Point mousePos) => mousePos.X < 0 || mousePos.X >= _grid.Width || mousePos.Y < 0 || mousePos.Y >= _grid.Height;

        public bool IsOutOfBound(Coord boardCoord) => boardCoord.Col < 0 || boardCoord.Col >= SIZE || boardCoord.Row < 0 || boardCoord.Row >= SIZE;

        /// <summary>
        /// Set the position of the mouse cursor on the chessboard coordinates
        /// </summary>
        /// <param name="mousePosition"> Relative to the top left corner of board </param>
        public void SetPosition(Point mousePosition)
        {
            PickUpCoord.Row = (int)(mousePosition.Y / ChessPiece.Size); // Row index;
            PickUpCoord.Col = (int)(mousePosition.X / ChessPiece.Size); // Column index
        }

        /// <summary>
        /// Get the position of the mouse cursor on the chessboard coordinates
        /// </summary>
        /// <param name="mousePosition"> Relative to the top left corner of board </param>
        /// <returns> Coordinate of board. output = (row index, column index) </returns>
        public Coord GetPosition(Point mousePosition)
        {
            return new Coord(
                (int)(mousePosition.Y / ChessPiece.Size), // Row index
                (int)(mousePosition.X / ChessPiece.Size) // Column index
            );
        }
        /// <summary>
        /// Add tip to the given coord with the holding chess.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns>Return is for Queen, Rook, Bishop</returns>
        public bool AddTip(Coord coord, ChessPiece chess)
        {
            if (!IsOutOfBound(coord))
            { // Not out of bound
                ChessPiece? targetChess = CurrentSituation[coord.Row, coord.Col];
                if (targetChess == null)
                { // No Chess there (Non-eaten)
                    TipIcon[coord.Row, coord.Col].Visibility = Visibility.Visible;

                    return true;
                }
                else
                { // Is chess there (Eaten)
                    if (!chess.IsSameColor(targetChess))
                    { // Different color chess there
                        TipIcon[coord.Row, coord.Col].Visibility = Visibility.Visible;
                        TipIcon[coord.Row, coord.Col].Background = Brushes.Red;
                    }
                }
            }

            return false; // Out of bound or is chess there
        }

        public bool isValidMove()
        {
            //Debug.WriteLine(holdChess.isWhite);
            //Debug.WriteLine(isWhiteTurn);

            return true;
        }
    }
}
