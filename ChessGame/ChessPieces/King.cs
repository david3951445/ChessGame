using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;

namespace ChessGame.ChessPieces
{
    public class King : OneGridChessPiece, ISpecialMovePiece
    {
        public bool HasMoved { get; set; }
        public Castling LongCastling { get; }
        public Castling ShortCastling { get; }

        public King(bool isWhite) : this(isWhite, default) { }

        public King(bool isWhite, Coord coord) : base(isWhite, "K")
        {
            Coord = coord;
            Directions = Dir.King();
            LongCastling = new Castling(isWhite, Coord.RangeByCol(Coord, -4));
            ShortCastling = new Castling(isWhite, Coord.RangeByCol(Coord, 3));
        }

        public override void AddTipToBoard(ChessBoard board)
        {
            base.AddTipToBoard(board);
            // Castling
            if (ShortCastling.IsCurrentValid(board))
                board.AddTip(ShortCastling.KingEndCoord, this);
            if (LongCastling.IsCurrentValid(board))
                board.AddTip(LongCastling.KingEndCoord, this);
        }

        public void DisableCastling(bool isShortSide)
        {
            if (isShortSide)
                ShortCastling.IsKingOrRookMoved = true;
            else
                LongCastling.IsKingOrRookMoved = true;
        }

        /// <summary>
        /// whether king is under attack
        /// </summary>
        public bool IsUnderAttacked(ChessBoard board) => IsUnderAttacked(board, IsWhite, Coord);

        public static bool IsUnderAttacked(ChessBoard board, bool isWhite, Coord currentCoord)
        {
            // Rook (rank and file)
            var dummyRook = new Rook(isWhite) { Coord = currentCoord };
            if (IsUnderAttack(board, dummyRook, chess => chess is Rook || chess is Queen))
                return true;
            // Bishop (diagonal)
            var dummyBishop = new Bishop(isWhite) { Coord = currentCoord };
            if (IsUnderAttack(board, dummyBishop, chess => chess is Bishop || chess is Queen))
                return true;
            // knight
            var dummyKnight = new Knight(isWhite) { Coord = currentCoord };
            if (IsUnderAttack(board, dummyKnight, chess => chess is Knight))
                return true;
            // Pawn
            var dummyPawn = new Pawn(isWhite) { Coord = currentCoord };
            if (IsUnderAttack(board, dummyPawn, chess => chess is Pawn))
                return true;

            return false;
        }

        private static bool IsUnderAttack(ChessBoard board, ChessPiece attackedChess, Predicate<ChessPiece> checkChessType)
        {
            var capturedChesses = attackedChess.CapturedChesses(board);
            if (capturedChesses.Any(capturedChess => checkChessType(capturedChess)))
                return true;
            return false;
        }
    }

    public class Castling
    {
        public bool IsKingOrRookMoved; // Permanent
        private bool _isWhite;
        private Coord[] _passingCoords;

        public Coord KingEndCoord { get; }        
        public Coord RookStartCoord { get; }
        public Coord RookEndCoord { get; }

        public Castling(bool isWhite, IEnumerable<Coord> coords)
        {
            _isWhite = isWhite;
            RookStartCoord = coords.Last();
            _passingCoords = coords.Skip(1).Take(coords.Count() - 2).ToArray();
            RookEndCoord = _passingCoords.First();
            KingEndCoord = _passingCoords.ElementAt(1);
        }

        public bool IsCurrentValid(ChessBoard board)
        {
            if (IsKingOrRookMoved)
                return false;
            if (_passingCoords.Any(coord => board.GetChessOn(coord) != null))
                return false;
            return _passingCoords.All(passingCoord => !King.IsUnderAttacked(board, _isWhite, passingCoord));
        }

        private bool HasAttacker(ChessBoard board, Coord coord)
        {
            if (board.IsOutOfBound(coord))
                return false;

            ChessPiece? targetChess = board.GetChessOn(coord);
            if (targetChess == null) // No Chess there
            {
                //TipIcon[coord.Row, coord.Col].Visibility = Visibility.Visible;
                return false;
            }

            if (_isWhite != targetChess.IsWhite) // Different color chess there
            //if (!chess.IsSameColor(targetChess)) // Different color chess there
            {
                //TipIcon[coord.Row, coord.Col].Visibility = Visibility.Visible;
                //TipIcon[coord.Row, coord.Col].Background = Brushes.Red;
                // Check targetChess type is correct type
                // ...
            }
            return true;
        }
    }

}
