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
            //(this.MapControlFrame.Template.FindName("PageFrame", this.MapControlFrame) as System.Windows.Controls.Frame).Navigate(Global.EndPointMap);
            this.MapControlFrame.Navigate(Global.EndPointMap);
            (this.IRCFrame.Template.FindName("PageFrame", this.IRCFrame) as System.Windows.Controls.Frame).Navigate(Global.IRCWindow);

            //this.BounceTaskButton.IsChecked = false;
            //this.PromptTaskButton.IsChecked = false;
            //this.TracONTaskButton.IsChecked = false;
            //this.ZeeChatTaskButton.IsChecked = false;
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

        private void MapControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ToggleButton toggelButton = BounceTaskBarButton.Content as ToggleButton;
            if (MapControl.IsVisible)
            {
                toggelButton.IsChecked = true;
                BounceTaskBarButton.Style = FindResource("TaskBarButtonBoxInverted") as Style;
                return;
            }
            toggelButton.IsChecked = false;
            BounceTaskBarButton.Style = FindResource("TaskBarButtonBox") as Style;
        }

        private void RemoteConsole_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ToggleButton toggelButton = RemoteConsoleTaskBarButton.Content as ToggleButton;
            if (RemoteConsole.IsVisible)
            {
                toggelButton.IsChecked = true;
                RemoteConsoleTaskBarButton.Style = FindResource("TaskBarButtonBoxInverted") as Style;
                return;
            }
            toggelButton.IsChecked = false;
            RemoteConsoleTaskBarButton.Style = FindResource("TaskBarButtonBox") as Style;
        }

        private void LocalConsole_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ToggleButton toggelButton = LocalConsoleTaskBarButton.Content as ToggleButton;
            if (LocalConsole.IsVisible)
            {
                toggelButton.IsChecked = true;
                LocalConsoleTaskBarButton.Style = FindResource("TaskBarButtonBoxInverted") as Style;
                return;
            }
            toggelButton.IsChecked = false;
            LocalConsoleTaskBarButton.Style = FindResource("TaskBarButtonBox") as Style;
        }

        private void IRC_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ToggleButton toggelButton = ZeeChatTaskBarButton.Content as ToggleButton;
            if (IRC.IsVisible)
            {
                toggelButton.IsChecked = true;
                ZeeChatTaskBarButton.Style = FindResource("TaskBarButtonBoxInverted") as Style;
                return;
            }
            toggelButton.IsChecked = false;
            ZeeChatTaskBarButton.Style = FindResource("TaskBarButtonBox") as Style;
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

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            ContentControl contentControl = toggleButton.Parent as ContentControl;
            Debug.WriteLine("Togglebutton is checked" + toggleButton.IsChecked.ToString());
            if (toggleButton.IsChecked == true)
            {
                contentControl.Style = FindResource("TaskBarButtonBoxInverted") as Style;
            }
            else
            {
                contentControl.Style = FindResource("TaskBarButtonBox") as Style;
            }

            if (contentControl == this.BounceTaskBarButton)
            {
                if (this.MapControl.IsVisible)
                {
                    this.MapControl.Visibility = Visibility.Collapsed;
                    this.MapControl.IsEnabled = false;
                    return;
                }
                this.MapControl.Visibility = Visibility.Visible;
                this.MapControl.IsEnabled = true;
            }
            else if (contentControl == this.ZeeChatTaskBarButton)
            {
                if (this.IRC.IsVisible)
                {
                    this.IRC.Visibility = Visibility.Collapsed;
                    this.IRC.IsEnabled = false;
                    return;
                }
                this.IRC.Visibility = Visibility.Visible;
                this.IRC.IsEnabled = true;
            }
            else if (contentControl == this.LocalConsoleTaskBarButton)
            {
                if (this.LocalConsole.IsVisible)
                {
                    this.LocalConsole.Visibility = Visibility.Collapsed;
                    this.LocalConsole.IsEnabled = false;
                    return;
                }
                this.LocalConsole.Visibility = Visibility.Visible;
                this.LocalConsole.IsEnabled = true;
            }
            else if (contentControl == this.RemoteConsoleTaskBarButton)
            {
                if (this.RemoteConsole.IsVisible)
                {
                    this.RemoteConsole.Visibility = Visibility.Collapsed;
                    this.RemoteConsole.IsEnabled = false;
                    return;
                }
                this.RemoteConsole.Visibility = Visibility.Visible;
                this.RemoteConsole.IsEnabled = true;
            }
        }
    }
}