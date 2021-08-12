using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace Game.UI
{
    internal class MaxButton : Button
    {
        private const string MaxText = "◇";
        private const string WindowFyText = "◈";
        private TextBlock ButtonText;

        public MaxButton()
        {
            this.Click += MaxButton_Click;
            Global.MainWindow.MainCanvas.SizeChanged += MainCanvas_SizeChanged;
            this.BorderBrush = Brushes.Transparent;
            this.Background = Brushes.Transparent;
            ButtonText = new TextBlock();
            ButtonText.Text = MaxText;
            this.AddChild(ButtonText);
        }

        private void MainCanvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            Control ContextedControl = this.DataContext as Control;
            if (ContextedControl == null)
            {
                return;
            }
            bool Maxed = (bool)ContextedControl.Resources["Maxed"];

            if (Maxed)
            {
                ContextedControl.Width = Global.MainWindow.MainCanvas.ActualWidth;
                ContextedControl.Height = Global.MainWindow.MainCanvas.ActualHeight;
                Canvas.SetLeft(ContextedControl, 0);
                Canvas.SetTop(ContextedControl, 0);
            }
        }

        private void MaxButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Control ContextedControl = this.DataContext as Control;
            Debug.WriteLine(ContextedControl.GetType());

            if (ContextedControl == null)
            {
                return;
            }
            bool Maxed = (bool)ContextedControl.Resources["Maxed"];

            if (!Maxed)
            {
                ContextedControl.Width = Global.MainWindow.MainCanvas.ActualWidth;
                ContextedControl.Height = Global.MainWindow.MainCanvas.ActualHeight;
                this.ButtonText.Text = WindowFyText;
                Canvas.SetLeft(ContextedControl, 0);
                Canvas.SetTop(ContextedControl, 0);

                Maxed = true;
            }
            else if (Maxed)
            {
                ContextedControl.Width = 400;
                ContextedControl.Height = 400;
                Canvas.SetLeft(ContextedControl, 200);
                Canvas.SetTop(ContextedControl, 200);
                this.ButtonText.Text = MaxText;
                Maxed = false;
            }
            ContextedControl.Resources["Maxed"] = Maxed;
        }
    }
}