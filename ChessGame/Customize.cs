/* This file defines the general Class, Struct in the project.
 * 
 * 
 */

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
    }

}