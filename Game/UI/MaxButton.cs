using System.Windows.Controls;

namespace Game.UI
{
    public class MaxButton : Button
    {
        private const string MaxText = "◇";
        private const string WindowFyText = "◈";
        private TextBlock ButtonText;

        public MaxButton()
        {
            this.Click += MaxButton_Click;
            this.Loaded += MaxButton_Loaded;
            //Global.MainWindow.MainCanvas.SizeChanged += MainCanvas_SizeChanged;
            ButtonText = new TextBlock();
            ButtonText.Text = MaxText;
            ButtonText.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            ButtonText.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            this.AddChild(ButtonText);
        }

        private void MaxButton_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.DataContext.GetType() == typeof(ProgramWindowNoButtons))
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
                (this.Parent as Control).Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void MainCanvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            Control programWindow = (Control)this.DataContext;
            if (programWindow == null)
            {
                return;
            }
            bool Maxed = false;//programWindow.maxed;

            if (Maxed)
            {
                programWindow.Width = Global.MainWindow.MainCanvas.ActualWidth;
                programWindow.Height = Global.MainWindow.MainCanvas.ActualHeight;
                Canvas.SetLeft(programWindow, 0);
                Canvas.SetTop(programWindow, 0);
            }
        }

        private void MaxButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ProgramWindow programWindow = (ProgramWindow)this.DataContext;

            if (programWindow == null)
            {
                return;
            }
            bool Maxed = programWindow.maxed;

            if (!Maxed)
            {
                programWindow.WindowedHeight = programWindow.Height;
                programWindow.WindowedWidth = programWindow.Width;
                programWindow.WindowedLeft = Canvas.GetLeft(programWindow);
                programWindow.WindowedTop = Canvas.GetTop(programWindow);
                programWindow.Width = Global.MainWindow.MainCanvas.ActualWidth;
                programWindow.Height = Global.MainWindow.MainCanvas.ActualHeight;
                this.ButtonText.Text = WindowFyText;
                Canvas.SetLeft(programWindow, 0);
                Canvas.SetTop(programWindow, 0);

                Maxed = true;
            }
            else if (Maxed)
            {
                programWindow.Width = programWindow.WindowedWidth;
                programWindow.Height = programWindow.WindowedHeight;
                Canvas.SetLeft(programWindow, programWindow.WindowedLeft);
                Canvas.SetTop(programWindow, programWindow.WindowedTop);
                this.ButtonText.Text = MaxText;
                Maxed = false;
            }
            programWindow.maxed = Maxed;
        }
    }
}