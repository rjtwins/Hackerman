using Game.UI.Pages;
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
        public List<ProgramWindow> ContentControlElements = new();
        public List<Button> StarMenuButtons = new();
        private bool SkipedSplash = false;
        private bool PlayingSetup = true;
        private Dictionary<string, Button> StartMenuButtonsDict { get; set; } = new Dictionary<string, Button>();
        public MainWindow()
        {
            this.Loaded += MainWindow_Loaded;
            InitializeComponent();
        }

        public void ShowGameScreen()
        {
            //this.RemoteConsoleFrame.Navigate(Global.RemoteConsole);
            //this.LocalConsoleFrame.Navigate(Global.LocalConsole);
            //this.MapControlFrame.Navigate(Global.EndPointMap);
            //this.IRCFrame.Navigate(Global.IRCWindow);
            //this.IPDailerFrame.Navigate(new IPDailer());
            //this.BankFrame.Navigate(new BankPage());

            //ContentControlElements.Add(RemoteConsole);
            //ContentControlElements.Add(LocalConsole);
            //ContentControlElements.Add(MapControl);
            //ContentControlElements.Add(IRC);
            //ContentControlElements.Add(SystemTime);

            this.FullWindowFrame.Visibility = Visibility.Collapsed;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => { PlaySetup(); });
            new ProgramWindow(Global.SystemTime);
            new ProgramWindow(Global.EndPointMap);
            new ProgramWindow(Global.IRCWindow);
            new ProgramWindow(Global.LocalConsole);
            new ProgramWindow(Global.RemoteConsole);
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
                    Global.SystemTime.UpdateTime(text1);
                });
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                //Do nothing
            }
        }

        public void RemoveFromTaskBar(ContentControl taskBarButton)
        {
            this.TaskBar.Children.Remove(taskBarButton);
        }

        public void AddStartMenuButton(string name, Action action)
        {
            if (this.StartMenuButtonsDict.ContainsKey(name))
            {
                return;
            }
            Button btn = new Button();
            btn.Content = name;
            btn.Click += (sender, e) => { action(); };
            this.StarMenuButtons.Insert(0, new Button());
            StartMenuButtonsDict[name] = btn;
        }

        public void RemoveStartMenuButton(string buttonName)
        {
            if (this.StartMenuButtonsDict.ContainsKey(buttonName))
            {
                this.StartMenuButtonsDict.Remove(buttonName);
            }
        }

        public void AddToTaskBar(ContentControl taskBarButton)
        {
            this.TaskBar.Children.Add(taskBarButton);
        }

        public void SetOntop(ContentControl contentControl)
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
            //ToggleButton toggleButton = sender as ToggleButton;
            //ContentControl contentControl = toggleButton.Parent as ContentControl;
            //if (toggleButton.IsChecked == true)
            //{
            //    contentControl.Style = FindResource("TaskBarButtonBoxInverted") as Style;
            //}
            //else
            //{
            //    contentControl.Style = FindResource("TaskBarButtonBox") as Style;
            //}

            //if (contentControl == this.BounceTaskBarButton)
            //{
            //    if (this.MapControl.IsVisible)
            //    {
            //        this.MapControl.Visibility = Visibility.Collapsed;
            //        this.MapControl.IsEnabled = false;
            //        RemoteConsole.Style = (Style)FindResource("ProgramWindowInActiveStyle");
            //        return;
            //    }
            //    this.MapControl.Visibility = Visibility.Visible;
            //    this.MapControl.IsEnabled = true;
            //    this.SetOntop(this.MapControl);
            //    MapControl.Style = (Style)FindResource("ProgramWindowActiveStyle");

            //}
            //else if (contentControl == this.ZeeChatTaskBarButton)
            //{
            //    if (this.IRC.IsVisible)
            //    {
            //        this.IRC.Visibility = Visibility.Collapsed;
            //        this.IRC.IsEnabled = false;
            //        RemoteConsole.Style = (Style)FindResource("ProgramWindowInActiveStyle");
            //        return;
            //    }
            //    this.IRC.Visibility = Visibility.Visible;
            //    this.IRC.IsEnabled = true;
            //    this.SetOntop(this.IRC);
            //    IRC.Style = (Style)FindResource("ProgramWindowActiveStyle");
            //}
            //else if (contentControl == this.LocalConsoleTaskBarButton)
            //{
            //    if (this.LocalConsole.IsVisible)
            //    {
            //        this.LocalConsole.Visibility = Visibility.Collapsed;
            //        this.LocalConsole.IsEnabled = false;
            //        RemoteConsole.Style = (Style)FindResource("ProgramWindowInActiveStyle");
            //        return;
            //    }
            //    this.LocalConsole.Visibility = Visibility.Visible;
            //    this.LocalConsole.IsEnabled = true;
            //    this.SetOntop(this.LocalConsole);
            //    LocalConsole.Style = (Style)FindResource("ProgramWindowActiveStyle");
            //}
            //else if (contentControl == this.RemoteConsoleTaskBarButton)
            //{
            //    if (this.RemoteConsole.IsVisible)
            //    {
            //        this.RemoteConsole.Visibility = Visibility.Collapsed;
            //        this.RemoteConsole.IsEnabled = false;
            //        RemoteConsole.Style = (Style)FindResource("ProgramWindowInActiveStyle");
            //        return;
            //    }
            //    this.RemoteConsole.Visibility = Visibility.Visible;
            //    this.RemoteConsole.IsEnabled = true;
            //    this.SetOntop(this.RemoteConsole);
            //    RemoteConsole.Style = (Style)FindResource("ProgramWindowActiveStyle");
            //}
        }

        private void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            ContentControl contentControl = toggleButton.Parent as ContentControl;

            if (toggleButton.IsChecked == true)
            {
                this.StartMenuMenu.Visibility = Visibility.Visible;
            }
            else
            {
                this.StartMenuMenu.Visibility = Visibility.Collapsed;
            }
            UpdateTastBarMenuButtonStyle();
        }

        private void UpdateTastBarMenuButtonStyle()
        {
            if(StartMenuMenu.Visibility == Visibility.Visible)
            {
                StartMenu.Style = FindResource("TaskBarButtonBoxInverted") as Style;
                return;
            }
            StartMenu.Style = FindResource("TaskBarButtonBox") as Style;
        }

        private void Window_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = false;
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
                UpdateTastBarMenuButtonStyle();
                return;
            }

            if (c.Name != "StartMenuMenu" && c.Name != "StartMenu" && c.Name != "StarMenuToggleButton")
            {
                StartMenuMenu.Visibility = Visibility.Collapsed;
                (StartMenu.Content as ToggleButton).IsChecked = false;
                UpdateTastBarMenuButtonStyle();
            }
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (PlayingSetup)
            {
                SkipPlaySetup();
            }
            e.Handled = false;
        }
    }
}