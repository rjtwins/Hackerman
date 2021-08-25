using System;
using System.Collections.Generic;
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
        private List<ContentControl> ContentControlElements = new();

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

            ContentControlElements.Add(RemoteConsole);
            ContentControlElements.Add(LocalConsole);
            ContentControlElements.Add(MapControl);
            ContentControlElements.Add(IRC);
        }

        internal void UpdateDateTime()
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    string text1 = Global.GameTime.Hour + ":" + Global.GameTime.Minute;
                    string text2 = Global.GameTime.Day + "-" + Global.GameTime.Month + "-" + Global.GameTime.Year;
                    this.GameTimeDDMMYYextBlock.Text = text2;
                    this.GameTimeHHMMTextBlock.Text =text1;
                    this.SystemTimeBoxTime.Text = text1;
                });
                
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                //Do nothing
            }
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
                SetOntop(RemoteConsole);
            }
            if (c.Name == LocalConsole.Name)
            {
                (this.LocalConsole.FindName("LocalGradientGrid") as Grid).Background = B;
                SetOntop(LocalConsole);
            }
            if (c.Name == MapControl.Name)
            {
                (this.MapControl.FindName("GradientGrid") as Grid).Background = B;
                SetOntop(MapControl);
            }
            if (c.Name == IRC.Name)
            {
                (this.IRC.FindName("IRCGradientGrid") as Grid).Background = B;
                SetOntop(IRC);
            }
        }

        private void SetOntop(ContentControl contentControl)
        {
            int maxZ = 0;
            ContentControl maxZContentControl;
            foreach(ContentControl c in this.ContentControlElements)
            {
                maxZ = Math.Max(maxZ, Canvas.GetZIndex(c));
            }
            Canvas.SetZIndex(contentControl, maxZ + 1);
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            ContentControl contentControl = toggleButton.Parent as ContentControl;
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

        private void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            ContentControl contentControl = toggleButton.Parent as ContentControl;

            if (toggleButton.IsChecked == true)
            {
                this.StartMenuMenu.Visibility = Visibility.Visible;
                contentControl.Style = FindResource("TaskBarButtonBoxInverted") as Style;
            }
            else
            {
                this.StartMenuMenu.Visibility = Visibility.Collapsed;
                contentControl.Style = FindResource("TaskBarButtonBox") as Style;
            }
        }

        private void Window_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Control c = e.Source as Control;
            if(c == null)
            {
                StartMenuMenu.Visibility = Visibility.Collapsed;
                (StartMenu.Content as ToggleButton).IsChecked = false;
                return;
            }
            if(c.Name != "StartMenuMenu")
            {
                StartMenuMenu.Visibility = Visibility.Collapsed;
                (StartMenu.Content as ToggleButton).IsChecked = false;
            }
        }

        private void GameSpeedButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            ContentControl parrent = radioButton.Parent as ContentControl;
            parrent.Style = FindResource("TaskBarButtonBoxInverted") as Style;
            switch (radioButton.Name)
            {
                case "PauseButton":
                    Global.EventTicker.StopTicker();
                    break;
                case "Speed1Button":
                    Global.EventTicker.ChangeSpeed(1);
                    Global.EventTicker.StartTicker();
                    break;
                case "Speed2Button":
                    Global.EventTicker.ChangeSpeed(2);
                    Global.EventTicker.StartTicker();
                    break;
                case "Speed3Button":
                    Global.EventTicker.ChangeSpeed(3);
                    Global.EventTicker.StartTicker();
                    break;
                case "Speed4Button":
                    Global.EventTicker.ChangeSpeed(4);
                    Global.EventTicker.StartTicker();
                    break;
                default:
                    break;
            }
        }

        private void GameSpeedButton_Unchecked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            ContentControl parrent = radioButton.Parent as ContentControl;
            parrent.Style = FindResource("TaskBarButtonBox") as Style;

        }
    }
}