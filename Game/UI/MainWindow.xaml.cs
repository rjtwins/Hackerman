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
        }
    }
}