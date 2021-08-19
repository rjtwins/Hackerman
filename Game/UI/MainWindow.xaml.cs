using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Game.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            Global.MainWindow = this;
            InitializeComponent();
        }

        //TODO move this on another load call then this one
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            (this.RemoteConsoleFrame.Template.FindName("PageFrame", this.RemoteConsoleFrame) as System.Windows.Controls.Frame).Navigate(Global.RemoteConsole);
            (this.LocalConsoleFrame.Template.FindName("PageFrame", this.LocalConsoleFrame) as System.Windows.Controls.Frame).Navigate(Global.LocalConsole);
            (this.MapControlFrame.Template.FindName("PageFrame", this.MapControlFrame) as System.Windows.Controls.Frame).Navigate(Global.EndPointMap);
            (this.IRCFrame.Template.FindName("PageFrame", this.IRCFrame) as System.Windows.Controls.Frame).Navigate(Global.IRCWindow);

            this.BounceTaskButton.IsChecked = false;
            this.PromptTaskButton.IsChecked = false;
            this.TracONTaskButton.IsChecked = false;
            this.ZeeChatTaskButton.IsChecked = false;
            //this.DebugControlFrame.Navigate(new DebugPage());
        }

        internal void UpdateDateTime()
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    string text = string.Empty;
                    text += Global.GameTime.Hour + ":" + Global.GameTime.Minute + "\n"
                    + Global.GameTime.Day + "-" + Global.GameTime.Month + "-" + Global.GameTime.Year;
                    this.GameTimeTextBlock.Text = text;
                });
                
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                //Do nothing
            }
        }

        private void DecreaseGameSpeedButton_Click(object sender, RoutedEventArgs e)
        {
            Global.EventTicker.DecreaseGameSpeed();
            UpdateGameSpeedIndicator();
        }

        private void PauseGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (Global.GamePaused)
            {
                Global.EventTicker.StartTicker();
                UpdateGameSpeedIndicator();
                return;
            }
            Global.EventTicker.StopTicker();
            GameSpeedTextBlock.Text = "0";
        }

        private void IncreaseGameSpeedButton_Click(object sender, RoutedEventArgs e)
        {
            Global.EventTicker.IncreaseGameSpeed();
            UpdateGameSpeedIndicator();
        }

        private void UpdateGameSpeedIndicator()
        {
            GameSpeedTextBlock.Text = Global.EventTicker.GameSpeed.ToString();
        }

        private void BounceTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.MapControl.IsEnabled)
            {
                this.MapControl.Visibility = Visibility.Collapsed;
                this.MapControl.IsEnabled = false;
                return;
            }
            this.MapControl.Visibility = Visibility.Visible;
            this.MapControl.IsEnabled = true;
        }

        private void ZeeChatTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.IRC.IsEnabled)
            {
                this.IRC.Visibility = Visibility.Collapsed;
                this.IRC.IsEnabled = false;
                return;
            }
            this.IRC.Visibility = Visibility.Visible;
            this.IRC.IsEnabled = true;
        }

        private void TracONTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.RemoteConsole.IsEnabled)
            {
                this.RemoteConsole.Visibility = Visibility.Collapsed;
                this.RemoteConsole.IsEnabled = false;
                return;
            }
            this.RemoteConsole.Visibility = Visibility.Visible;
            this.RemoteConsole.IsEnabled = true;
        }

        private void PromptTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.LocalConsole.IsEnabled)
            {
                this.LocalConsole.Visibility = Visibility.Collapsed;
                this.LocalConsole.IsEnabled = false;
                return;
            }
            this.LocalConsole.Visibility = Visibility.Visible;
            this.LocalConsole.IsEnabled = true;
        }

        private void BounceTaskButton_Loaded(object sender, RoutedEventArgs e)
        {
            //do nothing
        }

        private void MapControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (MapControl.IsVisible)
            {
                BounceTaskButton.IsChecked = false;
                return;
            }
            BounceTaskButton.IsChecked = true;
        }

        private void RemoteConsole_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (RemoteConsole.IsVisible)
            {
                TracONTaskButton.IsChecked = false;
                return;
            }
            TracONTaskButton.IsChecked = true;
        }

        private void LocalConsole_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (LocalConsole.IsVisible)
            {
                PromptTaskButton.IsChecked = false;
                return;
            }
            PromptTaskButton.IsChecked = true;
        }

        private void IRC_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IRC.IsVisible)
            {
                ZeeChatTaskButton.IsChecked = false;
                return;
            }
            ZeeChatTaskButton.IsChecked = true;
        }

        private void MapControl_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void WindowContentControlMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            Brush A = new LinearGradientBrush(Colors.Gray, Colors.White, 1);
            Brush B = new LinearGradientBrush(Colors.Navy, (Color)ColorConverter.ConvertFromString("#FF195689"), 1);

            ContentControl c = sender as ContentControl;

            if (c == null)
            {
                return;
            }

            Debug.WriteLine(c.Name);


            (this.RemoteConsole.FindName("RemoteGradientGrid") as Grid).Background = A;
            (this.LocalConsole.FindName("LocalGradientGrid") as Grid).Background = A;
            (this.MapControl.FindName("GradientGrid") as Grid).Background = A;
            (this.IRC.FindName("IRCGradientGrid") as Grid).Background = A;

            if (c.Name == RemoteConsole.Name)
            {
                (this.RemoteConsole.FindName("RemoteGradientGrid") as Grid).Background = B;
            }
            if (c.Name == LocalConsole.Name)
            {
                (this.LocalConsole.FindName("LocalGradientGrid") as Grid).Background = B;
            }
            if (c.Name == MapControl.Name)
            {
                (this.MapControl.FindName("GradientGrid") as Grid).Background = B;
            }
            if (c.Name == IRC.Name)
            {
                (this.IRC.FindName("IRCGradientGrid") as Grid).Background = B;
            }

        }

        private void MapControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}