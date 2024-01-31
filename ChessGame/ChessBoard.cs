using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
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
    public class ChessBoard
    {
        public event EventHandler<ChessPiece>? ChessAdded;
        public event EventHandler<ChessPiece>? ChessRemoved;
        public event EventHandler<ChessPiece>? ChessCaptured;
        public event EventHandler<ChessPiece>? ChessCreated;
        public Action? GetPromotedChess;

        //public bool isHoldingChess = false;
        public TextBlock[,] TipIcon = new TextBlock[SIZE, SIZE]; // The tips when moving the chess. Also use to check valid moves.
        public Coord PickUpCoord; // Current Coordinates when mouse pick up a chess
        public ChessPiece? HoldChess; // Current holding chess
        public const int SIZE = 8; // Number of grids on a side
        //public Stack<Coords> eatCoords = new Stack<Coords>(); // The coordinates where you can eat chess
        //public Stack<Coords> moveCoords = new Stack<Coords>(); // The coordinates where you can move chess
        public History _history = new History();
        private King _whiteKing;
        private King _blackKing;

        public bool IsWhiteTurn { get; set; }
        public ChessPiece?[,] CurrentState { get; private set; } // Current game situation of the board
        public Pawn? InPassingPawn { get; private set; } // Can use bool

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
            Add(ChessPieceType.Rook, isWhite, new Coord(7, 0));
            Add(ChessPieceType.Knight, isWhite, new Coord(7, 1));
            Add(ChessPieceType.Bishop, isWhite, new Coord(7, 2));
            Add(ChessPieceType.Queen, isWhite, new Coord(7, 3));
            _whiteKing = (King)CreatePiece(ChessPieceType.King, isWhite, new Coord(7, 4));
            Add(_whiteKing);
            Add(ChessPieceType.Bishop, isWhite, new Coord(7, 5));
            Add(ChessPieceType.Knight, isWhite, new Coord(7, 6));
            Add(ChessPieceType.Rook, isWhite, new Coord(7, 7));
            for (int j = 0; j < SIZE; j++)
                Add(ChessPieceType.Pawn, isWhite, new Coord(6, j));

            isWhite = false;
            Add(ChessPieceType.Rook, isWhite, new Coord(0, 0));
            Add(ChessPieceType.Knight, isWhite, new Coord(0, 1));
            Add(ChessPieceType.Bishop, isWhite, new Coord(0, 2));
            Add(ChessPieceType.Queen, isWhite, new Coord(0, 3));
            _blackKing = (King)CreatePiece(ChessPieceType.King, isWhite, new Coord(0, 4));
            Add(_blackKing);
            Add(ChessPieceType.Bishop, isWhite, new Coord(0, 5));
            Add(ChessPieceType.Knight, isWhite, new Coord(0, 6));
            Add(ChessPieceType.Rook, isWhite, new Coord(0, 7));
            for (int j = 0; j < SIZE; j++)
                Add(ChessPieceType.Pawn, isWhite, new Coord(1, j));
        }

        public ChessPiece CreatePiece(ChessPieceType type, bool isWhite, Coord coord, bool isShortSide = false)
        {
            ChessPiece chess = type switch
            {
                ChessPieceType.Rook => new Rook(isWhite) { IsShortSide = isShortSide },
                ChessPieceType.Knight => new Knight(isWhite),
                ChessPieceType.Bishop => new Bishop(isWhite),
                ChessPieceType.Queen => new Queen(isWhite),
                ChessPieceType.King => new King(isWhite, coord),
                ChessPieceType.Pawn => new Pawn(isWhite),
                _ => throw new NotImplementedException()
            };
            chess.Coord = coord;
            ChessCreated?.Invoke(this, chess);
            return chess;
        }

        // Add the chess to board corresponding to the coordinates
        private void Add(ChessPieceType type, bool isWhite, Coord coord) => Add(CreatePiece(type, isWhite, coord));

        public void Add(ChessPiece chessToAdd) => Add(chessToAdd, chessToAdd.Coord);

        public void Add(ChessPiece chessToAdd, Coord coord)
        {
            chessToAdd.Coord = coord;
            ChessAdded?.Invoke(this, chessToAdd);
            CurrentState[coord.Row, coord.Col] = chessToAdd;
        }

        public ChessPiece? Remove(Coord coord)
        {
            var chessToRemove = CurrentState[coord.Row, coord.Col];
            OnChessRemoved(chessToRemove, coord);
            CurrentState[coord.Row, coord.Col] = null;
            return chessToRemove;
        }

        public ChessPiece? GetChessOn(Coord coord) => IsOutOfBound(coord) ? null : CurrentState[coord.Row, coord.Col];

        /// <summary>
        /// Get seen chess on the direction of <paramref name="bias"/> from <paramref name="startCoord"/>.
        /// </summary>
        public ChessPiece? GetSeenChess(Coord startCoord, Coord bias)
        {
            ChessPiece? chessPiece = null;
            startCoord += bias;
            while (!IsOutOfBound(startCoord))
            {
                chessPiece = GetChessOn(startCoord);
                if (chessPiece != null)
                    break;
                startCoord += bias;
            }
            return chessPiece;
        }

        public void Move(Coord startCoord, Coord endCoord)
        {
            var chess = Remove(startCoord);
            if (chess == null)
                return;
            Add(chess, endCoord);
        }

        public bool IsOutOfBound(Coord boardCoord) => boardCoord.Col < 0 || boardCoord.Col >= SIZE || boardCoord.Row < 0 || boardCoord.Row >= SIZE;

        public void RemoveCapturedChessFromBoard(ChessPiece capturedChess)
        {
            _history.CapturedChess.Push(capturedChess); // Store the chess
            ChessCaptured?.Invoke(this, capturedChess);
        }

        /// <summary>
        /// Set the position of the mouse cursor on the chessboard coordinates
        /// </summary>
        /// <param name="mousePosition"> Relative to the top left corner of board </param>
        public void SetPosition(Point mousePosition)
        {
            PickUpCoord = new Coord(mousePosition.Y / ChessPiece.Size, mousePosition.X / ChessPiece.Size);
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

            ChessPiece? targetChess = GetChessOn(coord);
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

        public bool IsValidMove()
        {
            //Debug.WriteLine(holdChess.isWhite);
            //Debug.WriteLine(isWhiteTurn);

            return true;
        }

        public void PutDown(ChessPiece holdChess, Coord endCoord)
        {
            TryCastle(holdChess, endCoord);
            Add(holdChess, endCoord); // Add to board
            HoldChess = null;

            _history.TempMiddleMove = string.Empty; // Reset temp of middle move expression
            // Reset tip icons
            foreach (var item in TipIcon)
                if (item.Visibility == Visibility.Visible)
                    ResetTipIcon(item);
        }

        public void UpdateCastlingState(ChessPiece holdChess)
        {
            if (holdChess is Rook rook)
            {
                if (holdChess.IsWhite)
                    _whiteKing.DisableCastling(rook.IsShortSide);
                else
                    _blackKing.DisableCastling(rook.IsShortSide);
            }
            else if (holdChess is King)
            {
                if (holdChess.IsWhite)
                {
                    _whiteKing.DisableCastling(true);
                    _whiteKing.DisableCastling(false);
                }
                else
                {
                    _blackKing.DisableCastling(true);
                    _blackKing.DisableCastling(false);
                }
            }
        }

        public bool TryCastle(ChessPiece holdChess, Coord endCoord)
        {
            if (holdChess is not King king)
                return false;

            if (endCoord == king.ShortCastling.KingEndCoord) // Short castling
                Move(king.ShortCastling.RookStartCoord, king.ShortCastling.RookEndCoord);
            else if (endCoord == king.LongCastling.KingEndCoord) // Long castling
                Move(king.LongCastling.RookStartCoord, king.LongCastling.RookEndCoord);
            
            return true;
        }


        public void UpdateInPassingState(ChessPiece holdChess, Coord startCoord)
        {
            if (holdChess is Pawn pawn && startCoord + pawn.Foward2 == pawn.Coord)
                InPassingPawn = pawn;
            else
                InPassingPawn = null;
        }

        public bool IsKingDepended(ChessPiece holdChess)
        {
            var king = holdChess.IsWhite ? _whiteKing : _blackKing;
            return king.IsUnderAttacked(this);
        }

        //public ChessPiece TryPromote(ChessPiece chessToPromote)
        //{
        //    if (!CanPromotion(chessToPromote))
        //        return chessToPromote;
        //    return CreatePiece(ChessPieceType.Queen, chessToPromote.IsWhite);
        //}

        public bool CanPromotion(ChessPiece chess) => chess is Pawn && (chess.Coord.Row == 7 && !chess.IsWhite || chess.Coord.Row == 0 && chess.IsWhite);

        protected virtual void OnChessRemoved(ChessPiece? chess, Coord coord)
        {
            if (chess == null)
                return;
            ChessRemoved?.Invoke(this, chess);
        }
    }

    public class ChessMovedEventArgs : EventArgs
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