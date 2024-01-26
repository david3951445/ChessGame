using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using ChessGame.ChessPieces;

namespace ChessGame
{
    public partial class MainWindow : Window
    {
        private ChessBoard _board;
        private History _history;

        public MainWindow()
        {
            InitializeComponent();
            _board = new ChessBoard(gridBoard);
            InitializeBoard();
            _history = new History();
        }

        private void InitializeBoard()
        {
            // Add tip icon and chess pieces to board
            foreach (var item in _board.CurrentSituation)
                if (item != null)
                    AddEventToChess(item);
        }

        private void AddEventToChess(ChessPiece chess)
        {
            chess.Image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
            chess.Image.MouseMove += Image_MouseMove;
            chess.Image.MouseLeftButtonUp += Image_MouseLeftButtonUp;
        }

        /// <summary>
        /// Put chess back to the coord
        /// </summary>
        private void PutDown(Coord c)
        {
            _history.TempMiddleMove = string.Empty; // Reset temp of middle move expression

            // Reset tip icons
            foreach (var item in _board.TipIcon)
                if (item.Visibility == Visibility.Visible)
                    _board.ResetTipIcon(item);

            UI.Children.Remove(_board.HoldChess.Image); // Remove from air  
            _board.Add(c, _board.HoldChess); // Add to board
            SoundManager.ChessPutDown();
            _board.HoldChess = null;
        }

        private void RemoveEatenChessFromBoard(ChessPiece chess)
        {
            StackPanel eatenChess = whiteEatenChess;
            if (chess.IsWhite)
                eatenChess = blackEatenChess;

            gridBoard.Children.Remove(chess.Image); // Remove from board
            _history.EatenChess.Push(chess); // Store the chess
            eatenChess.Children.Add(chess.Image); // Show eaten chess on the stack panel
            chess.Image.Width = eatenChess.Height; // Match the size of stack panel
            chess.Image.Height = eatenChess.Height; // Match the size of stack panel
        }

        // Events

        /// <summary>
        /// Pick up a chess
        /// </summary>
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(UI); // Mouse position
            Coord currendCoord = _board.GetPosition(mousePosition); // Current coordinates
            ChessPiece? chess = _board.CurrentSituation[currendCoord.Row, currendCoord.Col]; // Chess in current coordinates

            if (chess != null) // Mouse hit the chess
            {
                //Debug.WriteLine("Pick up the chess");

                // Record the chess
                _history.TempMiddleMove += $"{chess.Name}{(char)('a' + currendCoord.Col)}{(char)('0' + 8 - currendCoord.Row)}";
                _board.PickUpCoord = currendCoord;

                // Find and show the valid move on board
                chess.AddTipToBoard(_board);

                // Remove chess image from board
                gridBoard.Children.Remove(chess.Image);
                _board.CurrentSituation[currendCoord.Row, currendCoord.Col] = null;

                // Add chess image to "air"
                UI.Children.Add(chess.Image);
                Grid.SetRow(chess.Image, 0);
                Grid.SetColumn(chess.Image, 0);
                chess.FollowMousePosition(mousePosition);
                _board.HoldChess = chess;
            }
            else
            {
                // Cancel some effect
                // ...
            }
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (_board.HoldChess == null) // Not holding a chess
                return;

            Point mousePosition = e.GetPosition(UI);
            _board.HoldChess.FollowMousePosition(mousePosition); // Let chess follow the mouse
            if (_board.IsOutOfBound(mousePosition))
                PutDown(_board.PickUpCoord); // Put back to the previous position
        }

        /// <summary>
        /// Put down a chess.
        /// </summary>
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var holdChess = _board.HoldChess;
            if (holdChess == null) // Not holding the chess
                return;

            Point mousePosition = e.GetPosition(UI);
            Coord coord = _board.GetPosition(mousePosition); // Current coordinates of mouse position.
            var currentChess = _board.CurrentSituation[coord.Row, coord.Col]; // Chess in current coordinates

            // Judgment when putting down chess
            TextBlock goalGrid = _board.TipIcon[coord.Row, coord.Col];
            if (goalGrid.Visibility != Visibility.Visible || holdChess.IsWhite != _board.IsWhiteTurn)
            {
                PutDown(_board.PickUpCoord); // Put back to the previous position
                return;
            }

            // Valid move
            string name;
            if (goalGrid.Background == Brushes.Black) // Non-eat move
            { 
                name = "E"; // Denote the Empty chess name
            }
            else if (goalGrid.Background == Brushes.Red) // Eat move
            { 
                name = currentChess.Name; // Eaten chess name
                RemoveEatenChessFromBoard(currentChess); // Remove it from board to eaten chess stackpanel
            }
            else
            {
                throw new Exception("Undefine tip color");
            }

            if (holdChess is Rook rook)
                if (rook.IsLeft)
                    _board.CanShortCastling = false;
                else
                    _board.CanLongCastling = false;
            if (holdChess is King)
                _board.CanShortCastling = _board.CanLongCastling = false;

            // Record the move
            _history.TempMiddleMove += $"{name}{(char)('a' + coord.Col)}{(char)('0' + 8 - coord.Row)}";
            historyTextBox.Text += _history.TempMiddleMove + "  "; // Show it on UI

            if (CanPromotion(coord))
            { 
                Debug.WriteLine("Promotion");
                // Promotion ...
            }

            PutDown(coord);

            _board.IsWhiteTurn = !_board.IsWhiteTurn; // Switch opponent 
        }

        private bool CanPromotion(Coord coord) => _board.HoldChess is Pawn && (coord.Row == 7 && !_board.HoldChess.IsWhite || coord.Row == 0 && _board.HoldChess.IsWhite);

        private void ImageBoard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void imageBoard_MouseMove(object sender, MouseEventArgs e)
        {
            Image_MouseMove(sender, e);
        }

        private void firstButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lastButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
