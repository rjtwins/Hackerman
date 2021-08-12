using System.Windows.Controls;
using System.Windows.Media;

namespace Game.UI
{
    internal class MinButton : Button
    {
        public MinButton()
        {
            this.Click += MinButton_Click;
            this.BorderBrush = Brushes.Transparent;
            this.Background = Brushes.Transparent;
        }

        private void MinButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Control ContextedControl = this.DataContext as Control;
            if (ContextedControl == null)
            {
                return;
            }
        }
    }
}