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


namespace ChessGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ChessBoard board;
        public MainWindow() {
            InitializeComponent();
            InitializeBoard();
        }

        private void InitializeBoard() {
            // Add tip icon
            Image[,] tipIcon = new Image[8, 8];

            // Add chess pieces to board
            this.board = new ChessBoard(gridBoard);
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
            UI.Children.Remove(board.holdChess.image); // Remove from air  
            board.Add(c, board.holdChess); // Add to board
            board.holdChess = null;
        }

        // Event handler

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Trace.WriteLine("Image_MouseLeftButtonDown");

            Point mousePosition = e.GetPosition(UI);
            Coords c = board.GetPosition(mousePosition);
            ChessPiece? chess = board.currentSituation[c.row, c.col];

            if (chess != null) { // Mouse hit the chess

                // Hold the picked chess
                board.holdChess = chess;
                board.currentSituation[c.row, c.col] = null;

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
            Point mousePosition = e.GetPosition(UI);
            Coords c = board.GetPosition(mousePosition);

            if (board.holdChess != null) { // Holding a chess

                // Let chess follow the mouse
                double x = mousePosition.X - ChessPiece.Size / 2;
                double y = mousePosition.Y - ChessPiece.Size / 2;
                board.holdChess.image.Margin = new Thickness(x, y, 0, 0);

                // If mouse out off bound
                if (board.isOutOfBound(mousePosition)) {
                    Trace.WriteLine("Out of bound");
                    PutDown(board.history[board.history.Count - 1]); // Put back to the previous position
                }
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            Point mousePosition = e.GetPosition(UI);
            Coords c = board.GetPosition(mousePosition);
            
            if (board.holdChess != null) { // Holding the chess
                if (board.holdChess.Rule()) { // Conform game rules
                    board.history.Add(c); // Record the move
                    historyTextBox.Text += c.ToString(); // Show it on UI
                    PutDown(c); // Put back to board
                }
                else {
                    PutDown(board.history[board.history.Count - 1]); // Put back to the previous position
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
