using ChessGame.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessGame
{
    /// <summary>
    /// PromotionUC.xaml 的互動邏輯
    /// </summary>
    public partial class PromotionUI : UserControl
    {
        public event EventHandler<ChessPiece>? ChessPieceSelected;

        public PromotionUI()
        {
            InitializeComponent();
        }

        public PromotionUI(ChessPiece chess, Point mousePosition) : this()
        {
            //Margin = new Thickness(0, 0, 0, 0);
            Margin = new Thickness(mousePosition.X, mousePosition.Y, 0, 0);

            var isWhite = chess.IsWhite;
            var QueenImage = MediaManager.GetChessImage(isWhite, "Q", ChessPiece.Size);
            var KnightImage = MediaManager.GetChessImage(isWhite, "N", ChessPiece.Size);
            var RookImage = MediaManager.GetChessImage(isWhite, "R", ChessPiece.Size);
            var BishopImage = MediaManager.GetChessImage(isWhite, "B", ChessPiece.Size);

            QueenImage.Tag = new Queen(isWhite) { Coord = chess.Coord};
            KnightImage.Tag = new Knight(isWhite) { Coord = chess.Coord };
            RookImage.Tag = new Rook(isWhite) { Coord = chess.Coord };
            BishopImage.Tag = new Bishop(isWhite) { Coord = chess.Coord };

            QueenImage.MouseDown += ChessPiece_Click;
            KnightImage.MouseDown += ChessPiece_Click;
            RookImage.MouseDown += ChessPiece_Click;
            BishopImage.MouseDown += ChessPiece_Click;

            grid.AddChild(QueenImage, 0, 0);
            grid.AddChild(KnightImage, 0, 1);
            grid.AddChild(RookImage, 1, 0);
            grid.AddChild(BishopImage, 1, 1);
        }

        private void ChessPiece_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Image selectedImage)
                ChessPieceSelected?.Invoke(this, (ChessPiece)selectedImage.Tag);
        }
    }
}
