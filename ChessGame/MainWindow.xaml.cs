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
            chess.image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
            chess.image.MouseMove += Image_MouseMove;
            chess.image.MouseLeftButtonUp += Image_MouseLeftButtonUp;
        }
        private void PutDown(Coords c) {
            history.tempMiddleMove = string.Empty; // Reset temp of middle move expression

            // Reset tip icons
            foreach (var item in board.tipIcon) { 
                if (item.Visibility == Visibility.Visible) {
                    board.resetTipIcon(item);
                }
            }

            UI.Children.Remove(board.holdChess.image); // Remove from air  
            board.Add(c, board.holdChess); // Add to board
            board.holdChess = null;
        }

        // Event handler

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Trace.WriteLine("Image_MouseLeftButtonDown");

            Point mousePosition = e.GetPosition(UI);
            Coords c = board.GetPosition(mousePosition); // Current coordinates
            ChessPiece ? chess = board.currentSituation[c.row, c.col];

            if (chess != null) { // Mouse hit the chess
                // Record the chess
                history.tempMiddleMove += $"{chess.name}{(char)('a' + c.col)}{(char)('0' + 8 - c.row )}";
                board.currentCoord = c;

                // Hold the picked chess
                board.holdChess = chess;
                board.currentSituation[c.row, c.col] = null;

                // Find and show the valid move on board
                chess.Rule(board);

                // Remove from board
                Image img = chess.image;
                gridBoard.Children.Remove(img);

                // Add to air
                double x = mousePosition.X - ChessPiece.Size / 2;
                double y = mousePosition.Y - ChessPiece.Size / 2;
                UI.Children.Add(img);
                img.Margin = new Thickness(x, y, 0, 0);
                Grid.SetRow(img, 0);
                Grid.SetColumn(img, 0);
            }
            else {
                // Cancel some effect
            }
        }
        private void Image_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
            //Coords c = board.GetPosition(mousePosition); // Current coordinates

            if (board.holdChess != null) { // Holding a chess
                Point mousePosition = e.GetPosition(UI);
                // Let chess follow the mouse
                double x = mousePosition.X - ChessPiece.Size / 2;
                double y = mousePosition.Y - ChessPiece.Size / 2;
                board.holdChess.image.Margin = new Thickness(x, y, 0, 0);

                // If mouse out off bound
                if (board.IsOutOfBound(mousePosition)) {
                    Trace.WriteLine("Out of bound");
                    PutDown(board.currentCoord); // Put back to the previous position
                }
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            Point mousePosition = e.GetPosition(UI);
            Coords c = board.GetPosition(mousePosition); // Current coordinates
            ChessPiece? chess = board.currentSituation[c.row, c.col];

            if (board.holdChess != null) { // Holding the chess
                // Cancel the tips
                TextBlock goalGrid = board.tipIcon[c.row, c.col];
                if (goalGrid.Visibility == Visibility.Visible) { // Valid move
                    string name;
                    if (goalGrid.Background == Brushes.Black) { // Non-eat move
                        name = "E"; // Denote the Empty chess
                    }
                    else if (goalGrid.Background == Brushes.Red) { // Eat move
                        name = chess.name; // Eaten chess
                    }
                    else {
                        throw new Exception("Undefine tip color");
                    }

                    // Record the move
                    history.tempMiddleMove += $"{name}{(char)('a' + c.col)}{(char)('0' + 8 - c.row)}";
                    historyTextBox.Text += history.tempMiddleMove + " "; // Show it on UI

                    PutDown(c); // Put back to board
                }
                else {
                    PutDown(board.currentCoord); // Put back to the previous position
                }
            }
        }

        private void ImageBoard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Trace.WriteLine("ImageBoard_MouseLeftButtonDown");

        }

        private void imageBoard_MouseMove(object sender, MouseEventArgs e) {
            Image_MouseMove(sender, e);
        }
    }
}
