using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Game.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private List<ProgramWindow> ContentControlElements = new();
        private bool SkipedSplash = false;
        private bool PlayingSetup = true;


        public MainWindow()
        {
            this.Loaded += MainWindow_Loaded;
            InitializeComponent();
        }

        public void ShowGameScreen()
        {
            this.RemoteConsoleFrame.Navigate(Global.RemoteConsole);
            this.LocalConsoleFrame.Navigate(Global.LocalConsole);
            this.MapControlFrame.Navigate(Global.EndPointMap);
            this.IRCFrame.Navigate(Global.IRCWindow);

            ContentControlElements.Add(RemoteConsole);
            ContentControlElements.Add(LocalConsole);
            ContentControlElements.Add(MapControl);
            ContentControlElements.Add(IRC);
            ContentControlElements.Add(SystemTime);

            this.FullWindowFrame.Visibility = Visibility.Collapsed;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => { PlaySetup(); });
        }

        public void SkipPlaySetup()
        {
            this.SkipedSplash = true;
            FinshedPlaySetup();
        }

        private void PlaySetup()
        {
            Global.App.Dispatcher.Invoke(() => { this.FullWindowFrame.Navigate(Global.SplashPage); });
            System.Threading.Thread.Sleep(7000);
            Global.App.Dispatcher.Invoke(() => { this.FullWindowFrame.Navigate(Global.SplashPage2); });
            System.Threading.Thread.Sleep(3000);
            
            Global.App.Dispatcher.Invoke(() => 
            {
                if (!SkipedSplash)
                {
                    this.FinshedPlaySetup();
                }
            });
        }

        private void FinshedPlaySetup()
        {
            PlayingSetup = false;
            Global.App.FinshedPlaySetup();
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
                    this.GameTimeHHMMTextBlock.Text = text1;
                    this.SystemTimeBoxTime.Text = text1;
                });
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                //Do nothing
            }
        }

        #region ProgramWindow Visibility

        private void MapControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ToggleButton toggelButton = BounceTaskBarButton.Content as ToggleButton;
            if (MapControl.IsVisible)
            {
                toggelButton.IsChecked = true;
                BounceTaskBarButton.Style = FindResource("TaskBarButtonBoxInverted") as Style;
                this.SetOntop(MapControl);
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
                this.SetOntop(MapControl);
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
                this.SetOntop(MapControl);
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
                this.SetOntop(MapControl);
                return;
            }
            toggelButton.IsChecked = false;
            ZeeChatTaskBarButton.Style = FindResource("TaskBarButtonBox") as Style;
        }

        #endregion ProgramWindow Visibility

        private void SetOntop(ContentControl contentControl)
        {
            int maxZ = 0;
            foreach (ContentControl c in this.ContentControlElements)
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
                    RemoteConsole.Style = (Style)FindResource("ProgramWindowInActiveStyle");
                    return;
                }
                this.MapControl.Visibility = Visibility.Visible;
                this.MapControl.IsEnabled = true;
                this.SetOntop(this.MapControl);
                MapControl.Style = (Style)FindResource("ProgramWindowActiveStyle");

            }
            else if (contentControl == this.ZeeChatTaskBarButton)
            {
                if (this.IRC.IsVisible)
                {
                    this.IRC.Visibility = Visibility.Collapsed;
                    this.IRC.IsEnabled = false;
                    RemoteConsole.Style = (Style)FindResource("ProgramWindowInActiveStyle");
                    return;
                }
                this.IRC.Visibility = Visibility.Visible;
                this.IRC.IsEnabled = true;
                this.SetOntop(this.IRC);
                IRC.Style = (Style)FindResource("ProgramWindowActiveStyle");
            }
            else if (contentControl == this.LocalConsoleTaskBarButton)
            {
                if (this.LocalConsole.IsVisible)
                {
                    this.LocalConsole.Visibility = Visibility.Collapsed;
                    this.LocalConsole.IsEnabled = false;
                    RemoteConsole.Style = (Style)FindResource("ProgramWindowInActiveStyle");
                    return;
                }
                this.LocalConsole.Visibility = Visibility.Visible;
                this.LocalConsole.IsEnabled = true;
                this.SetOntop(this.LocalConsole);
                LocalConsole.Style = (Style)FindResource("ProgramWindowActiveStyle");
            }
            else if (contentControl == this.RemoteConsoleTaskBarButton)
            {
                if (this.RemoteConsole.IsVisible)
                {
                    this.RemoteConsole.Visibility = Visibility.Collapsed;
                    this.RemoteConsole.IsEnabled = false;
                    RemoteConsole.Style = (Style)FindResource("ProgramWindowInActiveStyle");
                    return;
                }
                this.RemoteConsole.Visibility = Visibility.Visible;
                this.RemoteConsole.IsEnabled = true;
                this.SetOntop(this.RemoteConsole);
                RemoteConsole.Style = (Style)FindResource("ProgramWindowActiveStyle");
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

        private void Window_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Skip splash if playing
            if (PlayingSetup)
            {
                SkipPlaySetup();
            }


            Control c = e.Source as Control;
            if (c == null)
            {
                StartMenuMenu.Visibility = Visibility.Collapsed;
                (StartMenu.Content as ToggleButton).IsChecked = false;
                MenuButtonClick((StartMenu.Content as ToggleButton), null);
                return;
            }
            if (c.Name != "StartMenuMenu")
            {
                StartMenuMenu.Visibility = Visibility.Collapsed;
                (StartMenu.Content as ToggleButton).IsChecked = false;
                MenuButtonClick((StartMenu.Content as ToggleButton), null);
            }

            foreach (ProgramWindow programWindow in this.ContentControlElements)
            {
                if (programWindow.Name != c.Name)
                {
                    programWindow.Style = (Style)FindResource("ProgramWindowInActiveStyle");
                    continue;
                }
                programWindow.Style = (Style)FindResource("ProgramWindowActiveStyle");
                SetOntop(programWindow);
            }
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (PlayingSetup)
            {
                SkipPlaySetup();
            }
        }
    }
}