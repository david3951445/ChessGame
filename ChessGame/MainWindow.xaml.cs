using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ChessGame.ChessPieces;
using Point = System.Windows.Point;
using System.Diagnostics;

namespace ChessGame
{
    public partial class MainWindow : Window
    {
        private readonly ChessBoard _board;
        private PromotionUI? _promotionUI;

        public MainWindow()
        {
            InitializeComponent();
            ChessPiece.Size = gridBoard.Width / ChessBoard.SIZE;
            _board = new ChessBoard();
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            // Add tip icons
            for (int row = 0; row < ChessBoard.SIZE; row++)
                for (int col = 0; col < ChessBoard.SIZE; col++)
                    gridBoard.AddChild(_board.TipIcon[row, col], row, col);

            // Add chess pieces to board
            _board.ChessAdded += Board_ChessAdded;
            _board.ChessRemoved += Board_ChessRemoved;
            _board.ChessCaptured += Board_ChessCaptured;
            _board.ChessCreated += Board_ChessCreated;
            StartNewGame();
        }

        private void Board_ChessAdded(object? _, ChessPiece chess)
        {
            var image = chess.Image;
            image.Margin = new Thickness(0, 0, 0, 0);
            gridBoard.AddChild(image, chess.Coord.Row, chess.Coord.Col);
        }

        private void Board_ChessRemoved(object? _, ChessPiece chess)
        {
            gridBoard.Children.Remove(chess.Image);
        }

        private void Board_ChessCaptured(object? _, ChessPiece chess)
        {
            // Remove it from board to eaten chess stackpanel
            StackPanel stackPanel = chess.IsWhite ? blackEatenChesses : whiteEatenChesses;
            var image = chess.Image;
            gridBoard.Children.Remove(image); // Remove from board
            stackPanel.Children.Add(image); // Show eaten chess on the stack panel
            image.Width = stackPanel.Height; // Match the size of stack panel
            image.Height = stackPanel.Height; // Match the size of stack panel
        }

        private void Board_ChessCreated(object? _, ChessPiece chess) => AddImageEvent(chess.Image);

        private void AddImageEvent(Image image)
        {
            image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
            image.MouseMove += Image_MouseMove;
            image.MouseLeftButtonUp += Image_MouseLeftButtonUp;
        }

        /// <summary>
        /// Pick up a chess
        /// </summary>
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(UI); // Mouse position
            Coord currendCoord = _board.GetPosition(mousePosition); // Current coordinates
            ChessPiece? pickUpChess = _board.GetChessOn(currendCoord); // Chess in current coordinates

            if (pickUpChess != null) // Mouse hit the chess
            {
                // Find and show the valid move on board
                pickUpChess.AddTipToBoard(_board);

                // Remove chess from board
                _board.Remove(pickUpChess);

                // Add chess image to "air"
                var image = pickUpChess.Image;
                UI.AddChild(image, 0, 0);
                pickUpChess.FollowMousePosition(mousePosition);
                _board.HoldChess = pickUpChess;

                // Record the chess
                _board._history.TempMiddleMove += $"{pickUpChess.Name}{(char)('a' + currendCoord.Col)}{(char)('0' + 8 - currendCoord.Row)}";
            }
            else
            {
                // Cancel some effect
                // ...
            }
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            var holdChess = _board.HoldChess;
            if (holdChess == null) // Not holding a chess
                return;

            Point mousePosition = e.GetPosition(UI);
            holdChess.FollowMousePosition(mousePosition); // Let chess follow the mouse
            if (IsOutOfBound(mousePosition))
            {
                PutDown(holdChess, holdChess.Coord); // Put back to the previous position
            }
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
            Coord startCoord = holdChess.Coord;
            Coord endCoord = _board.GetPosition(mousePosition);
            var endChess = _board.GetChessOn(endCoord);

            // Judgment when putting down chess
            TextBlock goalGrid = _board.TipIcon[endCoord.Row, endCoord.Col];
            if (goalGrid.Visibility != Visibility.Visible || holdChess.IsWhite != _board.IsWhiteTurn || _board.IsKingDepended(holdChess))
            {
                PutDown(holdChess, holdChess.Coord); // Put back to the previous position
                return;
            }

            // Valid move
            string name;
            if (endChess == null) // Non-capture move
            {
                name = "E"; // Temporary. Denote the Empty chess name
            }
            else if (goalGrid.Background == Brushes.Red) // Capture move
            {
                name = endChess.Name; // Eaten chess name
                _board.RemoveCapturedChessFromBoard(endChess);
            }
            else
            {
                throw new Exception("Undefine tip color");
            }

            // Record the move
            _board._history.TempMiddleMove += $"{name}{(char)('a' + endCoord.Col)}{(char)('0' + 8 - endCoord.Row)}";
            historyTextBox.Text += _board._history.TempMiddleMove + "  "; // Show it on UI

            // PutDown
            PutDown(holdChess, endCoord);

            // Upadte status in board
            if (_board.CanPromotion(holdChess))
            {
                _promotionUI = new PromotionUI(holdChess, mousePosition);
                _promotionUI.ChessPieceSelected += PromotionUI_ChessPieceSelected;
                UI.AddChild(_promotionUI, 0, 0);
            }       
            _board.UpdateCastlingState(holdChess); // Update castling state
            _board.UpdateInPassingState(holdChess, startCoord);
            _board.IsWhiteTurn = !_board.IsWhiteTurn; // Switch opponent 
        }

        private void PromotionUI_ChessPieceSelected(object? sender, ChessPiece chess)
        {
            AddImageEvent(chess.Image);
            _board.Remove(chess.Coord); // Remove pawn
            _board.Add(chess); // Add promoted chess piece
            UI.Children.Remove(_promotionUI); // Close promotion UI
        }

        /// <summary>
        /// Put chess to the coord
        /// </summary>
        private void PutDown(ChessPiece holdChess, Coord endCoord)
        {
            UI.Children.Remove(holdChess.Image); // Remove from air  
            _board.PutDown(holdChess, endCoord);
            MediaManager.ChessPutDown();
        }

        private bool IsOutOfBound(Point mousePos) => mousePos.X < 0 || mousePos.X >= gridBoard.Width || mousePos.Y < 0 || mousePos.Y >= gridBoard.Height;


        private void ImageBoard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ImageBoard_MouseMove(object sender, MouseEventArgs e)
        {
            Image_MouseMove(sender, e);
        }

        private void FirstButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LastButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void startButton_Click(object sender, RoutedEventArgs e) => StartNewGame();

        private void StartNewGame()
        {
            var tipIcons = gridBoard.Children.OfType<Image>().ToList();
            foreach (var tipIcon in tipIcons)
                gridBoard.Children.Remove(tipIcon);
            whiteEatenChesses.Children.Clear();
            blackEatenChesses.Children.Clear();
            historyTextBox.Text = string.Empty;

            _board.StartNewGame();
        }
    }
}
