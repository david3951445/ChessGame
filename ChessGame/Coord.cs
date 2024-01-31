/* This file defines the general Class, Struct in the project.
 * 
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ChessGame
{
    /// <summary>
    /// Coordinates of board.
    /// 
    /// That is, the index of Grid object.
    /// </summary>
    public struct Coord
    {
        public int Row; // rank
        public int Col; // file

        public Coord(double row, double col) : this((int)row, (int)col) { }

        public Coord(int row, int col)
        {
            Row = row;
            Col = col;
        }


        /// <summary>
        /// Get new coordinates that rotate itself 90
        /// </summary>
        public Coord GetRotate90() => new Coord(-Col, Row);

        public Coord GetRotate180() => new Coord(-Row, -Col);

        public static IEnumerable<Coord> RangeByRow(Coord origin, int offset) => MyRange(origin.Row, offset).Select(row => new Coord(row, origin.Col));

        public static IEnumerable<Coord> RangeByCol(Coord origin, int offset) => MyRange(origin.Col, offset).Select(col => new Coord(origin.Row, col));

        /// <summary>
        /// Extend Enumerable.Range, use "offset" rather "count".
        /// </summary>
        private static IEnumerable<int> MyRange(int start, int offset)
        {
            var count = Math.Abs(offset) + 1;
            if (offset >= 0)
                return Enumerable.Range(start, count);
            return Enumerable.Range(start + offset, count).Reverse();
        }

        public override string ToString() => $"({Row}, {Col})";

        public static Coord operator +(Coord a, Coord b) => new Coord(a.Row + b.Row, a.Col + b.Col);

        public static Coord operator *(int scale, Coord a) => new Coord(a.Row * scale, a.Col * scale);

        public static bool operator ==(Coord a, Coord b) => a.Row == b.Row && a.Col == b.Col;

        public static bool operator !=(Coord a, Coord b) => a != b;

        public override bool Equals(object? obj) => this == (Coord?)obj;

        public override int GetHashCode() => HashCode.Combine(Row, Col);
    }
}
