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
        private ChessBoard _board;
        private PromotionUI _promotionUI;

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
            for (int i = 0; i < ChessBoard.SIZE; i++)
                for (int j = 0; j < ChessBoard.SIZE; j++)
                {
                    var textBlock = _board.TipIcon[i, j];
                    gridBoard.Children.Add(textBlock);
                    Grid.SetRow(textBlock, i);
                    Grid.SetColumn(textBlock, j);
                }

            // Add chess pieces to board
            _board.ChessAdded += Board_ChessAdded;
            _board.ChessRemoved += Board_ChessRemoved;
            _board.ChessCaptured += Board_ChessEaten;
            _board.ChessCreated += Board_ChessCreated;
            StartNewGame();
        }

        private void Board_ChessAdded(object? _, ChessMovedEventArgs e)
        {
            var image = e.Chess.Image;
            image.Margin = new Thickness(0, 0, 0, 0);
            gridBoard.Children.Add(image);
            Grid.SetRow(image, e.Coord.Row);
            Grid.SetColumn(image, e.Coord.Col);
        }

        private void Board_ChessRemoved(object? _, ChessMovedEventArgs e)
        {
            gridBoard.Children.Remove(e.Chess.Image);
        }

        private void Board_ChessEaten(object? sender, ChessMovedEventArgs e)
        {
            // Remove it from board to eaten chess stackpanel
            var chess = e.Chess;
            StackPanel stackPanel = whiteEatenChesses;
            if (chess.IsWhite)
                stackPanel = blackEatenChesses;
            gridBoard.Children.Remove(chess.Image); // Remove from board
            stackPanel.Children.Add(chess.Image); // Show eaten chess on the stack panel
            chess.Image.Width = stackPanel.Height; // Match the size of stack panel
            chess.Image.Height = stackPanel.Height; // Match the size of stack panel
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
            ChessPiece? chess = _board.GetChessOn(currendCoord); // Chess in current coordinates

            if (chess != null) // Mouse hit the chess
            {
                // Record the chess
                _board._history.TempMiddleMove += $"{chess.Name}{(char)('a' + currendCoord.Col)}{(char)('0' + 8 - currendCoord.Row)}";
                _board.PickUpCoord = currendCoord;

                // Find and show the valid move on board
                chess.AddTipToBoard(_board);

                // Remove chess image from board
                _board.Remove(currendCoord);
                var image = chess.Image;

                // Add chess image to "air"
                UI.Children.Add(image);
                Grid.SetRow(image, 0);
                Grid.SetColumn(image, 0);
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
            var holdChess = _board.HoldChess;
            if (holdChess == null) // Not holding a chess
                return;

            Point mousePosition = e.GetPosition(UI);
            holdChess.FollowMousePosition(mousePosition); // Let chess follow the mouse
            if (IsOutOfBound(mousePosition))
            {
                PutDown(holdChess, _board.PickUpCoord); // Put back to the previous position
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
            Coord endCoord = _board.GetPosition(mousePosition);
            var currentChess = _board.GetChessOn(endCoord); // Chess in current coordinates

            // Judgment when putting down chess
            TextBlock goalGrid = _board.TipIcon[endCoord.Row, endCoord.Col];
            if (goalGrid.Visibility != Visibility.Visible || holdChess.IsWhite != _board.IsWhiteTurn)
            {
                PutDown(holdChess, _board.PickUpCoord); // Put back to the previous position
                return;
            }
            //if (King is depend)
            //{
            //    ...
            //    return;
            //}

            // Valid move
            string name;
            if (currentChess == null) // Non-capture move
            {
                name = "E"; // Temporary. Denote the Empty chess name
            }
            else if (goalGrid.Background == Brushes.Red) // Capture move
            {
                name = currentChess.Name; // Eaten chess name
                _board.RemoveCapturedChessFromBoard(currentChess);
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
            if (_board.CanPromotion(holdChess))
            {
                _promotionUI = new PromotionUI(holdChess, mousePosition);
                //_promotionUI.Margin = new Thickness(mousePosition.X, mousePosition.Y, 0, 0);
                _promotionUI.ChessPieceSelected += PromotionUI_ChessPieceSelected;
                UI.Children.Add(_promotionUI);
                Grid.SetRow(_promotionUI, 0);
                Grid.SetColumn(_promotionUI, 0);
                //Popup chessPiecePopup = new Popup();
                //chessPiecePopup.IsOpen = true;
                //chessPiecePopup.HorizontalOffset = mousePosition.X;
                //chessPiecePopup.VerticalOffset = mousePosition.Y;
            }
            
            _board.UpdateCastlingState(holdChess); // Update castling state
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
        /// Put chess back to the coord
        /// </summary>
        private void PutDown(ChessPiece holdChess, Coord c)
        {
            UI.Children.Remove(holdChess.Image); // Remove from air  
            _board.PutDown(holdChess, c);
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
