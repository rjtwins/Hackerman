using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Game.UI
{
    internal class PopUpWindow : ProgramWindow
    {
        private string Header = string.Empty;
        private string Message = string.Empty;

        private UIElement Sender;

        //double X;
        //double Y;
        public PopUpWindow(string header, string message, UIElement sender) : base(null)
        {
            //Global.MainWindow.MainCanvas.Children.Add(this);
            Canvas.SetLeft(this, Global.MainWindow.MainCanvas.ActualWidth / 2);
            Canvas.SetTop(this, Global.MainWindow.MainCanvas.ActualHeight / 2);
            Canvas.SetZIndex(this, int.MaxValue);

            this.DataContext = this;
            this.Loaded += PopUpWindow_Loaded;
            this.Style = (Style)FindResource("ProgramWindowActiveStyle");
            this.title = new TextBlock();
            //this.mainContent = new TextBlock();
            this.Header = header;
            this.Message = message;
            this.Sender = sender;
            sender.IsEnabled = false;
            //this.X = x;
            //this.Y = y;
            this.HasControlButtons = false;
        }

        private void PopUpWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.title.Text = Header;
            this.title.Foreground = Brushes.White;
            StackPanel stp = new StackPanel();
            TextBlock txb = new TextBlock();
            ContentControl btnContent = new ContentControl();
            Button btn = new Button();
            btn.Click += Btn_Click;

            txb.Text = Message;
            txb.Foreground = Brushes.Black;
            txb.Margin = new Thickness(5, 5, 5, 5);
            txb.HorizontalAlignment = HorizontalAlignment.Left;
            txb.FontSize = 12;

            btn.Content = "OK";
            btn.Margin = new Thickness(-1);
            btn.Padding = new Thickness(15, 2, 15, 2);

            btnContent.Content = btn;
            btnContent.FontSize = 20;
            btnContent.Margin = new Thickness(5, 10, 5, 5);
            btnContent.Padding = new Thickness(10, 2, 10, 2);
            btnContent.FontWeight = FontWeights.Bold;
            btnContent.Foreground = Brushes.Black;
            btnContent.HorizontalAlignment = HorizontalAlignment.Center;
            btnContent.Style = (Style)FindResource("TaskBarButtonBox");

            stp.Children.Add(txb);
            stp.Children.Add(btnContent);

            this.mainContent = stp;

            this.Width = stp.Width;
            this.Height = stp.Height;
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Sender.IsEnabled = true;
            Global.MainWindow.MainCanvas.Children.Remove(this);
        }
    }
}