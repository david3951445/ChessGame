using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.ChessPieces
{
    public class King : OneGridChessPiece, ISpecialMovePiece
    {
        public bool HasMoved { get; set; }
        public Castling LongCastling { get; }
        public Castling ShortCastling { get; }

        public King(bool isWhite) : base(isWhite, "K")
        {
            Directions = Dir.King();
            LongCastling = new Castling(isWhite, Coord.RangeByRow(Coord, 4));
            ShortCastling = new Castling(isWhite, Coord.RangeByRow(Coord, -5));
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
    }

    public class Castling
    {
        public Coord[] PassingCoords;
        public Coord KingEndCoord;
        public bool IsKingOrRookMoved; // Permanent
        private bool _isWhite;

        public Castling(bool isWhite, IEnumerable<Coord> coords)
        {
            _isWhite = isWhite;
            PassingCoords = coords.Skip(1).Take(coords.Count() - 2).ToArray();
            KingEndCoord = PassingCoords[1];
        }

        public bool IsCurrentValid(ChessBoard board)
        {
            if (IsKingOrRookMoved)
                return false;
            if (PassingCoords.Any(coord => board.GetChessOn(coord) != null))
                return false;

            // whether PassingCoords is under attack
            foreach (var passingCoord in PassingCoords)
            {
                // Rook (rank and file)
                var dummyRook = new Rook(_isWhite) { Coord = passingCoord };
                if (IsUnderAttack(board, dummyRook, chess => chess is Rook || chess is Queen))
                    return false;
                // Bishop (diagonal)
                var dummyBishop = new Bishop(_isWhite) { Coord = passingCoord };
                if (IsUnderAttack(board, dummyBishop, chess => chess is Bishop || chess is Queen))
                    return false;
                // knight
                var dummyKnight = new Knight(_isWhite) { Coord = passingCoord };
                if (IsUnderAttack(board, dummyKnight, chess => chess is Knight))
                    return false;
                // Pawn
                var dummyPawn = new Pawn(_isWhite) { Coord = passingCoord };
                if (IsUnderAttack(board, dummyPawn, chess => chess is Pawn))
                    return false;
            }

            return true;
        }

        private static bool IsUnderAttack(ChessBoard board, ChessPiece attackedChess, Predicate<ChessPiece> checkChessType)
        {
            var capturedChesses = attackedChess.CapturedChesses(board);
            if (capturedChesses.Any(capturedChess => checkChessType(capturedChess)))
                return true;
            return false;
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
