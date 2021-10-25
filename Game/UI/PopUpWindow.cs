using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Game.UI
{
    internal class PopUpWindow : ProgramWindow
    {
        private string Header = string.Empty;
        private string Message = string.Empty;
        private Action<bool> RunOnComplete;
        private bool IsYesNoDialog = false;

        private UIElement Sender;

        //double X;
        //double Y;
        public PopUpWindow(string header, string message, UIElement sender, bool yesNo = false, Action<bool> runOnComplete = null) : base(null)
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
            this.RunOnComplete = runOnComplete;
            this.IsYesNoDialog = yesNo;
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


            txb.Text = Message;
            txb.Foreground = Brushes.Black;
            txb.Margin = new Thickness(5, 5, 5, 5);
            txb.HorizontalAlignment = HorizontalAlignment.Left;
            txb.FontSize = 12;
            stp.Children.Add(txb);

            if (IsYesNoDialog)
            {
                StackPanel btnPannel = new StackPanel();
                btnPannel.Orientation = Orientation.Horizontal;
                btnPannel.HorizontalAlignment = HorizontalAlignment.Center;
                btnPannel.VerticalAlignment = VerticalAlignment.Center;
                ContentControl y_btnContent = new ContentControl();
                ContentControl n_btnContent = new ContentControl();

                Button y_btn = new Button();
                y_btn.Click += Y_btn_Click;
                Button n_btn = new Button();
                n_btn.Click += N_btn_Click;
                y_btn.Content = "Yes";
                y_btn.Margin = new Thickness(-1);
                y_btn.Padding = new Thickness(15, 2, 15, 2);

                n_btn.Content = "No";
                n_btn.Margin = new Thickness(-1);
                n_btn.Padding = new Thickness(15, 2, 15, 2);

                y_btnContent.Content = y_btn;
                y_btnContent.FontSize = 20;
                y_btnContent.Margin = new Thickness(5, 10, 5, 5);
                y_btnContent.Padding = new Thickness(10, 2, 10, 2);
                y_btnContent.FontWeight = FontWeights.Bold;
                y_btnContent.Foreground = Brushes.Black;
                y_btnContent.HorizontalAlignment = HorizontalAlignment.Center;
                y_btnContent.Style = (Style)FindResource("TaskBarButtonBox");

                n_btnContent.Content = n_btn;
                n_btnContent.FontSize = 20;
                n_btnContent.Margin = new Thickness(5, 10, 5, 5);
                n_btnContent.Padding = new Thickness(10, 2, 10, 2);
                n_btnContent.FontWeight = FontWeights.Bold;
                n_btnContent.Foreground = Brushes.Black;
                n_btnContent.HorizontalAlignment = HorizontalAlignment.Center;
                n_btnContent.Style = (Style)FindResource("TaskBarButtonBox");

                btnPannel.Children.Add(y_btnContent);
                btnPannel.Children.Add(n_btnContent);
                stp.Children.Add(btnPannel);

            }
            else
            {
                ContentControl btnContent = new ContentControl();
                Button btn = new Button();
                btn.Click += Btn_Click;

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

                stp.Children.Add(btnContent);
            }

            this.mainContent = stp;
            this.Width = stp.Width;
            this.Height = stp.Height;
        }

        private void N_btn_Click(object sender, RoutedEventArgs e)
        {
            Cleanup();
            TryRunCompletionAction(false);
        }

        private void Y_btn_Click(object sender, RoutedEventArgs e)
        {
            Cleanup();
            TryRunCompletionAction(true);
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Cleanup();
            TryRunCompletionAction(true);
        }

        private void Cleanup()
        {
            Sender.IsEnabled = true;
            Global.MainWindow.MainCanvas.Children.Remove(this);
        }

        private void TryRunCompletionAction(bool result)
        {
            if (this.RunOnComplete != null)
            {
                RunOnComplete(result);
            }
        }
    }
}