using ChessGame.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame
{
    /// <summary>
    /// History of chess moving in chess game.
    /// Contains the function of rewinding and advancing the game.
    /// </summary>
    internal class History
    {
        public History() {

        }

        private int currentIndex; // Current index of this.list
        public string tempMiddleMove = String.Empty; // The temporary record in one move. One move: MouseLeftButtonDown() -> MouseMove() -> MouseLeftButtonUp()
        private Stack<string> strEatenChess = new Stack<string>(); // Store the eaten chesses (string form)
        public Stack<ChessPiece> eatenChess = new Stack<ChessPiece>(); // Store the eaten chesses
        public List<string> list = new List<string>(); // History, use the standard record method. (ex. R3xd7, O-O)
        public List<Coords> history = new List<Coords>(); // Record the coord of board, for testing

        public void FirstMove() {

        }
        public void PreviousMove() {

        }
        public void NextMove() {

        }
        public void LastMove() {

        }
        /// <summary>
        /// Convert the history move to an exactly move.
        /// 
        /// Algorithm:
        /// Let A = a move in game   
        /// Let B = middle expression
        ///     ex.
        ///     - Nb1Nc3 : b1 Knight eat c3 Knight 
        ///     -
        /// Let C = standard expression
        ///     ex.
        ///     - Nxc3: a Knight eat c3 chess
        /// In the game, we record the history by the order A -> B -> C. This method Move() is the inverse
        /// process C -> B -> A.
        /// C -> B:
        ///     - Use the record of eaten chess, stackEatenChess
        ///         ex.
        ///         - Nxc3 -> NNc3
        ///     - Finding the start position of the chess in a move, FindBeginPosition()
        ///         ex.
        ///         - NNc3 -> Nb1Nc3
        /// B -> A:
        ///     oneMove()
        /// 
        /// </summary>
        /// <param name="board"></param>
        private void Move(ChessBoard board) { // 
            //string oneMove = list[currentIndex];
            string oneMove = "R4xd4";
        }
        private void FindBeginPosition() {

        }

    }
}
