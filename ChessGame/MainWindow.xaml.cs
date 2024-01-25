using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Xml.Linq;
using System.ComponentModel;
using ChessGame.ChessPieces;
using System.Media;

namespace ChessGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
            this.board = new ChessBoard(gridBoard);
            InitializeBoard();
            this.history = new History();
        }

        ChessBoard board;
        History history;

        private void InitializeBoard() {
            // Add tip icon and chess pieces to board
            foreach (var item in board.currentSituation) {
                if (item != null) {
                    AddEventToChess(item);
                }
            }
        }
        private void AddEventToChess(ChessPiece chess) {
            chess.Image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
            chess.Image.MouseMove += Image_MouseMove;
            chess.Image.MouseLeftButtonUp += Image_MouseLeftButtonUp;
        }
        private void PutDown(Coord c) { // Put chess back to the coord
            history.tempMiddleMove = string.Empty; // Reset temp of middle move expression

            // Reset tip icons
            foreach (var item in board.tipIcon) {
                if (item.Visibility == Visibility.Visible) {
                    board.resetTipIcon(item);
                }
            }

            UI.Children.Remove(board.holdChess.Image); // Remove from air  
            board.Add(c, board.holdChess); // Add to board
            SoundManager.ChessPutDown();
            board.holdChess = null;
        }
        private void RemoveEatenChessFromBoard(ChessPiece chess) {
            StackPanel eatenChess = whiteEatenChess;
            if (chess.IsWhite) {
                eatenChess = blackEatenChess;
            }

            gridBoard.Children.Remove(chess.Image); // Remove from board
            history.eatenChess.Push(chess); // Store the chess
            eatenChess.Children.Add(chess.Image); // Show eaten chess on the stack panel
            chess.Image.Width = eatenChess.Height; // Match the size of stack panel
            chess.Image.Height = eatenChess.Height; // Match the size of stack panel
        }

        // Event handler

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Point mousePosition = e.GetPosition(UI); // Mouse position
            Coord c = board.GetPosition(mousePosition); // Current coordinates
            ChessPiece? chess = board.currentSituation[c.row, c.col]; // Chess in current coordinates

            if (chess != null) { // Mouse hit the chess
                //Debug.WriteLine("Pick up the chess");

                // Record the chess
                history.tempMiddleMove += $"{chess.Name}{(char)('a' + c.col)}{(char)('0' + 8 - c.row)}";
                board.pickUpCoord = c;

                // Find and show the valid move on board
                chess.Rule(board);

                // Remove chess image from board
                gridBoard.Children.Remove(chess.Image);
                board.currentSituation[c.row, c.col] = null;

                // Add chess image to "air"
                UI.Children.Add(chess.Image);
                Grid.SetRow(chess.Image, 0);
                Grid.SetColumn(chess.Image, 0);
                chess.FollowMousePosition(mousePosition);
                board.holdChess = chess;
            }
            else {
                // Cancel some effect
                // ...
            }
        }
        private void Image_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
            if (board.holdChess != null) { // Holding a chess
                Point mousePosition = e.GetPosition(UI);

                board.holdChess.FollowMousePosition(mousePosition); // Let chess follow the mouse

                if (board.IsOutOfBound(mousePosition)) { // Mouse out off bound
                    PutDown(board.pickUpCoord); // Put back to the previous position
                }
            }
        }
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            Point mousePosition = e.GetPosition(UI);
            Coord c = board.GetPosition(mousePosition); // Current coordinates
            ChessPiece? chess = board.currentSituation[c.row, c.col]; // Chess in current coordinates

            if (board.holdChess != null) { // Holding the chess
                // Judgment when putting down chess
                TextBlock goalGrid = board.tipIcon[c.row, c.col];
                if ((goalGrid.Visibility == Visibility.Visible) && !(board.holdChess.IsWhite ^ board.isWhiteTurn)) { // Valid move
                    string name;
                    if (goalGrid.Background == Brushes.Black) { // Non-eat move
                        name = "E"; // Denote the Empty chess name
                    }
                    else if (goalGrid.Background == Brushes.Red) { // Eat move
                        name = chess.Name; // Eaten chess name

                        RemoveEatenChessFromBoard(chess); // Remove it from board to eaten chess stackpanel
                    }
                    else {
                        throw new Exception("Undefine tip color");
                    }

                    // Record the move
                    history.tempMiddleMove += $"{name}{(char)('a' + c.col)}{(char)('0' + 8 - c.row)}";
                    historyTextBox.Text += history.tempMiddleMove + "  "; // Show it on UI

                    if (board.holdChess is Pawn && c.row == 7 && !board.holdChess.IsWhite || c.row == 0 && board.holdChess.IsWhite) { // Promotion
                        Debug.WriteLine("Promotion");
                    }
                    // Put down to board
                    PutDown(c); 

                    board.isWhiteTurn = !board.isWhiteTurn; // Switch opponent 
                }
                else {
                    PutDown(board.pickUpCoord); // Put back to the previous position
                }
            }
        }

        private void ImageBoard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {

        }

        private void imageBoard_MouseMove(object sender, MouseEventArgs e) {
            Image_MouseMove(sender, e);
        }

        private void firstButton_Click(object sender, RoutedEventArgs e) {

        }

        private void previousButton_Click(object sender, RoutedEventArgs e) {

        }

        private void nextButton_Click(object sender, RoutedEventArgs e) {

        }

        private void lastButton_Click(object sender, RoutedEventArgs e) {

        }
    }
}
