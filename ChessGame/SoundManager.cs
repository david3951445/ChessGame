using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame
{
    internal static class SoundManager
    {
        public static void ChessPutDown() {
            var player = new SoundPlayer();
            player.SoundLocation = @".\sound\ChessDrop.wav";
            player.Play();
        }
    }
}
