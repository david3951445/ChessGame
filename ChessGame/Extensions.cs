using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace ChessGame
{
    public static class Extensions
    {
        public static void AddChild(this Grid grid, UIElement element, int column, int row)
        {
            grid.Children.Add(element);
            Grid.SetColumn(element, column);
            Grid.SetRow(element, row);
        }
    }
}
