using System;
using System.Windows;

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

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //this.TestFrame2.Navigate(Global.Console);
            //this.TestFrame.Navigate(new UI.Page());

            this.ConsoleFrame.Navigate(Global.Console);
            this.MapFrame.Navigate(Global.EndPointMap);
            this.DebugControlFrame.Navigate(new DebugPage());
        }

        internal void UpdateDateTime()
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.GameTimeTextBlock.Text = Global.GameTime.ToString("yyy-MM-dd-HH-mm"); //mm-HH-dd-MM-yyy
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
    }
}