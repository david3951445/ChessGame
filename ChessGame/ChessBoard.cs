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
        public event EventHandler<ChessMovedEventArgs>? ChessAdded;
        public event EventHandler<ChessMovedEventArgs>? ChessRemoved;
        public event EventHandler<ChessMovedEventArgs>? ChessEaten;

        //public bool isHoldingChess = false;
        public TextBlock[,] TipIcon = new TextBlock[SIZE, SIZE]; // The tips when moving the chess. Also use to check valid moves.
        public Coord PickUpCoord; // Current Coordinates when mouse pick up a chess
        public ChessPiece? HoldChess; // Current holding chess
        public const int SIZE = 8; // Number of grids on a side
        //public Stack<Coords> eatCoords = new Stack<Coords>(); // The coordinates where you can eat chess
        //public Stack<Coords> moveCoords = new Stack<Coords>(); // The coordinates where you can move chess
        public History _history = new History();

        public bool IsWhiteTurn { get; set; }
        public King WhiteKing { get; private set; }
        public King BlackKing { get; private set; }
        public ChessPiece?[,] CurrentState { get; private set; } // Current game situation of the board
        public bool CanShortCastling { get; set; } = true;
        public bool CanLongCastling { get; set; } = true;

        public ChessBoard()
        {
            InitializeTipIcon();
        }

        private void InitializeTipIcon()
        {
            // Add tip icon
            for (int i = 0; i < SIZE; i++)
                for (int j = 0; j < SIZE; j++)
                {
                    var textBlock = new TextBlock()
                    {
                        TextWrapping = TextWrapping.Wrap,
                        Opacity = 0.5,
                    };
                    ResetTipIcon(textBlock);
                    TipIcon[i, j] = textBlock;
                }
        }

        public void StartNewGame()
        {
            InitializeChessesPosition();
            IsWhiteTurn = true;
            _history = new History();
        }

        public void ResetTipIcon(TextBlock t)
        {
            t.Background = Brushes.Black;//"#FF030315"
            t.Visibility = Visibility.Hidden;
        }

        private void InitializeChessesPosition()
        {
            CurrentState = new ChessPiece[SIZE, SIZE];

            bool isWhite = true;
            Add(new Coord(7, 0), new Rook(isWhite));
            Add(new Coord(7, 1), new Knight(isWhite));
            Add(new Coord(7, 2), new Bishop(isWhite));
            Add(new Coord(7, 3), new Queen(isWhite));
            WhiteKing = new King(isWhite);
            Add(new Coord(7, 4), WhiteKing);
            Add(new Coord(7, 5), new Bishop(isWhite));
            Add(new Coord(7, 6), new Knight(isWhite));
            Add(new Coord(7, 7), new Rook(isWhite) { IsShortSide = true });
            for (int j = 0; j < SIZE; j++)
                Add(new Coord(6, j), new Pawn(isWhite));

            isWhite = false;
            Add(new Coord(0, 0), new Rook(isWhite));
            Add(new Coord(0, 1), new Knight(isWhite));
            Add(new Coord(0, 2), new Bishop(isWhite));
            Add(new Coord(0, 3), new Queen(isWhite));
            BlackKing = new King(isWhite);
            Add(new Coord(0, 4), BlackKing);
            Add(new Coord(0, 5), new Bishop(isWhite));
            Add(new Coord(0, 6), new Knight(isWhite));
            Add(new Coord(0, 7), new Rook(isWhite) { IsShortSide = true });
            for (int j = 0; j < SIZE; j++)
                Add(new Coord(1, j), new Pawn(isWhite));
        }

        public Coord GetRookStartingCoord(bool isWhite, bool isShort)
        {
            int rank = isWhite ? 7 : 0;
            int file = isShort ? 7 : 0;
            return new Coord(rank, file);
        }

        // Add the chess to board corresponding to the coordinates
        public void Add(Coord coord, ChessPiece chessToAdd)
        {
            chessToAdd.Coord = coord;
            ChessAdded?.Invoke(this, new ChessMovedEventArgs(chessToAdd, coord));
            CurrentState[coord.Row, coord.Col] = chessToAdd;
        }

        public ChessPiece? Remove(Coord coord)
        {
            var chessToRemove = CurrentState[coord.Row, coord.Col];
            OnChessRemoved(chessToRemove, coord);
            CurrentState[coord.Row, coord.Col] = null;
            return chessToRemove;
        }

        public void Move(Coord startingCoord, Coord endCoord)
        {
            var chess = Remove(startingCoord);
            if (chess == null)
                return;
            Add(endCoord, chess);
        }

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
            if (IsOutOfBound(coord))
                return false;

            ChessPiece? targetChess = CurrentState[coord.Row, coord.Col];
            if (targetChess == null) // No Chess there (Non-eaten)
            {
                TipIcon[coord.Row, coord.Col].Visibility = Visibility.Visible;
                return true;
            }

            // Is chess there (Eaten)
            if (!chess.IsSameColor(targetChess)) // Different color chess there
            {
                TipIcon[coord.Row, coord.Col].Visibility = Visibility.Visible;
                TipIcon[coord.Row, coord.Col].Background = Brushes.Red;
            }
            return false; // Is chess there
        }

        public bool isValidMove()
        {
            //Debug.WriteLine(holdChess.isWhite);
            //Debug.WriteLine(isWhiteTurn);

            return true;
        }

        protected virtual void OnChessRemoved(ChessPiece? chess, Coord coord)
        {
            if (chess == null)
                return;
            ChessRemoved?.Invoke(this, new ChessMovedEventArgs(chess, coord));
        }

        public void TryCastle(King king, Coord endCoord)
        {
            if (endCoord == king.Coord + new Coord(0, 2)) // Short castling
            {
                if (king.IsWhite)
                    Move(new Coord(7, 7), new Coord(7, 5));
                else
                    Move(new Coord(0, 7), new Coord(0, 5));
            }
            else if (endCoord == king.Coord + new Coord(0, -2)) // Long castling
            {
                if (king.IsWhite)
                    Move(new Coord(7, 0), new Coord(7, 3));
                else
                    Move(new Coord(0, 0), new Coord(0, 3));
            }
        }

        public void RemoveEatenChessFromBoard(ChessPiece chessToEat)
        {
            _history.EatenChess.Push(chessToEat); // Store the chess
            ChessEaten?.Invoke(this, new ChessMovedEventArgs(chessToEat, chessToEat.Coord));
        }
    }

    class ChessMovedEventArgs : EventArgs
    {
        public ChessPiece Chess;
        public Coord Coord;

        public ChessMovedEventArgs(ChessPiece chess, Coord coord)
        {
            Chess = chess;
            Coord = coord;
        }
    }
}