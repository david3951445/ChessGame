/* This file defines the general Class, Struct in the project.
 * 
 * 
 */

using System;
using System.Windows.Controls;

namespace ChessGame
{
    /// <summary>
    /// Coordinates of board.
    /// 
    /// That is, the index of Grid object.
    /// </summary>
    public struct Coord
    {
        public int Row;
        public int Col;

        public Coord(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public override string ToString() => $"({Row}, {Col})";
        public static Coord operator +(Coord a, Coord b) => new Coord(a.Row + b.Row, a.Col + b.Col);
        public static bool operator ==(Coord a, Coord b) => a.Row == b.Row && a.Col == b.Col;
        public static bool operator !=(Coord a, Coord b) => a != b;

        /// <summary>
        /// Get new coordinates that rotate itself 90
        /// </summary>
        public Coord GetRotate90() => new Coord(-Col, Row);

        public override bool Equals(object? obj) => this == (Coord?)obj;

        public override int GetHashCode() => HashCode.Combine(Row, Col);
    }
}
