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
    public struct Coords
    {
        public int row;
        public int col;

        public Coords(int _X, int _Y) {
            row = _X;
            col = _Y;
        }

        public override string ToString() => $"({row}, {col})";
        public static Coords operator +(Coords a, Coords b) => new Coords(a.row + b.row, a.col + b.col);
        public Coords GetRotate90() { // Get new coordinates that rotate itself 90
            return new Coords(-col, row);
        }
    }


}