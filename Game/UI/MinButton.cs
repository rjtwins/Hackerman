using System.Windows.Controls;

namespace Game.UI
{
    public class MinButton : Button
    {
        private TextBlock ButtonText;

        public MinButton()
        {
            this.Click += MinButton_Click;
            this.Loaded += MinButton_Loaded;
            ButtonText = new TextBlock();
            ButtonText.Text = "_";
            ButtonText.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            ButtonText.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            this.AddChild(ButtonText);
        }

        private void MinButton_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.DataContext.GetType() == typeof(ProgramWindowNoButtons))
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
                (this.Parent as Control).Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void MinButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Control ContextedControl = this.DataContext as Control;
            if (ContextedControl == null)
            {
                return;
            }
            if (ContextedControl.IsEnabled)
            {
                ContextedControl.IsEnabled = false;
                ContextedControl.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}