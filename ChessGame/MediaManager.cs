using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessGame
{
    public static class MediaManager
    {
        public static void ChessPutDown()
        {
            var player = new SoundPlayer
            {
                SoundLocation = @".\sound\ChessDrop.wav"
            };
            player.Play();
        }

        public static Image GetChessImage(bool isWhite, string name, double size)
        {
            return new Image()
            {
                //Stretch = Stretch.Fill,
                Width = size,
                Height = size,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 0, 0),
                Source = GetChessImageSource(isWhite, name)
            };
        }

        private static ImageSource GetChessImageSource(bool isWhite, string name)
        {
            string fileName = isWhite ? $"img/w{name}.png" : $"img/b{name}.png";
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fileName, UriKind.Relative);
            bitmap.EndInit();
            return bitmap;
        }
    }
}
