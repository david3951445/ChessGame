using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace ChessGame.ChessPieces
{
    public abstract class OneGridChessPiece : ChessPiece // Can move one number of grids. ex. King, Knight
    {
        protected OneGridChessPiece(bool isWhite, string name) : base(isWhite, name) { }

        public override void AddTipToBoard(ChessBoard board)
        {
            foreach (var bias in Directions)
                board.AddTip(Coord + bias, this);
        }
    }

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

    public class Knight : OneGridChessPiece
    {
        public Knight(bool _isWhite) : base(_isWhite, "N")
        {
            Directions = Dir.Knight();
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
            //...
            foreach (var passingCoord in PassingCoords)
            {
                // file
                Coord bias = passingCoord.Row == 0 ? new Coord(1, 0) : new Coord(-1, 0);
                Coord coord = passingCoord + bias;
                while (!board.IsOutOfBound(coord))
                {
                    var chess = board.GetChessOn(coord);
                    if (chess == null)
                    {
                        coord += bias;
                        continue;
                    }
                    if (_isWhite != chess.IsWhite && (chess is Rook || chess is Queen))
                        return false;
                    break;
                }
                // left diagonal
                bias = passingCoord.Row == 0 ? new Coord(1, -1) : new Coord(-1, -1);
                coord = passingCoord + bias;
                while (!board.IsOutOfBound(coord))
                {
                    var chess = board.GetChessOn(coord);
                    if (chess == null)
                    {
                        coord += bias;
                        continue;
                    }
                    if (_isWhite != chess.IsWhite && (chess is Bishop || chess is Queen))
                        return false;
                    break;
                }
                // right diagonal
                bias = passingCoord.Row == 0 ? new Coord(1, 1) : new Coord(-1, 1);
                coord = passingCoord + bias;
                while (!board.IsOutOfBound(coord))
                {
                    var chess = board.GetChessOn(coord);
                    if (chess == null)
                    {
                        coord += bias;
                        continue;
                    }
                    if (_isWhite != chess.IsWhite && (chess is Bishop || chess is Queen))
                        return false;
                    break;
                }
                // knight
                Coord[] knightBiases = passingCoord.Row == 0 ? ChessPiece.Dir.LowerKnight : ChessPiece.Dir.UpperKnight;
                foreach (var knightBias in knightBiases)
                {
                    coord = passingCoord + knightBias;
                    var chess = board.GetChessOn(coord);
                    if (chess == null)
                        continue;
                    if (_isWhite != chess.IsWhite && (chess is Knight))
                        return false;
                }
            }
            return true;
        }
        private bool IsTopSide(int rank) => rank == 0;
        private bool HasAttacter(ChessBoard board, Coord coord)
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
