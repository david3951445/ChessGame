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
        public static void AddChild(this Grid grid, UIElement element, int row, int column)
        {
            grid.Children.Add(element);
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
        }

        public static IEnumerable<T> NonNull<T>(this IEnumerable<T?> items) where T : class
            => items.Where(item => item != null).Select(item => item!);
    }
}
