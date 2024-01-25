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
        public int row;
        public int col;

        public Coord(int _X, int _Y)
        {
            row = _X;
            col = _Y;
        }

        public override string ToString() => $"({row}, {col})";
        public static Coord operator +(Coord a, Coord b) => new Coord(a.row + b.row, a.col + b.col);
        /// <summary>
        /// Get new coordinates that rotate itself 90
        /// </summary>
        public Coord GetRotate90() => new Coord(-col, row);
    }
}
