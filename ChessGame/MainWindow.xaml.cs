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

        private Point preMousePosition;

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
        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (board.holdChess == null) {
                Trace.WriteLine("Image_MouseLeftButtonDown");

                // Hold the picked chess and remove it from current situation of board. 
                Image img = sender as Image;
                int row = Grid.GetRow(img);
                int col = Grid.GetColumn(img);
                board.holdChess = board.currentSituation[row, col];
                board.currentSituation[row, col] = null;

                // Pick the chess up.
                preMousePosition = e.GetPosition(UI);
                board.PickUp(UI, preMousePosition);
            }
        }
        private void Image_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
            if (board.holdChess != null) {
                Trace.WriteLine("Image_MouseMove");

                // Let chess follow the mouse
                Point mousePosition = e.GetPosition(UI);
                double x = mousePosition.X - ChessPiece.Size / 2;
                double y = mousePosition.Y - ChessPiece.Size / 2;
                board.holdChess.image.Margin = new Thickness(x, y, 0, 0);

                // If mouse out off bound
                if (board.isOutOfBound(mousePosition)) {
                    Trace.WriteLine("Out of bound");

                    //Trace.WriteLine(preMousePosition.ToString()); 
                    board.PutDown(UI, preMousePosition); // Put back to the previous position
                }
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            Point mousePosition = e.GetPosition(UI);
            if (!board.isOutOfBound(mousePosition)) {
                Trace.WriteLine("Image_MouseLeftButtonUp");

                if (board.holdChess.Rule()) {
                    board.PutDown(UI, mousePosition);
                }
                else {
                    board.PutDown(UI, preMousePosition);
                }
                preMousePosition = mousePosition;
            }
        }

        private void ImageBoard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            //mousePosition = e.GetPosition(UI);
            //textBlock.Text = mousePosition.ToString();
        }

        private void imageBoard_MouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                //mousePosition = e.GetPosition(UI);
                //textBlock.Text = mousePosition.ToString();
            }
        }
    }
}
